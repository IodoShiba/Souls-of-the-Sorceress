using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D)),DisallowMultipleComponent]
public class Mortal : MonoBehaviour,IodoShibaUtil.ManualUpdateClass.IManualUpdate
{
    public interface IDyingCallbackReceiver : UnityEngine.EventSystems.IEventSystemHandler
    {
        void OnSelfDying(DealtAttackInfo causeOfDeath);
    }

    public class DealtAttackInfo
    {

        public Mortal attacker;
        public AttackData attackData;
        public Vector2 relativePosition;
        public IOnAttackEvaluatedAction onAttackEvaluatedCallback;

        public DealtAttackInfo(Mortal attacker, AttackData attackData,Vector2 relativePosition, IOnAttackEvaluatedAction onAttackEvaluatedCallback)
        {
            this.attacker = attacker;
            this.attackData = attackData;
            this.relativePosition = relativePosition;
            this.onAttackEvaluatedCallback = onAttackEvaluatedCallback;
        }
    }

    public class Viewer : MonoBehaviour
    {
        [SerializeField] Mortal target;
        protected float Health { get => target == null ? 0 : target.health; }
        protected float MaxHealth { get => target.maxHealth; }
    }

    [System.Serializable] public class UnityEvent_Mortal : UnityEngine.Events.UnityEvent<Mortal> { }
    [System.Serializable] public class UnityEvent_float : UnityEngine.Events.UnityEvent<float> { }

    private const float impulseMultiplyer = 0.025f;
    [SerializeField] protected float health;
    [SerializeField] protected float maxHealth;
    [SerializeField] private  bool isInvulnerable;
    [SerializeField] UnityEngine.Events.UnityEvent dyingCallbacks;
    [SerializeField] public UnityEvent_Mortal onAttackedCallbacks; //Though this field is public, do not refer this field directly because it is just to avoid the UnityEvent Bug.
    [SerializeField] private UnityEvent_float onHitstopGiven;
    [SerializeField] UnityEngine.Events.UnityEvent onHealthRecoveredCallbacks;
    [SerializeField] UnityEngine.Events.UnityEvent onInvinsibleTimeOver;
    [SerializeField] UnityEngine.Events.UnityEvent onDestroy;
    [SerializeField] Rigidbody2D selfRigidbody;
    [SerializeField] List<AttackConverter> dealingAttackConverters;
    [SerializeField] List<AttackConverter> dealtAttackConverters;

    AttackData argAttackData = new AttackData();
    GameObject argObj;
    System.Action<bool> argSucceedCallback;
    float leftStunTime = 0.3f;
    int dealtAttackCount = 0;
    List<DealtAttackInfo> dealtAttackInfos = new List<DealtAttackInfo>(4);
    Actor actor;
    int invincibleOrderedCount = 0;
    int originalLayer;

    public Actor Actor { get => actor == null ? (actor = GetComponent<Actor>()) : actor; }
    public bool TryGetActor(out Actor actor) => (actor = Actor) != null;
    public UnityEvent_Mortal OnAttackedCallbacks { get => onAttackedCallbacks; }
    public UnityEvent DyingCallbacks { get => dyingCallbacks; }
    public bool IsInvulnerable //{ get => isInvulnerable; set => isInvulnerable = value; }
    {
        get => isInvulnerable;
        set => isInvulnerable = value;
    }
    public bool IsInvincible { get => invincibleOrderedCount > 0; }
    public UnityEvent_float OnHitstopGiven { get => onHitstopGiven; }

    protected virtual void Awake()
    {
        //dyingCallbacks.AddListener(OnDying);
        invincibleOrderedCount = 0;
    }
    protected virtual void OnAttacked(GameObject attackObj,AttackData attack) { }

    protected virtual void OnTriedAttack(Mortal attacker, AttackData dealt, in Vector2 relativePosition)
    {

    }

    public virtual void OnDying(DealtAttackInfo causeOfDeath)
    {
        UnityEngine.EventSystems.ExecuteEvents.Execute<IDyingCallbackReceiver>(
            gameObject,
            null,
            (dyingCallbackReceiver, disposedEventData) => { dyingCallbackReceiver.OnSelfDying(causeOfDeath); }
            );
        Destroy(gameObject);
    }

    public void ConvertDealingAttack(AttackData dealee)
    {
        dealingAttackConverters.ForEach(dac => dac.Convert(dealee));
    }
    protected void ConvertDealtAttack(AttackData dealt)
    {
        //dealtAttackConverters.TrueForAll(dac => dac.Convert(dealt));
        dealtAttackConverters.ForEach(dac => dac.Convert(dealt));
    }
    

    public void TryAttack(GameObject argObj, AttackData argAttackData, System.Action<bool> succeedCallback)
    {
        this.argObj = argObj;
        AttackData.Copy(this.argAttackData, argAttackData);
        this.argSucceedCallback = succeedCallback;
    }
    public void TryAttack(Mortal attacker, AttackData argAttackData,in Vector2 relativePosition,IOnAttackEvaluatedAction onAttackEvaluatedCallback)
    {
        OnTriedAttack(attacker, argAttackData, relativePosition);

        if (IsInvulnerable) { return; }

        if (dealtAttackCount >= dealtAttackInfos.Count)
        {
            dealtAttackInfos.Add(new DealtAttackInfo(attacker, new AttackData(argAttackData), relativePosition, onAttackEvaluatedCallback));
        }
        else if (dealtAttackInfos[dealtAttackCount] == null)
        {
            dealtAttackInfos[dealtAttackCount] = new DealtAttackInfo(attacker, new AttackData(argAttackData), relativePosition, onAttackEvaluatedCallback);
        }
        else
        {
            dealtAttackInfos[dealtAttackCount].attacker = attacker;
            AttackData.Copy(dealtAttackInfos[dealtAttackCount].attackData, argAttackData);
            dealtAttackInfos[dealtAttackCount].relativePosition = relativePosition;
            dealtAttackInfos[dealtAttackCount].onAttackEvaluatedCallback = onAttackEvaluatedCallback;
        }
        dealtAttackCount++;
    }

    public void ManualUpdate()
    {
        if (dealtAttackCount == 0) { return; }

        float rxsum = 0;
        AttackData result = new AttackData(); // TODO: new 無しの実装にする
        DealtAttackInfo mainAttackInfo = dealtAttackInfos[0];//主な攻撃（攻撃力が最も高く、主な死因となりうる攻撃）

        for (int i = 0; i < dealtAttackCount; ++i) //1フレームの間に与えられた複数の攻撃と相対座標を統合する
        {
            dealtAttackConverters.ForEach(dac => dac.Convert(dealtAttackInfos[i].attackData));//攻撃の変換

            result.damage += dealtAttackInfos[i].attackData.damage;//ダメージ
            if(mainAttackInfo.attackData.damage < dealtAttackInfos[i].attackData.damage)
            {
                mainAttackInfo = dealtAttackInfos[i];
            }

            result.knockBackImpulse
                += new Vector2(
                    -Mathf.Sign(dealtAttackInfos[i].relativePosition.x)*dealtAttackInfos[i].attackData.knockBackImpulse.x,
                    dealtAttackInfos[i].attackData.knockBackImpulse.y);//ノックバック

            if(result.hitstopSpan < dealtAttackInfos[i].attackData.hitstopSpan)
            {
                result.hitstopSpan = dealtAttackInfos[i].attackData.hitstopSpan;
            }//ヒットストップ

            rxsum += dealtAttackInfos[i].relativePosition.x;

            
        }
        //dealtAttackCount = 0;

        //if (result.damage <= 0 && result.knockBackImpulse.magnitude < 0.01) { return; }//攻撃が無意味ならば処理を中断
        bool isValidAttack = result.damage > 0 || result.knockBackImpulse.magnitude >= 0.01;
        if (isValidAttack)//攻撃を受ける処理
        {
            float originalHealth = health;
            health -= result.damage; //体力を減算する
            selfRigidbody.velocity = Vector2.zero; //Actorの動きを止める
            selfRigidbody.AddForce(
                result.knockBackImpulse * impulseMultiplyer,
                //ForceMode2D.Force); //ノックバックを与える
                ForceMode2D.Impulse); //ノックバックを与える

            //ヒットストップを与える（未実装）
            GiveHitstop(result.hitstopSpan);

            OnAttackedCallbacks.Invoke(mainAttackInfo.attacker);//被攻撃時のコールバック関数を呼び出し
            for(int i = 0; i < dealtAttackCount; ++i)
            {
                if (dealtAttackInfos[i] != null){ dealtAttackInfos[i].onAttackEvaluatedCallback.OnAttackEvaluated(true, this, result); }
            }
            if (health <= 0 && originalHealth > 0)
            {
                health = 0;
                dyingCallbacks.Invoke();
                OnDying(mainAttackInfo);
            }
        }
        else
        {
            for (int i = 0; i < dealtAttackCount; ++i)
            {
                if (dealtAttackInfos[i] != null) { dealtAttackInfos[i].onAttackEvaluatedCallback.OnAttackEvaluated(false, this, result); }
            }
        }
        dealtAttackCount = 0;//攻撃を全て統合したのでカウンターを0にし、与えられた攻撃を忘却する

    }

    public void RecoverHealth(float amount)
    {
        health = Mathf.Clamp(health + amount, 0, maxHealth);
        onHealthRecoveredCallbacks.Invoke();
    }

    public void DestroySelf(float time) { Destroy(gameObject, time); }

    public void OrderInvincible(float time)
    {
        StartCoroutine(OrderInvincibleImple(time));
    }

    public void GiveHitstop(float time)
    {
        OnHitstopGiven.Invoke(time);
    }

    public void Suicide()
    {
        health = 0;
        dyingCallbacks.Invoke();
        OnDying(new DealtAttackInfo(this,new AttackData(), Vector2.zero, AttackEvaluatedActionDoNothing.Instance));
    }

    IEnumerator OrderInvincibleImple(float time)
    {
        if(invincibleOrderedCount == 0)
        {
            originalLayer = gameObject.layer;
            gameObject.layer = LayerMask.NameToLayer(LayerName.invincibleActor);
        }
        invincibleOrderedCount++;

        yield return new WaitForSeconds(time);

        if (invincibleOrderedCount > 0)
        {
            invincibleOrderedCount--;
            if (invincibleOrderedCount == 0)
            {
                gameObject.layer = originalLayer;
            }
        }
        onInvinsibleTimeOver.Invoke();
    }

    private void OnDestroy()
    {
        onDestroy.Invoke();
    }
}


