using UnityEngine;

public interface IOnAttackEvaluatedAction
{
    void OnAttackEvaluated(bool isSuccess, Mortal subjectMortal, AttackData finallyGiven);

}

public class AttackEvaluatedActionDoNothing : IOnAttackEvaluatedAction
{
    static AttackEvaluatedActionDoNothing instance;
    public static AttackEvaluatedActionDoNothing Instance { get => instance; }

    static AttackEvaluatedActionDoNothing()
    {
        instance = new AttackEvaluatedActionDoNothing();
    }

    private AttackEvaluatedActionDoNothing(){}

    public void OnAttackEvaluated(bool isSuccess, Mortal subjectMortal, AttackData finallyGiven)
    {
        // do nothing
    }

}
