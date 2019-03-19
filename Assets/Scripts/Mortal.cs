using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Mortal : PassiveBehaviour, ActorBehaviour.IParamableWith<GameObject, AttackData,System.Action<bool>>
{
    [SerializeField] protected float health;
    [SerializeField] protected float maxHealth;
    [SerializeField] UnityEngine.Events.UnityEvent dyingCallbacks;
    [SerializeField] Rigidbody2D selfRigidbody;
    [SerializeField] AttackData argAttackData = new AttackData();
    [SerializeField] GameObject argObj;
    float leftStunTime = 0;
    System.Action<bool> succeedCallback;

    protected abstract void OnAttacked(GameObject attackObj,AttackData attack);
    protected abstract bool IsInvulnerable();
    public abstract void Dying();
    
    public virtual void ConvertDealingAttack(AttackData attackData)
    {
    }
    protected virtual void ConvertDealtAttack(AttackData dealt)
    {

    }
    
    protected override bool CanContinue(bool ordered)
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

    private void _OnAttackedInternal(GameObject attackerObj, AttackData data)
    {
        if (!IsInvulnerable())
        {
            OnAttacked(attackerObj, data);
            int kbdir = System.Math.Sign(transform.position.x - attackerObj.transform.position.x);
            data.knockBackImpact.x *= kbdir;
            ConvertDealtAttack(data);

            health -= data.damage;

            //フィールドに昇格する　要修正
            leftStunTime = 0.3f;

            selfRigidbody.velocity = Vector2.zero;
            selfRigidbody.AddForce(data.knockBackImpact);

            Debug.Log(gameObject.name + " damaged");

            if (health <= 0)
            {
                dyingCallbacks.Invoke();
                Dying();
            }
            if (succeedCallback != null)
            {
                succeedCallback(true);
            }
        }
        else
        {
            if (succeedCallback != null)
            {
                succeedCallback(false);
            }
        }
    }

    public void SetParams(GameObject argObj, AttackData argAttackData, System.Action<bool> succeedCallback)
    {
        this.argObj = argObj;
        this.argAttackData = argAttackData;
        this.succeedCallback = succeedCallback;
    }
}
