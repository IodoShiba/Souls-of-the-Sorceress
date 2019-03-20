using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class CounterAttack : AttackConverter
{
    public enum ConvertMode
    {
        overwrite,
        multiplyByConstants,
        copy,
    }

    [SerializeField] AttackData baseAttack;
    [SerializeField] AttackDealer owner;
    [SerializeField] ConvertMode convertMode;
    [SerializeField] List<AttackConverter> dealingAttackConverters;
    private bool isActive = false;
    private List<Mortal> subjects = new List<Mortal>();
    private AttackData capturedAndSend;

    public bool IsActive {
        get => isActive;
        set
        {
            isActive = value;
            if(!value)
            {
                subjects.Clear();
            }
        }
    }

    public void AddSubjects(Mortal subject)
    {
        subjects.Add(subject);
    }

    public void Attack()
    {
        if (!isActive) return;

        foreach(var s in subjects)
        {
            //s._OnAttackedInternal(gameObject, attack.ParamsConvertedByOwner);
            
            s.SetParams(gameObject, capturedAndSend, null);
            s.SendSignal();
        }
    }

    public void Attack(Mortal subject)
    {
        if (!isActive) return;
        //subject._OnAttackedInternal(gameObject, attack.ParamsConvertedByOwner);
        subject.SetParams(gameObject, capturedAndSend, null);
        subject.SendSignal();
    }

    public override bool Convert(AttackData value)
    {
        if (!isActive) { return true; }
        
        //captured = value;
        switch (convertMode)
        {
            case ConvertMode.overwrite:
                AttackData.DeepCopy(capturedAndSend, baseAttack);
                break;

            case ConvertMode.multiplyByConstants:
                AttackData.DeepCopy(capturedAndSend, value);
                capturedAndSend.damage *= baseAttack.damage;
                capturedAndSend.knockBackImpact.x *= capturedAndSend.knockBackImpact.x;
                capturedAndSend.knockBackImpact.y *= capturedAndSend.knockBackImpact.y;
                break;

            case ConvertMode.copy:
                AttackData.DeepCopy(capturedAndSend, value);
                break;
        }
        dealingAttackConverters.ForEach(dac => dac.Convert(capturedAndSend));
        owner.ConvertDealingAttack(capturedAndSend);
        return false;
        
    }
}
