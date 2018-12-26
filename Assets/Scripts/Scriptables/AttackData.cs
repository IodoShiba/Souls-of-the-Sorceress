using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "a")]
[System.Serializable]
public class AttackData_notscriptable /*: ScriptableObject*/ {
    [SerializeField] Sprite sprite;
    [SerializeField] float damage;
    [SerializeField] Vector2 knockBackImpact;
    [SerializeField] Collider2D attackCollider;
    [SerializeField] bool throughable;

    public Sprite Sprite { get { return sprite; } }
    public float Damage { get { return damage; } }
    public Vector2 KnockBackImpact { get { return knockBackImpact; } }
    public Collider2D AttackCollider { get { return attackCollider; } }
    public bool Throughable { get { return throughable; } }
}

[CreateAssetMenu(fileName = "a")]
//[System.Serializable]
public class AttackData : ScriptableObject
{
    [SerializeField] Sprite sprite;
    [SerializeField] float damage;
    [SerializeField] Vector2 knockBackImpact;
    [SerializeField] bool throughable;

    public Sprite Sprite { get { return sprite; } }
    public float Damage { get { return damage; } }
    public Vector2 KnockBackImpact { get { return knockBackImpact; } }
    public bool Throughable { get { return throughable; } }
}
