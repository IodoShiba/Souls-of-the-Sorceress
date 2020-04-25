using UnityEngine;
using System.Collections;

public class StunDealer : MonoBehaviour
{
    [SerializeField] float stunTime;

    public void DealStunState(ActorState.ActorStateConnector subjectAsc, float stunTime)
    {
        var s = InterrupterStates.StunStateManager.GetStunState(stunTime);
        subjectAsc.InterruptWith(s);
    }

    public void DealStunState(ActorState.ActorStateConnector subjectAsc)
    {
        DealStunState(subjectAsc, stunTime);
    }

    public void DealStunState(Mortal mortal)
    {
        DealStunState(mortal.GetComponent<ActorState.ActorStateConnector>());
    }
}
