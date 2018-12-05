using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Mortal : MonoBehaviour{
    [SerializeField] protected float health;

    public abstract void OnAttacked(GameObject attackObj,AttackData attack);
    public abstract bool IsInvulnerable();
    public abstract void Dying();
    
    public virtual float ConvertDamage(float given) {
        return given;
    }
    public virtual Vector2 ConvertKnockDown(Vector2 given)
    {
        return given;
    }

    public void OnAttackedInternal(GameObject attackObj,AttackData data)
    {
        if (!IsInvulnerable())
        {
            health -= ConvertDamage(data.Damage);

            Vector2 kb = data.KnockBackImpact;
            int kbdir = System.Math.Sign(transform.position.x - attackObj.transform.position.x);
            var rb = GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.zero;
            rb.AddForce(ConvertKnockDown(new Vector2(kbdir * kb.x, kb.y)));

            Debug.Log(gameObject.name + " damaged");

            if (health <= 0)
            {
                Dying();
            }

            OnAttacked(attackObj, data);
        }
    }
}
