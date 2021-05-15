using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Buffs
{
    public class StunFunctor : BuffFunctor
    {
        public override BuffTypeID buffTypeID => BuffTypeID.Stun;

        public override bool IsActive => timeLeft > 0;

        public float TimeLeft { get => timeLeft; }

        float timeLeft = 0;

        Actor owner;

        StunState stunState;

        public StunFunctor(Actor owner)
        {
            this.owner = owner;
            stunState = new StunState(this, owner.FightAsc.Smashed);
        }

        public override void Reset()
        {
            timeLeft = 0;
        }

        protected override void UpdateSpecify()
        {
            timeLeft = Mathf.Max(timeLeft - Time.deltaTime, 0);
        }

        protected override void OnActivate()
        {
            owner.FightAsc.InterruptWith(stunState);
        }

        protected override void OnInactivate()
        {
            base.OnInactivate();
        }

        public override void GetView(out BuffView view)
        {
            base.GetView(out view);
            view.param1 = timeLeft;
        }

        public void Extend(float sec)
        {
            timeLeft += sec;
        }

        public void SwitchActivate(float sec)
        {
            timeLeft = IsActive ? 0 : sec;
        }

        public class StunState : FightActorStateConector.SmashedState
        {
            StunFunctor functor;
            FightActorStateConector.SmashedState smashed;

            public StunState(StunFunctor functor, FightActorStateConector.SmashedState smashed)
            {
                this.functor = functor;
                this.smashed = smashed;
            }

            protected override bool ShouldCotinue()
            {
                return functor.IsActive;
            }

            protected override void OnInitialize()
            {
                smashed._OnInitialize();
            }

            protected override void OnTerminate(bool isNormal)
            {
                smashed._OnTerminate(isNormal);
                functor.Reset();
            }
        }
    }
}