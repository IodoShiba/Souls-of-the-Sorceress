using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ActorBehaviour;

public class WalkingMashBehaviours : ActorBehavioursManager
{
    protected override void Structure()
    {
        Allow<HorizontalMove>();
    }
}
