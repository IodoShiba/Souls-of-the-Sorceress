using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementBehaviours : ActorBehaviours
{
    protected override void Structure()
    {
        Allow<HorizontalMove>();
        Allow<ShootBullet>();
    }
}
