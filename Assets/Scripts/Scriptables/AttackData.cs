using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "a")]
public class AttackData : ScriptableObject {
    [SerializeField] Sprite sprite;
    [SerializeField] float damage;
    [SerializeField] Vector2 knockBackImpact;
    [SerializeField] bool throughable;

    public Sprite Sprite { get { return sprite; } }
    public float Damage { get { return damage; } }
    public Vector2 KnockBackImpact { get { return knockBackImpact; } }
    public bool Throughable { get { return throughable; } }
}
