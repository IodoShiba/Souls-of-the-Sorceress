using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D)),DisallowMultipleComponent]
public class Mortal : MonoBehaviour,IodoShiba.Utilities.IManualUpdate
{
    public class DealtAttackInfo
    {
        public Mortal attacker;
        public AttackData attackData;

        public DealtAttackInfo(Mortal attacker, AttackData attackData)
        {
            this.attacker = attacker;
            this.attackData = attackData;
        }
    }

    [SerializeField] protected float health;
    [SerializeField] protected float maxHealth;
    [SerializeField] private  bool isInvulnerable;
    [SerializeField] UnityEngine.Events.UnityEvent dyingCallbacks;
    [SerializeField] Rigidbody2D selfRigidbody;
    [SerializeField] List<AttackConverter> dealingAttackConverters;
    [SerializeField] List<AttackConverter> dealtAttackConverters;
    [SerializeField, UnityEngine.Serialization.FormerlySerializedAs("leftStunTime")] float initialStunTime = 0.3f;

    AttackData argAttackData = new AttackData();
    GameObject argObj;
    System.Action<bool> argSucceedCallback;
    float leftStunTime = 0.3f;
    int dealtAttackCount = 0;
    List<DealtAttackInfo> dealtAttackInfos = new List<DealtAttackInfo>(4);
    Actor actor;

    public bool IsInvulnerable { get => isInvulnerable; set => isInvulnerable = value; }
    public Actor Actor { get => actor == null ? (actor = GetComponent<Actor>()) : actor; }

    private void Awake()
    {
        Actor.MortalUpdate = ManualUpdate;
    }
    protected virtual void OnAttacked(GameObject attackObj,AttackData attack) { }
    protected virtual bool _IsInvulnerable() { return isInvulnerable; }

    protected virtual void OnTriedAttack(in Mortal attacker, AttackData dealt) { }

    public virtual void Dying() { Destroy(gameObject); }

    public void ConvertDealingAttack(AttackData dealee)
    {
        dealingAttackConverters.ForEach(dac => dac.Convert(dealee));
    }
    protected void ConvertDealtAttack(AttackData dealt)
    {
        //dealtAttackConverters.TrueForAll(dac => dac.Convert(dealt));
        dealtAttackConverters.ForEach(dac => dac.Convert(dealt));
    }
    

    private void _OnAttackedInternal(GameObject attackerObj, AttackData givenData)
    {
        if (!_IsInvulnerable())
        {
            OnAttacked(attackerObj, givenData);
            int kbdir = System.Math.Sign(transform.position.x - attackerObj.transform.position.x);
            givenData.knockBackImpact.x *= kbdir;

            if (!dealtAttackConverters.TrueForAll(dac => dac.Convert(givenData)))
            {
                if (argSucceedCallback != null) argSucceedCallback(false);
            }

            health -= givenData.damage;

            leftStunTime = initialStunTime;

            selfRigidbody.velocity = Vector2.zero;
            selfRigidbody.AddForce(givenData.knockBackImpact);

            Debug.Log(gameObject.name + " damaged");

            if (health <= 0)
            {
                dyingCallbacks.Invoke();
                Dying();
            }

            if (argSucceedCallback != null)
            {
                argSucceedCallback(true);
            }
        }
        else if (argSucceedCallback != null)
        {
            argSucceedCallback(false);
        }
            
        
    }

    public void TryAttack(GameObject argObj, AttackData argAttackData, System.Action<bool> succeedCallback)
    {
        this.argObj = argObj;
        AttackData.Copy(this.argAttackData, argAttackData);
        this.argSucceedCallback = succeedCallback;
    }
    public void TryAttack(Mortal attacker, AttackData argAttackData)
    {
        OnTriedAttack(attacker, argAttackData);

        if (isInvulnerable) { return; }

        if (dealtAttackCount >= dealtAttackInfos.Count)
        {
            dealtAttackInfos.Add(new DealtAttackInfo(attacker, new AttackData(argAttackData)));
        }
        else if (dealtAttackInfos[dealtAttackCount] == null)
        {
            dealtAttackInfos[dealtAttackCount] = new DealtAttackInfo(attacker, new AttackData(argAttackData));
        }
        else
        {
            dealtAttackInfos[dealtAttackCount].attacker = attacker;
            AttackData.Copy(dealtAttackInfos[dealtAttackCount].attackData, argAttackData);
        }
        dealtAttackCount++;
    }

    public void ManualUpdate()
    {
        AttackData result = new AttackData();

        for (int i = 0; i < dealtAttackCount; ++i)
        {
            dealtAttackConverters.ForEach(dac => dac.Convert(dealtAttackInfos[i].attackData));
            result.damage += dealtAttackInfos[i].attackData.damage;
            result.knockBackImpact += dealtAttackInfos[i].attackData.knockBackImpact;
            if(result.hitstopSpan < dealtAttackInfos[i].attackData.hitstopSpan)
            {
                result.hitstopSpan = dealtAttackInfos[i].attackData.hitstopSpan;
            }
        }
        dealtAttackCount = 0;
        
    }
}


//[RequireComponent(typeof(Rigidbody2D)), DisallowMultipleComponent]
//public class Mortal : PassiveBehaviour, ActorBehaviour.IParamableWith<GameObject, AttackData, System.Action<bool>>
//{
//    [SerializeField] protected float health;
//    [SerializeField] protected float maxHealth;
//    [SerializeField] UnityEngine.Events.UnityEvent dyingCallbacks;
//    [SerializeField] Rigidbody2D selfRigidbody;
//    [SerializeField] List<AttackConverter> dealingAttackConverters;
//    [SerializeField] List<AttackConverter> dealtAttackConverters;

//    AttackData argAttackData = new AttackData();
//    GameObject argObj;
//    System.Action<bool> argSucceedCallback;
//    float leftStunTime = 0.3f;
//    [SerializeField, UnityEngine.Serialization.FormerlySerializedAs("leftStunTime")] float initialStunTime = 0.3f;

//    protected virtual void OnAttacked(GameObject attackObj, AttackData attack) { }
//    protected virtual bool IsInvulnerable() { return false; }
//    public virtual void Dying() { Destroy(gameObject); }

//    public void ConvertDealingAttack(AttackData dealee)
//    {
//        dealingAttackConverters.ForEach(dac => dac.Convert(dealee));
//    }
//    protected void ConvertDealtAttack(AttackData dealt)
//    {
//        dealtAttackConverters.TrueForAll(dac => dac.Convert(dealt));
//    }

//    protected override bool ShouldContinue(bool ordered)
//    {
//        leftStunTime -= Time.deltaTime;
//        if (leftStunTime < 0)
//        {
//            leftStunTime = 0;
//            return false;
//        }
//        return true;
//    }

//    protected override void OnInitialize()
//    {
//        _OnAttackedInternal(argObj, argAttackData);
//    }

//    private void _OnAttackedInternal(GameObject attackerObj, AttackData givenData)
//    {
//        if (!IsInvulnerable())
//        {
//            OnAttacked(attackerObj, givenData);
//            int kbdir = System.Math.Sign(transform.position.x - attackerObj.transform.position.x);
//            givenData.knockBackImpact.x *= kbdir;
//            //ConvertDealtAttack(data);
//            if (!dealtAttackConverters.TrueForAll(dac => dac.Convert(givenData)))
//            {
//                if (argSucceedCallback != null) argSucceedCallback(false);
//            }

//            health -= givenData.damage;

//            leftStunTime = initialStunTime;

//            selfRigidbody.velocity = Vector2.zero;
//            selfRigidbody.AddForce(givenData.knockBackImpact);

//            Debug.Log(gameObject.name + " damaged");

//            if (health <= 0)
//            {
//                dyingCallbacks.Invoke();
//                Dying();
//            }

//            if (argSucceedCallback != null)
//            {
//                argSucceedCallback(true);
//            }
//        }
//        else if (argSucceedCallback != null)
//        {
//            argSucceedCallback(false);
//        }


//    }

//    public void SetParams(GameObject argObj, AttackData argAttackData, System.Action<bool> succeedCallback)
//    {
//        this.argObj = argObj;
//        AttackData.DeepCopy(this.argAttackData, argAttackData);
//        this.argSucceedCallback = succeedCallback;
//        if (Activated) { _OnAttackedInternal(argObj, argAttackData); }
//    }
//}
