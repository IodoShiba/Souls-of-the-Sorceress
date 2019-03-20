using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerStates.Awakening;
using UnityEngine.Serialization;

public class AwakeMutableAttack : AttackConverter
{
    [SerializeField,FormerlySerializedAs("owner")] Player awakenist;
    [Space(16)]
    [SerializeField] AttackData ordinaryAttack;
    [SerializeField] AttackData awakenAttack;
    [SerializeField] AttackData blueAwakenAttack;
    
    Ordinary ordinary;
    Awaken awaken;
    BlueAwaken blueAwaken;
    bool ready = false;

    private void Start()
    {
        GameObject player = awakenist.gameObject;
        ordinary = player.GetComponent<Ordinary>();
        awaken = player.GetComponent<Awaken>();
        blueAwaken = player.GetComponent<BlueAwaken>();
        //ordinary.RegisterInitialize(() => { attackDataPrototype = ordinaryAttack; });
        //awaken.RegisterInitialize(() => { attackDataPrototype = awakenAttack; });
        //blueAwaken.RegisterInitialize(() => { attackDataPrototype = blueAwakenAttack; });
        AdjustAwake();
    }

    public void AdjustAwake()
    {
        //if(ordinary.IsCurrent) { attackDataPrototype = ordinaryAttack; }
        //else if (awaken.IsCurrent) { attackDataPrototype = awakenAttack; }
        //else if (blueAwaken.IsCurrent) { attackDataPrototype = blueAwakenAttack; }
        
    }

    public override bool Convert(AttackData value)
    {
        if (ordinary.IsCurrent) {AttackData.DeepCopy(value,ordinaryAttack); }
        else if (awaken.IsCurrent) { AttackData.DeepCopy(value, awakenAttack);}
        else if (blueAwaken.IsCurrent) { AttackData.DeepCopy(value, blueAwakenAttack); }
        return true;
    }
}
