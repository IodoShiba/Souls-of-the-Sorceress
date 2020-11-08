using UnityEngine;

namespace Buffs
{
    public class StunAffector : BuffAffectorScriptable
    {
        [SerializeField] float time;

        public override BuffTypeID buffTypeID => BuffTypeID.Stun;

        public override void Affect(BuffFunctor functor)
        {
            ((StunFunctor)functor).Extend(time);
        }
    }
}