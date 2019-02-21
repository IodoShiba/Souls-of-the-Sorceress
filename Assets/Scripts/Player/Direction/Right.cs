using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStates
{
    namespace Direction
    {
        public class Right : State
        {
            [SerializeField] StateManager behavior;
            bool initialized = false;
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
                //Debug.Log(string.Format("{0},{1}", Input.GetButton("Right") , Input.GetButton("Left")));
                if ((!Input.GetButton("Right") && Input.GetButton("Left") || Input.GetAxis("Horizontal") < 0))// && ((behavior.CurrentState is PlayerOnGround )|| (behavior.CurrentState is PlayerFlying)))
                {
                    return GetComponent<PlayerStates.Direction.Left>();
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
