using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AttackImmuneTime : MonoBehaviour, Mortal.IOnTriedAttackCallbackReceiver
{
    struct Entry 
    {
        public Mortal keyAttacker;
        public float timeExpire;

        public Entry(Mortal keyAttacker = null, float timeExpire = 0)
        {
            this.keyAttacker = keyAttacker;
            this.timeExpire = timeExpire;
        }


        public bool IsEmptyOrExpired() => keyAttacker == null || Time.time >= timeExpire;
        
    }

    [System.Serializable]
    class UnityEvent_Mortal_AttackData : UnityEvent<Mortal, AttackData> {}

    [SerializeField] int entryCount = 32;
    [SerializeField] UnityEvent_Mortal_AttackData onAttackNegated;

    Entry[] entries;
    int first = 0;
    int last = 0;

    bool BoolXor(bool a, bool b) => a? !b : b;

    void Awake()
    {
        entries = new Entry[entryCount];
    }

    public void OnTriedAttack(Mortal attacker, AttackData dealt, in Vector2 relativePosition)
    {
        if (IsRecorded(attacker))
        {
            dealt.damage = 0;
            dealt.knockBackImpulse = Vector2.zero;

            onAttackNegated.Invoke(attacker, dealt);
        }
    }

    public bool IsRecorded(Mortal mortal)
    {
        return Find(mortal) >= 0;
    }

    int Find(Mortal mortal)
    {
        int idx = first;
        while(idx != last)
        {
            if(entries[idx].IsEmptyOrExpired())
            {
                first = Next(first);
            }
            else if(entries[idx].keyAttacker == mortal)
            {
                return idx;
            }

            idx = Next(idx);
        }

        return -1;
    }

    public int Add(Mortal mortal, float timeExpire)
    {
        if(!entries[last].IsEmptyOrExpired())
        {
            return -1;
        }

        while(first != last)
        {
            if(!entries[first].IsEmptyOrExpired())
            {
                break;
            }
            first = Next(first);
        }

        entries[last].keyAttacker = mortal;
        entries[last].timeExpire = timeExpire;

        int added = last;
        last = Next(last);
        return added;
    }

    int Next(int idx)
    {
        return (idx+1)%entries.Length;
    }
}
