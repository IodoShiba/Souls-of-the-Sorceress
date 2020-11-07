using UnityEngine;

namespace Buffs
{
    public abstract class BuffAffectorScriptable : ScriptableObject, IBuffAffector
    {
        public abstract BuffTypeID buffTypeID { get; }

        public abstract void Affect(BuffDestination destination);
    }

}