using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Buffs
{
    [CreateAssetMenu(menuName = "Buffs/FunctorGenerators/StunFunctorGenerator")]
    public class StunFunctorGenerator : BuffFunctorGenerator
    {
        public override BuffTypeID buffTypeID => BuffTypeID.Stun;

        public override BuffFunctor GetDestinationInstance(Actor owner) => GetStunFunctor(owner);

        public static StunFunctor GetStunFunctor(Actor owner)
        {
            var stunf = new StunFunctor(owner);
            return stunf;
        }
    }
}