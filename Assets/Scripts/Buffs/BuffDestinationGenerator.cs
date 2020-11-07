using UnityEngine;

namespace Buffs
{
    public abstract class BuffDestinationGenerator : ScriptableObject
    {
        public abstract int buffTypeID { get; }

        public abstract BuffDestination GetDestinationInstance(Mortal owner);

        public void SetDestination(out BuffDestination destinationValuable, Mortal destinationOwner)
        {
            var set = GetDestinationInstance(destinationOwner);
            destinationValuable = set;
        }
    }
}
