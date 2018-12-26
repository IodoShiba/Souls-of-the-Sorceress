using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Attack : MonoBehaviour {

    [System.Serializable]
    public class Parameters
    {
        //public Sprite sprite;
        public float damage;
        public Vector2 knockBackImpact;
        public Collider2D attackCollider;
        public bool throughable;

        public Parameters(float damage, Vector2 knockBackImpact, Collider2D attackCollider, bool throughable) { this.damage = damage;this.knockBackImpact = knockBackImpact; this.attackCollider = attackCollider;this.throughable = throughable; }
        public Parameters(Parameters original) { this.damage = original.damage; this.knockBackImpact = original.knockBackImpact; this.attackCollider = original.attackCollider; this.throughable = original.throughable; }
    }
    //[SerializeField] protected Sprite sprite;
    [SerializeField] protected Mortal owner;
    [SerializeField] protected float damage;
    [SerializeField] protected Vector2 knockBackImpact;
    [SerializeField] protected Collider2D attackCollider;
    [SerializeField] protected bool throughable;

    protected Parameters paramsRaw;
    protected Parameters paramsConvertedByOwner;
    //public Sprite Sprite { get { return sprite; } }
    public float Damage { get { return damage; } }
    public Vector2 KnockBackImpact { get { return knockBackImpact; } }
    public Collider2D AttackCollider { get { return attackCollider; } }
    public bool Throughable { get { return throughable; } }

    public Parameters ParamsConvertedByOwner { get => paramsConvertedByOwner; }

    //[SerializeField] protected AttackData data;
    //[SerializeField] protected Data data;
    //protected Collider2D attackCollider;
    [SerializeField] string targetTag;
    [SerializeField] float lifespan;

    protected void Awake()
    {
        paramsRaw = new Parameters(damage, knockBackImpact, attackCollider, throughable);
        paramsConvertedByOwner = new Parameters(paramsRaw);
    }

    void Start () {
        /*if (sprite != null)
        {
            GetComponent<SpriteRenderer>().sprite = sprite;
        }*/
        if (lifespan > 0)
        {
            Destroy(gameObject, lifespan);
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
        /*else if(hit.tag == "Ground")
        {
            //仮
            Destroy(gameObject);
        }*/
    }

    //リファクト対象
    protected virtual void OnEnable()
    {
        SetDependentsEnable(true);
        if (owner)
        {
            paramsConvertedByOwner = new Parameters(paramsRaw);
            owner.ConvertDealingAttack(paramsConvertedByOwner);
        }
    }

    protected virtual void OnDisable()
    {
        SetDependentsEnable(false);
        
    }

    private void SetDependentsEnable(bool toggle)
    {
        attackCollider.enabled = toggle;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.enabled = toggle;
        }
    }
}
