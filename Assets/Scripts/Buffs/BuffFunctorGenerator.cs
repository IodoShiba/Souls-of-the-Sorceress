using UnityEngine;

namespace Buffs
{
    public abstract class BuffFunctorGenerator : ScriptableObject
    {
        public abstract BuffTypeID buffTypeID { get; }

        public abstract BuffFunctor GetDestinationInstance(Actor owner);

        public void SetFunctor(out BuffFunctor destinationValuable, Actor destinationOwner)
        {
            var set = GetDestinationInstance(destinationOwner);
            destinationValuable = set;
        }
    }
}
