using UnityEngine;

namespace Buffs
{
    public struct BuffView
    {
        public BuffTypeID buffTypeID;
        public float param1;
        public float param2;

        public override string ToString() => $"{buffTypeID.ToString()}: ({param1},{param2})\n";
    }
}