using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActorStateConnectorSarah : ActorState.ActorStateConnector
{
    [SerializeField] SarahStates.SarahDefault sarahDefault;
    [SerializeField] SarahStates.VerticalSlash verticalSlash;
    [SerializeField] SarahStates.ReturnSmash returnSmash;
    [SerializeField] SarahStates.SmashSlash smashSlash;
    [SerializeField] SarahStates.AerialSlash aerialSlash;

    public override ActorState DefaultState => sarahDefault;

    protected override void BuildStateConnection()
    {
        ConnectState(() => true ? (ActorState)verticalSlash : (true ? (ActorState)returnSmash : (ActorState)smashSlash));
    }
}



namespace SarahStates
{
    [System.Serializable]
    public class SarahDefault : Default
    {

    }

    [System.Serializable]
    public class VerticalSlash : ActorState
    {

    }

    [System.Serializable]
    public class ReturnSmash : ActorState
    {

    }

    [System.Serializable]
    public class SmashSlash : ActorState
    {

    }

    [System.Serializable]
    public class AerialSlash : ActorState
    {

    }
}