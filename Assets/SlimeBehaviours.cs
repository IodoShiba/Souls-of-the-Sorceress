using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBehaviours : ActorBehaviour.ActorBehavioursManager
{
    protected override void Structure()
    {
        Allow<Spike>();
    }
}
