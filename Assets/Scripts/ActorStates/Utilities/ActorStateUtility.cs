using UnityEngine;
using System.Collections;

namespace ActorStateUtility {
    public class ChainAttackStream
    {
        ActorState.ActorStateConnector owner;
        float chainWaitSpan;
        float waitTimeLimit;
        ActorState[] states;
        int i = 0;

        public ChainAttackStream(float chainWaitSpan,params ActorState[] states)
        {
            this.chainWaitSpan = chainWaitSpan;
            this.states = states;
        }

        public ActorState Proceed()
        {
            if(Time.time > waitTimeLimit) { Reset(); }
            if (i >= states.Length) { return null; }
            return states[i++];
        }

        public void SetTimeLimit(float span)
        {
            waitTimeLimit = Time.time + chainWaitSpan;
        }
        public void Reset()
        {
            i = 0;
        }
    }
}
