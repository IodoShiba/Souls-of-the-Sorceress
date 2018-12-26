using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStates {
    public class PlayerAerialSlash : State
    {
        [SerializeField] Rigidbody2D playerRb;
        [SerializeField] GroundSensor groundSensor;
        [SerializeField] float _motionLength;
        float t = 0;

        public override State Check()
        {
            if (t > _motionLength || groundSensor.IsOnGround) 
            {
                return GetComponent<PlayerStates.EndOfAction>();
            }
            return null;
        }

        public override void Initialize()
        {
            t = 0;
        }

        public override void Execute()
        {
            //playerRb.velocity = Vector2.zero;
            t += Time.deltaTime;
        }

        public override void Terminate()
        {
            t = 0;
        }
    }
}
