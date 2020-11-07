using UnityEngine;

namespace Buffs
{

    public abstract class BuffDestination 
    {
        public abstract BuffTypeID buffTypeID { get; }
        public abstract bool IsActive { get; }
        public abstract void Update();

        /// <summary>
        /// 中断し、初期化する
        /// </summary>
        public abstract void Reset();
    }

}
