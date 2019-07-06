using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerStates.Awakening;
using UnityEngine.Serialization;


public class AwakeMutableAttack : AttackConverter
{
    [SerializeField] Player awakenist;
    [Space(16)]
    [SerializeField] AttackData ordinaryAttack;
    [SerializeField] AttackData awakenAttack;
    [SerializeField] AttackData blueAwakenAttack;

    ActionAwake actionAwake;
    bool ready = false;

    private void Start()
    {
        actionAwake = awakenist.gameObject.GetComponent<ActionAwake>();
    }

    public override bool Convert(AttackData value)
    {
        switch (actionAwake.AwakeLevel)
        {
            case ActionAwake.AwakeLevels.ordinary:
                AttackData.Copy(value, ordinaryAttack);
                break;
            case ActionAwake.AwakeLevels.awaken:
                AttackData.Copy(value, awakenAttack);
                break;
            case ActionAwake.AwakeLevels.blueAwaken:
                AttackData.Copy(value, blueAwakenAttack);
                break;
        }
        return true;
    }
}
