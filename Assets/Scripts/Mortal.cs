﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Mortal : MonoBehaviour{
    [SerializeField] protected float health;
    [SerializeField] protected float maxHealth;
    [SerializeField] UnityEngine.Events.UnityEvent dyingCallbacks;
    [SerializeField] Rigidbody2D selfRigidbody;

    protected abstract void OnAttacked(GameObject attackObj,AttackInHitbox.AttackData attack);
    protected abstract bool IsInvulnerable();
    public abstract void Dying();
    
    public virtual void ConvertDealingAttack(AttackInHitbox.AttackData attackData)
    {
    }
    protected virtual void ConvertDealtAttack(AttackInHitbox.AttackData dealt)
    {

    }

    public void _OnAttackedInternal(GameObject attackObj,AttackInHitbox.AttackData data)
    {
        if (!IsInvulnerable())
        {
            OnAttacked(attackObj, data);
            int kbdir = System.Math.Sign(transform.position.x - attackObj.transform.position.x);
            data.knockBackImpact.x *= kbdir;
            ConvertDealtAttack(data);

            health -= data.damage;
            
            selfRigidbody.velocity = Vector2.zero;
            selfRigidbody.AddForce(data.knockBackImpact);

            Debug.Log(gameObject.name + " damaged");

            if (health <= 0)
            {
                dyingCallbacks.Invoke();
                Dying();
            }
        }
    }
}
