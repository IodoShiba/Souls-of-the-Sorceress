using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static ActorBehaviour;

public class SarahBehaviours : ActorBehavioursManager
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
            Allow<Guard>();
            Allow<Tackle>();
            triples = Allow<VerticalSlash>().Follow<ReturnSlash>(maxIntervalOfTripleAttack);
        }
        FollowCondition smashSlashWaitingCond = triples.Follow<SmashSlash>(maxIntervalOfTripleAttack);
        using (ElseScope())
        {
            Allow<Glide>();

            Allow<DropAttack>();
            using (DenyIfScope(smashSlashWaitingCond))
            {
                Allow<AerialSlash>();
            }
            Allow<RisingAttack>();
        }
    }
}


