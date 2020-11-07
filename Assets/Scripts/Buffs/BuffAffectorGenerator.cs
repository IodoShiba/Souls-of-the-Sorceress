using UnityEngine;

namespace Buffs
{
    public abstract class BuffAffectorGenerator : ScriptableObject
    {
        public abstract int buffTypeID { get; }

        // TODO: 2019.4に移行後、SerializeReferenceなIBuffAffectorを返すメソッドを定義する
        public abstract IBuffAffector GetAffector();
    }
}
