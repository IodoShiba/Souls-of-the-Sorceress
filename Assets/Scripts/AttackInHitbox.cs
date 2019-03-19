using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class AttackInHitbox : MonoBehaviour {

    
    [SerializeField] protected Mortal owner;
    [SerializeField] protected bool onceOnly;
    [SerializeField] protected bool initiallyActivate;
    
    public float Damage { get { return convertedAttackData.damage; } }
    public Vector2 KnockBackImpact { get { return convertedAttackData.knockBackImpact; } }
    public Collider2D AttackCollider { get { return convertedAttackData.attackCollider; } }
    public bool Throughable { get { return convertedAttackData.throughable; } }

    public AttackData ParamsConvertedByOwner { get => new AttackData(convertedAttackData); }//不自然
    public Mortal Owner { get => owner;}
    
    [SerializeField,TagField] string targetTag;
    [UnityEngine.Serialization.FormerlySerializedAs("lifespan")]
    [SerializeField] float activeSpan;
    private float realActiveSpan;
    
    [SerializeField,FormerlySerializedAs("paramsRaw")]
    protected AttackData attackDataPrototype;
    protected AttackData convertedAttackData;
    

    protected void Awake()
    {
        convertedAttackData = new AttackData(attackDataPrototype);
    }

    void Start ()
    {
        if (initiallyActivate)
        {
            Activate();
        }
        else
        {
            Inactivate();
        }
	}

    private void OnTriggerEnter2D(Collider2D collider)
    {
        GameObject hit = collider.gameObject;
        if (hit.tag == targetTag)
        {
            Mortal mortal = hit.GetComponent<Mortal>();
            mortal.SetParams(gameObject, this.ParamsConvertedByOwner, result => { if (result) HitProcess(); } );
            mortal.SendSignal();
        }
    }
    public void HitProcess()
    {
        if (!this.Throughable)
        {
            if (onceOnly)
            {
                Destroy(gameObject);
            }
            else
            {
                Inactivate();
            }
        }
    }

    private void SetDependentsEnable(bool toggle)
    {
        if (convertedAttackData.attackCollider != null) convertedAttackData.attackCollider.enabled = toggle;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.enabled = toggle;
        }
    }

    public void Activate()
    {
        convertedAttackData = new AttackData(attackDataPrototype);
        if (owner)
        {
            owner.ConvertDealingAttack(convertedAttackData);
        }
        
        SetDependentsEnable(true);

        if (!float.IsInfinity(activeSpan))
        {
            StartCoroutine(Clock());
        }
    }

    public void Inactivate()
    {
        SetDependentsEnable(false);
    }

    
    IEnumerator Clock()
    {
        float t = 0;
        realActiveSpan = activeSpan;
        while (t < realActiveSpan) {
            t += Time.deltaTime;
            yield return null;
        }
        
        if (onceOnly)
        {
            Destroy(gameObject);
        }
        else
        {
            Inactivate();
        }
    }
}
[System.Serializable]
public class AttackData
{
    public float damage;
    public Vector2 knockBackImpact;
    public Collider2D attackCollider;
    public bool throughable;

    public AttackData() { damage = 0; knockBackImpact = Vector2.zero;attackCollider = null; throughable = false; }

    public AttackData(float damage, Vector2 knockBackImpact, Collider2D attackCollider, bool throughable)
    {
        this.damage = damage;
        this.knockBackImpact = knockBackImpact;
        this.attackCollider = attackCollider;
        this.throughable = throughable;
    }

    public AttackData(AttackData original)
    {
        this.damage = original.damage;
        this.knockBackImpact = original.knockBackImpact;
        this.attackCollider = original.attackCollider;
        this.throughable = original.throughable;
    }

}