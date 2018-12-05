using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStates {
    namespace Direction {
        public class Left : State
        {
            [SerializeField] StateManager behavior;
            bool initialized =false;
            // Use this for initialization
            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {

            }

            public override State Check()
            {
                if (Input.GetButton("Right") && !Input.GetButton("Left") && ((behavior.CurrentState is PlayerOnGround) || (behavior.CurrentState is PlayerFlying)))
                {
                    return GetComponent<PlayerStates.Direction.Right>();
                }
                return null;
            }
            public override void Initialize()
            {
                initialized = true;
            }
            public override void Execute()
            {
            }
            public override void Terminate()
            {
                initialized = false;
            }
        }
    }
}
