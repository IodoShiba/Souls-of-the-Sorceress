using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Mortal : MonoBehaviour{
    [SerializeField] protected float health;
    [SerializeField] protected float maxHealth;

    protected abstract void OnAttacked(GameObject attackObj,Attack attack);
    protected abstract bool IsInvulnerable();
    public abstract void Dying();

    protected virtual float ConvertDealtDamage(float given) {
        return given;
    }
    protected virtual Vector2 ConvertDealtKnockBack(Vector2 given)
    {
        return given;
    }
    public virtual void ConvertDealingAttack(ref Attack.Parameters attackData)
    {
    }

    public void _OnAttackedInternal(GameObject attackObj,Attack data)
    {
        if (!IsInvulnerable())
        {
            OnAttacked(attackObj, data);
            health -= ConvertDealtDamage(data.ParamsConvertedByOwner.damage);

            Vector2 kb = data.ParamsConvertedByOwner.knockBackImpact;
            int kbdir = System.Math.Sign(transform.position.x - attackObj.transform.position.x);
            var rb = GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.zero;
            rb.AddForce(ConvertDealtKnockBack(new Vector2(kbdir * kb.x, kb.y)));

            Debug.Log(gameObject.name + " damaged");

            if (health <= 0)
            {
                Dying();
            }
        }
    }
}
