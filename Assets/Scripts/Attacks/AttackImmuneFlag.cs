using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AttackImmuneFlag : MonoBehaviour, Mortal.IOnTriedAttackCallbackReceiver
{
    [System.Serializable]
    class UnityEvent_Mortal_AttackData : UnityEvent<Mortal, AttackData> {}

    [SerializeField] bool invert;
    [SerializeField, FlagField(typeof(AttackData.AttrFlags))] AttackData.AttrFlags immuneFlags;
    [SerializeField] UnityEvent_Mortal_AttackData onAttackNegated;

    bool BoolXor(bool a, bool b) => a? !b : b;

    public void OnTriedAttack(Mortal attacker, AttackData dealt, in Vector2 relativePosition)
    {
        int map_and = (int)immuneFlags & (int)dealt.attrFlags;
        bool satisfy = map_and != 0;

        if (BoolXor(invert, satisfy))
        {
            dealt.damage = 0;
            dealt.knockBackImpulse = Vector2.zero;

            onAttackNegated.Invoke(attacker, dealt);
        }
    }
}
