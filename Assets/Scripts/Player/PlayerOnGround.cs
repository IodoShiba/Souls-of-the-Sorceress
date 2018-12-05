using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PlayerStates
{
    public class PlayerOnGround : State
    {
        [SerializeField] GroundSensor groundSensor;
        [SerializeField] InputA inputA;
        [SerializeField] Vector2 jumpForce;
        [SerializeField] float horizontalMoveSpeed;
        private float jumpInterval = 0;
        private Rigidbody2D rb;
        private readonly float jumpIntervalDefault = 0.03f;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        public override State Check()
        {
            if (!groundSensor.IsOnGround)
            {
                return GetComponent<PlayerStates.PlayerFlying>();
            }

            if (jumpInterval < 0 && inputA.GetButton("Jump"))
            {
                //rb.AddForce(new Vector2(sign*jumpForce.x, jumpForce.y));
                jumpInterval = jumpIntervalDefault;
                return GetComponent<PlayerFlying>();
            }

            if (Input.GetButtonDown("Attack"))
            {
                return GetComponent<PlayerStates.PlayerVerticalSlash>();
            }

            if(Input.GetButtonDown("Magical Attack"))
            {
                return GetComponent<PlayerStates.PlayerMagicCharging>();
            }

            if(Input.GetButton("Open Umbrella"))
            {
                rb.velocity = new Vector2(0, 0);
                return GetComponent<PlayerStates.PlayerGuard>();
            }

            return null;
        }

        public override void Initialize()
        {
        }

        public override void Execute()
        {

            jumpInterval -= Time.deltaTime;
            int sign = 0;
            if (inputA.GetButton("Right"))
            {
                sign += 1;
            }
            if (inputA.GetButton("Left"))
            {
                sign -= 1;
            }
            rb.velocity = new Vector2(sign * horizontalMoveSpeed, 0);
            
        }

        public override void Terminate()
        {
            return;
        }
    }
}