using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStates
{
    public class PlayerReturnSlash : State
    {
        [SerializeField] float _motionLength;
        [SerializeField] float nextAttackRestrictedTime;
        float t = 0;

        public override State Check()
        {
            if (t > _motionLength)
            {
                return GetComponent<PlayerStates.EndOfAction>();
            }
            else
            {
                if (t > nextAttackRestrictedTime && InputDaemon.IsPressed("Attack"))
                {
                    return GetComponent<PlayerStates.PlayerSmashSlash>();
                }
            }
            return null;
        }

        public override void Initialize()
        {
            t = 0;
        }

        public override void Execute()
        {
            t += Time.deltaTime;
        }

        public override void Terminate()
        {
            t = 0;
        }
    }
}