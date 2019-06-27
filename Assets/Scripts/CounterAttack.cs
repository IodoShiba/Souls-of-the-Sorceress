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
            
            s.TryAttack(gameObject, capturedAndSend, null);
        }
    }

    public void Attack(Mortal subject)
    {
        if (!isActive) return;
        //subject._OnAttackedInternal(gameObject, attack.ParamsConvertedByOwner);
        subject.TryAttack(gameObject, capturedAndSend, null);
    }

    public override bool Convert(AttackData value)
    {
        if (!isActive) { return true; }
        
        //captured = value;
        switch (convertMode)
        {
            case ConvertMode.overwrite:
                AttackData.Copy(capturedAndSend, baseAttack);
                break;

            case ConvertMode.multiplyByConstants:
                AttackData.Copy(capturedAndSend, value);
                capturedAndSend.damage *= baseAttack.damage;
                capturedAndSend.knockBackImpulse.x *= capturedAndSend.knockBackImpulse.x;
                capturedAndSend.knockBackImpulse.y *= capturedAndSend.knockBackImpulse.y;
                break;

            case ConvertMode.copy:
                AttackData.Copy(capturedAndSend, value);
                break;
        }
        dealingAttackConverters.ForEach(dac => dac.Convert(capturedAndSend));
        owner.ConvertDealingAttack(capturedAndSend);
        return false;
        
    }
}
