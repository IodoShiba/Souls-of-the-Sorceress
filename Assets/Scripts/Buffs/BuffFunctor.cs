using UnityEngine;

namespace Buffs
{

    public abstract class BuffFunctor 
    {
        public abstract BuffTypeID buffTypeID { get; }
        public abstract bool IsActive { get; }
        bool oldIsActive = false;
        public void Update()
        {
            bool isActive = IsActive;
            if(!isActive)
            {
                if(oldIsActive){ OnInactivate(); }
            }
            else
            {
                if(!oldIsActive){ OnActivate(); }

                UpdateSpecify();
            }
            oldIsActive = isActive;
        }

        protected abstract void UpdateSpecify();
        protected virtual void OnActivate(){}
        protected virtual void OnInactivate(){}

        /// <summary>
        /// 中断し、初期化する
        /// </summary>
        public abstract void Reset();
    }

}
