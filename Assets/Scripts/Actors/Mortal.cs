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

        public DealtAttackInfo(Mortal attacker, AttackData attackData,Vector2 relativePosition)
        {
            this.attacker = attacker;
            this.attackData = attackData;
            this.relativePosition = relativePosition;
        }
    }

    public class Viewer : MonoBehaviour
    {
        [SerializeField] Mortal target;
        protected float Health { get => target.health; }
        protected float MaxHealth { get => target.maxHealth; }
    }

    [SerializeField] protected float health;
    [SerializeField] protected float maxHealth;
    [SerializeField] private  bool isInvulnerable;
    [SerializeField] float initialStunTime = 0.3f;
    [SerializeField] UnityEngine.Events.UnityEvent dyingCallbacks;
    [SerializeField] UnityEngine.Events.UnityEvent onAttackedCallbacks;
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

    public bool IsInvulnerable { get => isInvulnerable; set => isInvulnerable = value; }
    public Actor Actor { get => actor == null ? (actor = GetComponent<Actor>()) : actor; }
    public UnityEngine.Events.UnityEvent OnAttackedCallbacks { get => onAttackedCallbacks; }
    public UnityEvent DyingCallbacks { get => dyingCallbacks; }

    protected virtual void Awake()
    {
        //dyingCallbacks.AddListener(OnDying);
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
    public void TryAttack(Mortal attacker, AttackData argAttackData,in Vector2 relativePosition)
    {
        OnTriedAttack(attacker, argAttackData, relativePosition);

        if (isInvulnerable) { return; }

        if (dealtAttackCount >= dealtAttackInfos.Count)
        {
            dealtAttackInfos.Add(new DealtAttackInfo(attacker, new AttackData(argAttackData), relativePosition));
        }
        else if (dealtAttackInfos[dealtAttackCount] == null)
        {
            dealtAttackInfos[dealtAttackCount] = new DealtAttackInfo(attacker, new AttackData(argAttackData), relativePosition);
        }
        else
        {
            dealtAttackInfos[dealtAttackCount].attacker = attacker;
            AttackData.Copy(dealtAttackInfos[dealtAttackCount].attackData, argAttackData);
            dealtAttackInfos[dealtAttackCount].relativePosition = relativePosition;
        }
        dealtAttackCount++;
    }

    public void ManualUpdate()
    {
        if (dealtAttackCount == 0) { return; }

        float rxsum = 0;
        AttackData result = new AttackData();
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
        if (result.damage > 0 || result.knockBackImpulse.magnitude >= 0.01)//攻撃を受ける処理
        {
            health -= result.damage; //体力を減算する
            selfRigidbody.velocity = Vector2.zero; //Actorの動きを止める
            selfRigidbody.AddForce(
                result.knockBackImpulse,
                ForceMode2D.Force); //ノックバックを与える

            //ヒットストップを与える（未実装）

            OnAttackedCallbacks.Invoke();//被攻撃時のコールバック関数を呼び出し
            if (health <= 0)
            {
                health = 0;
                dyingCallbacks.Invoke();
                OnDying(mainAttackInfo);
            }
        }
        dealtAttackCount = 0;//攻撃を全て統合したのでカウンターを0にし、与えられた攻撃を忘却する

    }

    public void RecoverHealth(float amount)
    {
        health = Mathf.Clamp(health + amount, 0, maxHealth);
    }

    public void DestroySelf(float time) { Destroy(gameObject, time); }
}


