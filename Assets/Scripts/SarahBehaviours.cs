using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SarahBehaviours : ActorBehaviours
{
    [SerializeField] float maxIntervalOfTripleAttack;
    [SerializeField] GroundSensor groundSensor;

    protected override void Structure()
    {
        Allow<HorizontalMove>();

        FollowCondition triples = null;
        using (IfScope(() => groundSensor.IsOnGround))//new Condition(this, () => groundSensor.IsOnGround))
        {
            Allow<Jump>();
            triples = Allow<VerticalSlash>().Follow<ReturnSlash>(maxIntervalOfTripleAttack);
        }
        FollowCondition smashSlashWaitingCond = triples.Follow<SmashSlash>(maxIntervalOfTripleAttack);
        using (ElseScope())
        {
            using (Deny(smashSlashWaitingCond))
            {
                Allow<AerialSlash>();
            }
        }
    }
}


