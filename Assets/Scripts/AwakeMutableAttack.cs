using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerStates.Awakening;

public class AwakeMutableAttack : Attack
{
    [Space(16)]
    [SerializeField] Parameters ordinaryAttack;
    [SerializeField] Parameters awakenAttack;
    [SerializeField] Parameters blueAwakenAttack;
    
    Ordinary ordinary;
    Awaken awaken;
    BlueAwaken blueAwaken;
    bool ready = false;

    private void Start()
    {
        GameObject player = owner.gameObject;
        ordinary = player.GetComponent<Ordinary>();
        awaken = player.GetComponent<Awaken>();
        blueAwaken = player.GetComponent<BlueAwaken>();
        ordinary.RegisterInitialize(() => { paramsRaw = ordinaryAttack; });
        awaken.RegisterInitialize(() => { paramsRaw = awakenAttack; });
        blueAwaken.RegisterInitialize(() => { paramsRaw = blueAwakenAttack; });
        AdjustAwake();
    }

    public void AdjustAwake()
    {
        if(ordinary.IsCurrent) { paramsRaw = ordinaryAttack; }
        else if (awaken.IsCurrent) { paramsRaw = awakenAttack; }
        else if (blueAwaken.IsCurrent) { paramsRaw = blueAwakenAttack; }
        
    }
}
