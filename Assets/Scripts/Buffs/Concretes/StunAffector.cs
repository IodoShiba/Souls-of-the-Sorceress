using UnityEngine;

namespace Buffs
{
    [CreateAssetMenu(menuName = "Buffs/Affectors/StunAffector")]
    public class StunAffector : BuffAffectorScriptable
    {
        [SerializeField] float time;

        public override BuffTypeID buffTypeID => BuffTypeID.Stun;

        public override void Affect(BuffFunctor functor)
        {
            ((StunFunctor)functor).SwitchActivate(time);
        }
    }
}