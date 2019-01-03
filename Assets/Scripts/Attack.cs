using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Attack : MonoBehaviour {

    [System.Serializable]
    public struct Parameters
    {
        public float damage;
        public Vector2 knockBackImpact;
        public Collider2D attackCollider;
        public bool throughable;

        public Parameters(float damage, Vector2 knockBackImpact, Collider2D attackCollider, bool throughable) { this.damage = damage;this.knockBackImpact = knockBackImpact; this.attackCollider = attackCollider;this.throughable = throughable; }
        public Parameters(Parameters original) { this.damage = original.damage; this.knockBackImpact = original.knockBackImpact; this.attackCollider = original.attackCollider; this.throughable = original.throughable; }
        
    }
    [SerializeField] protected Mortal owner;
    [SerializeField] protected float damage;
    [SerializeField] protected Vector2 knockBackImpact;
    [SerializeField] protected Collider2D attackCollider;
    [SerializeField] protected bool throughable;
    [SerializeField] protected bool onceOnly;
    [SerializeField] protected bool initiallyActivate;
    
    public float Damage { get { return damage; } }
    public Vector2 KnockBackImpact { get { return knockBackImpact; } }
    public Collider2D AttackCollider { get { return attackCollider; } }
    public bool Throughable { get { return throughable; } }

    public Parameters ParamsConvertedByOwner { get => paramsConvertedByOwner; }
    public Mortal Owner { get => owner;}
    
    [SerializeField] string targetTag;
    [UnityEngine.Serialization.FormerlySerializedAs("lifespan")]
    [SerializeField] float activeSpan;
    private float realActiveSpan;
    
    [SerializeField]
    protected Parameters paramsRaw;
    protected Parameters paramsConvertedByOwner;

    protected void Awake()
    {
        paramsConvertedByOwner = paramsRaw;
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
            hit.GetComponent<Mortal>()._OnAttackedInternal(gameObject,this);
            if (!this.Throughable)
            {
                Destroy(gameObject);
            }
        }
    }

    private void SetDependentsEnable(bool toggle)
    {
        if (attackCollider != null) attackCollider.enabled = toggle;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.enabled = toggle;
        }
    }

    public void Activate()
    {
        if (owner)
        {
            paramsConvertedByOwner = paramsRaw;
            owner.ConvertDealingAttack(ref paramsConvertedByOwner);

        }

        damage = paramsConvertedByOwner.damage;
        knockBackImpact = paramsConvertedByOwner.knockBackImpact;
        attackCollider = paramsConvertedByOwner.attackCollider;
        throughable = paramsConvertedByOwner.throughable;

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
