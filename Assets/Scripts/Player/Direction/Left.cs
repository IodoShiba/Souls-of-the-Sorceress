using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStates {
    namespace Direction {
        public class Left : State
        {
            [SerializeField] StateManager behavior;
            HorizontalMove _horizontalMove;
            bool initialized =false;

            HorizontalMove horizontalMove { get => _horizontalMove != null ? _horizontalMove : _horizontalMove = GetComponent<HorizontalMove>(); }
            // Use this for initialization
            void Start()
            {
                _horizontalMove = GetComponent<HorizontalMove>();
            }

            // Update is called once per frame
            void Update()
            {

            }

            public override State Check()
            {
                if (horizontalMove.Sign == 1)//((Input.GetAxis("Horizontal") > 0))// && ((behavior.CurrentState is PlayerOnGround) || (behavior.CurrentState is PlayerFlying)))
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
