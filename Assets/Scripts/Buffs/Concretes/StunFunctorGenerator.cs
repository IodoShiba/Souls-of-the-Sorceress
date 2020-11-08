using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Buffs
{
    [CreateAssetMenu(menuName = "Buffs/Concretes/StunFunctorGenerator")]
    public class StunFunctorGenerator : BuffFunctorGenerator
    {
        public override BuffTypeID buffTypeID => BuffTypeID.Stun;

        public override BuffFunctor GetDestinationInstance(Actor owner)
        {
            var stunf = new StunFunctor(owner);
            return stunf;
        }
    }
}