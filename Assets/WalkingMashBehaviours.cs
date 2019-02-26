using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingMashBehaviours : ActorBehaviours
{
    protected override void Structure()
    {
        Allow<HorizontalMove>();
    }
}
