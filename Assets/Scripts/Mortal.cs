using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D)),DisallowMultipleComponent]
public abstract class Mortal : PassiveBehaviour, ActorBehaviour.IParamableWith<GameObject, AttackData,System.Action<bool>>
{
    [SerializeField] protected float health;
    [SerializeField] protected float maxHealth;
    [SerializeField] UnityEngine.Events.UnityEvent dyingCallbacks;
    [SerializeField] Rigidbody2D selfRigidbody;
    [SerializeField] List<AttackConverter> dealtAttackConverters;

    AttackData argAttackData = new AttackData();
    GameObject argObj;
    System.Action<bool> argSucceedCallback;
    float leftStunTime = 0;

    protected abstract void OnAttacked(GameObject attackObj,AttackData attack);
    protected abstract bool IsInvulnerable();
    public abstract void Dying();
    
    //public virtual void ConvertDealingAttack(AttackData attackData)
    //{
    //}
    protected virtual void ConvertDealtAttack(AttackData dealt)
    {
        dealtAttackConverters.TrueForAll(dac => dac.Convert(dealt));
    }
    
    protected override bool ShouldContinue(bool ordered)
    {
        leftStunTime -= Time.deltaTime;
        if (leftStunTime < 0)
        {
            leftStunTime = 0;
            return false;
        }
        return true;
    }

    protected override void OnInitialize()
    {
        _OnAttackedInternal(argObj, argAttackData);
    }

    private void _OnAttackedInternal(GameObject attackerObj, AttackData givenData)
    {
        if (!IsInvulnerable())
        {
            OnAttacked(attackerObj, givenData);
            int kbdir = System.Math.Sign(transform.position.x - attackerObj.transform.position.x);
            givenData.knockBackImpact.x *= kbdir;
            //ConvertDealtAttack(data);
            if (!dealtAttackConverters.TrueForAll(dac => dac.Convert(givenData)))
            {
                if (argSucceedCallback != null) argSucceedCallback(false);
            }

            health -= givenData.damage;

            //フィールドに昇格する　要修正
            leftStunTime = 0.3f;

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

    public void SetParams(GameObject argObj, AttackData argAttackData, System.Action<bool> succeedCallback)
    {
        this.argObj = argObj;
        //this.argAttackData = argAttackData;
        AttackData.DeepCopy(this.argAttackData, argAttackData);
        this.argSucceedCallback = succeedCallback;
    }
}
