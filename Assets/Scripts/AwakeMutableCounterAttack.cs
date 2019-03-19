using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwakeMutableCounterAttack : MonoBehaviour
{
    [SerializeField] AwakeMutableAttack attack;
    private List<Mortal> subjects = new List<Mortal>();

    public void AddSubjects(Mortal subject)
    {
        subjects.Add(subject);
    }

    public void Attack()
    {
        attack.AdjustAwake();
        foreach (var s in subjects)
        {
            //s._OnAttackedInternal(gameObject, attack.ParamsConvertedByOwner);
            s.SetParams(gameObject, attack.ParamsConvertedByOwner, f => { });
            s.SendSignal();
        }
    }

    public void Attack(Mortal subject)
    {
        attack.AdjustAwake();
        attack.Activate();
        //subject._OnAttackedInternal(gameObject, attack.ParamsConvertedByOwner);
        subject.SetParams(gameObject, attack.ParamsConvertedByOwner, f => { });
        subject.SendSignal();
        attack.Inactivate();
    }
}
