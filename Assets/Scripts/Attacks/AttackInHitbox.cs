﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class AttackInHitbox : MonoBehaviour
{
    //[SerializeField] private Mortal owner;
    [SerializeField,UnityEngine.Serialization.FormerlySerializedAs("_oowner")] private Mortal owner;
    [SerializeField] private bool onceOnly;
    [SerializeField] private bool initiallyActivate;
    [SerializeField] private bool allowToAttackSelf;
    [SerializeField, TagField] string targetTag;
    [SerializeField] float activeSpan;
    [SerializeField] private AttackData attackDataPrototype;
    [SerializeField,FormerlySerializedAs("dealingAttackConverters")] private List<AttackConverter> attackConvertersOnActivate;
    [SerializeField] private List<AttackConverter> attackConvertersOnHit;
    bool isAttackActive = false;

    public float Damage { get { return convertedAttackData.damage; } }
    public Vector2 KnockBackImpact { get { return convertedAttackData.knockBackImpulse; } }
    public Collider2D AttackCollider { get { return convertedAttackData.attackCollider; } }
    public bool Throughable { get { return convertedAttackData.throughable; } }

    public AttackData ParamsConvertedByOwner { get => new AttackData(convertedAttackData); }//不自然
    //public AttackDealer Owner { get => owner;}
    public Mortal Owner { get => owner; }
    public bool IsAttackActive { get => isAttackActive; }

    private float realActiveSpan;
    
    private AttackData convertedAttackData;


    public void AddConverter(AttackConverter item) { attackConvertersOnActivate.Add(item); }
    public void RemoveConverter(AttackConverter item) { attackConvertersOnActivate.Remove(item); }

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
            if(!allowToAttackSelf && owner == mortal)
            {
                return;
            }

            attackConvertersOnHit.ForEach(acOnHit => acOnHit.Convert(convertedAttackData));
            //mortal.TryAttack(gameObject, this.ParamsConvertedByOwner, result => { HitProcess(); } );
            mortal.TryAttack(owner, this.ParamsConvertedByOwner, transform.position - hit.gameObject.transform.position);
            HitProcess();
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
        
        attackConvertersOnActivate.ForEach(ac => { ac.Convert(convertedAttackData); });
        if(owner!=null) owner.ConvertDealingAttack(convertedAttackData);

        SetDependentsEnable(true);

        if (!(activeSpan<=0||float.IsInfinity(activeSpan)))
        {
            StartCoroutine(Clock());
        }

        isAttackActive = true;
    }

    public void Inactivate()
    {
        SetDependentsEnable(false);
        isAttackActive = false;
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

    public static AttackInHitbox InstantiateThis(AttackInHitbox original, Vector3 position, Quaternion rotation, Mortal owner)
    {
        AttackInHitbox ins = Instantiate(original, position, rotation);
        ins.owner = owner;
        return ins;
    }
}

[System.Serializable]
public class AttackData
{
    public float damage;
    [UnityEngine.Serialization.FormerlySerializedAs("knockBackImpact")] public Vector2 knockBackImpulse;
    public Collider2D attackCollider;
    public bool throughable;
    public float hitstopSpan;

    public AttackData() { damage = 0; knockBackImpulse = Vector2.zero; attackCollider = null; throughable = false; hitstopSpan = 0; }

    public AttackData(float damage, Vector2 knockBackImpact, Collider2D attackCollider = null, bool throughable = false, float hitstopSpan = 0)
    {
        this.damage = damage;
        this.knockBackImpulse = knockBackImpact;
        this.attackCollider = attackCollider;
        this.throughable = throughable;
        this.hitstopSpan = hitstopSpan;
    }

    //コピーコンストラクタ
    public AttackData(AttackData original)
    {
        this.damage = original.damage;
        this.knockBackImpulse = original.knockBackImpulse;
        this.attackCollider = original.attackCollider;
        this.throughable = original.throughable;
        this.hitstopSpan = original.hitstopSpan;
    }

    public static AttackData Copy(AttackData target,AttackData original)
    {
        target.damage = original.damage;
        target.knockBackImpulse = original.knockBackImpulse;
        target.attackCollider = original.attackCollider;
        target.throughable = original.throughable;
        target.hitstopSpan = original.hitstopSpan;
        return target;
    }
}

public abstract class AttackConverter : MonoBehaviour
{
    public abstract bool Convert(AttackData value);
    public interface UsingPhaseSpecifier { }
    public interface OnAttackActivate : UsingPhaseSpecifier { }
    public interface OnGiveAttack : UsingPhaseSpecifier { }
    public interface OnReceiveAttack : UsingPhaseSpecifier { }
}


//AttackConverterの使用タイミングを型レベルで指定するよう変更するときに備えたクラス
public abstract class AttackConverter<UsingPhase> where UsingPhase : AttackConverter.UsingPhaseSpecifier
{

}

#if UNITY_EDITOR
[CustomEditor(typeof(AttackConverter)),CanEditMultipleObjects]
public class AttackConverterEditorView : UnityEditor.Editor
{
    AttackConverter _target;
    private void OnEnable()
    {
       _target = (AttackConverter)target;
    }

    public override void OnInspectorGUI()
    {

        base.OnInspectorGUI();
    }
}
#endif