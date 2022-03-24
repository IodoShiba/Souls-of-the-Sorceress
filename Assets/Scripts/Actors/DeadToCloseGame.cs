using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadToCloseGame : MonoBehaviour, Mortal.IDyingCallbackReceiver
{
    public void OnSelfDying(Mortal.DealtAttackInfo causeOfDeath)
    {
        GameLifeCycle.CloseGame();
    }
}
