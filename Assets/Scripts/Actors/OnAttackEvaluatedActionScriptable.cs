using UnityEngine;

public abstract class OnAttackEvaluatedActionScriptable : ScriptableObject, IOnAttackEvaluatedAction
{
    public abstract void OnAttackEvaluated(bool isSuccess, Mortal subjectMortal, AttackData finallyGiven);
    
}
