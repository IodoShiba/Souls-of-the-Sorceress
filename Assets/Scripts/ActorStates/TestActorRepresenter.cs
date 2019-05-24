using System.Collections.Generic;
using UnityEngine;
using static ActorState;

public class TestActorRepresenter : ActorStateConnector
{
    [SerializeField] Skill1 skill1;
    [SerializeField] Skill2 skill2;
    [SerializeField] Default idle;

    public override ActorState DefaultState => idle;

    protected override List<ActorState> ConstructActorState()
        => new List<ActorState> { new Skill1(), new Skill2() };
    protected override void BuildStateConnection()
    {
        ConnectAsSkill(() => Input.GetKeyDown(KeyCode.Q), skill1);
        ConnectAsSkill(() => Input.GetKeyDown(KeyCode.W), skill2);
    }
}

