using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackMultiplier : AttackConverter
{
    [System.Serializable]
    public class Implement{

        [SerializeField] float damageMultiplier;
        [SerializeField] float knockbackMultiplier;

        public float DamageMultiplier { get => damageMultiplier; set => damageMultiplier = value; }
        public float KnockbackMultiplier { get => knockbackMultiplier; set => knockbackMultiplier = value; }

        public bool Convert(AttackData value)
        {
            value.damage *= DamageMultiplier;
            value.knockBackImpulse *= knockbackMultiplier;
            return true;
        }
    }
    
    [SerializeField] Implement implement;

    float DamageMultiplier {get => implement.DamageMultiplier; set => implement.DamageMultiplier = value; }
    float KnockbackMultiplier {get => implement.KnockbackMultiplier; set => implement.KnockbackMultiplier = value; }

    public override bool Convert(AttackData value) => implement.Convert(value);
}
