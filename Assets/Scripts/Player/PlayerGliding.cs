using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStates
{
    public class PlayerGliding : State
    {
        [SerializeField] GroundSensor groundSensor;
        [SerializeField] InputA inputA;
        [SerializeField] AwakeMutableObject UmbrellaUpward;
        [SerializeField] float horizontalMoveSpeed;
        [SerializeField] float maxFallSpeed;

        private Rigidbody2D rb;

        // Use this for initialization
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }
        public override State Check()
        {
            if (groundSensor.IsOnGround)
            {
                return GetComponent<PlayerStates.PlayerOnGround>();
            }

            if (!Input.GetButton("Open Umbrella"))
            {
                return GetComponent<PlayerStates.PlayerFlying>();
            }

            if (Input.GetButtonDown("Attack"))
            {
                return GetComponent<PlayerStates.PlayerRisingAttack>();
            }

            return null;
        }
        public override void Initialize()
        {
        }

        public override void Execute()
        {

            bool r = false;
            bool l = false;
            Vector2 newv;
            newv.x = newv.y = 0;

            //横移動
            if (r = inputA.GetButton("Right"))
            {
                newv.x += horizontalMoveSpeed;
            }
            if (l = inputA.GetButton("Left"))
            {
                newv.x -= horizontalMoveSpeed;
            }
            if (!(r || l))
            {
                newv.x = 0;
            }
            
            newv.y = System.Math.Max(rb.velocity.y, -maxFallSpeed);

            rb.velocity = newv;
            //Debug.Log(rb.velocity.y);
        }

        public override void Terminate()
        {
        }
    }
}
