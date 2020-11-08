using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Buffs
{
    class StunFunctor : BuffFunctor
    {
        public override BuffTypeID buffTypeID => BuffTypeID.Stun;

        public override bool IsActive => t > 0;

        float t = 0;

        Actor owner;

        public StunFunctor(Actor owner)
        {
            this.owner = owner;
        }

        public override void Reset()
        {
            t = 0;
        }

        protected override void UpdateSpecify()
        {
            t = Mathf.Max(t - Time.deltaTime, 0);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
        }

        protected override void OnInactivate()
        {
            base.OnInactivate();
        }

        public void Extend(float sec)
        {
            t += sec;
        }
    }
}