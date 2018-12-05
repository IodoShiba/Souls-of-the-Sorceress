using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PlayerStates
{
    public class EndOfAction : State
    {
        [SerializeField] GroundSensor groundSensor;
        public override State Check()
        {
            if (groundSensor.IsOnGround)
            {
                return GetComponent<PlayerOnGround>();
            }
            else
            {
                return GetComponent<PlayerFlying>();
            }
        }
        public override void Initialize(){ }
        public override void Execute() { }
        public override void Terminate() { }

    }
}