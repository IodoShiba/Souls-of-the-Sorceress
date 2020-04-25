using UnityEngine;
using System.Collections;

namespace InterrupterStates
{
    public static class StunStateManager
    {
        const int arraySize = 4;

        static StunState[] stunStates;
        static int next = 0;

        static StunStateManager()
        {
            stunStates = new StunState[arraySize];
            for(int i=0;i < arraySize; ++i)
            {
                stunStates[i] = new StunState();
            }
        }

        public static StunState GetStunState(float stunTime)
        {
            StunState ret = stunStates[next];
            next = (next + 1) % arraySize;
            ret.Reset(stunTime);
            return ret;
        }

        public class StunState : ActorState
        {
            float t = 0;
            float timeSpan;
            bool canceled = false;
            bool horizontalMoveUsed = false;
            ActorFunction.HorizontalMoveMethod horizontalMove;

            public float TimeSpan { get => timeSpan; private set => timeSpan = value; }

            public void Reset(float timeSpan)
            {
                TimeSpan = timeSpan;
                t = 0;
                canceled = false;
            }

            public void Cancel()
            {
                canceled = true;
            }

            public void Extend(float time)
            {
                t -= time;
            }

            protected override bool ShouldCotinue()
            {
                return !canceled && t < timeSpan;
            }

            protected override void OnInitialize()
            {
                t = 0;
                horizontalMove = GameObject.GetComponent<ActorFunction.HorizontalMoveMethod>();
                if(horizontalMove != null)
                {
                    horizontalMoveUsed = horizontalMove.Use;
                    horizontalMove.Use = false;
                }
            }

            protected override void OnTerminate(bool isNormal)
            {
                if(horizontalMove != null)
                {
                    horizontalMove.Use = horizontalMoveUsed;
                }
            }

            protected override void OnActive()
            {
                t += Time.deltaTime;
            }
        }
    }
}
