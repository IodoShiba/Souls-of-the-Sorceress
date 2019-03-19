using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterAttack : MonoBehaviour
{
    [SerializeField] AttackInHitbox attack;
    private List<Mortal> subjects = new List<Mortal>();

    public void AddSubjects(Mortal subject)
    {
        subjects.Add(subject);
    }

    public void Attack()
    {
        foreach(var s in subjects)
        {
            //s._OnAttackedInternal(gameObject, attack.ParamsConvertedByOwner);
            s.SetParams(gameObject, attack.ParamsConvertedByOwner, null);
            s.SendSignal();
        }
    }

    public void Attack(Mortal subject)
    {
        //subject._OnAttackedInternal(gameObject, attack.ParamsConvertedByOwner);
        subject.SetParams(gameObject, attack.ParamsConvertedByOwner, null);
        subject.SendSignal();
    }
}
