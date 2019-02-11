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
        [SerializeField] Player player;
        private float jumpInterval = 0;
        private Rigidbody2D rb;
        private readonly float jumpIntervalDefault = 0.03f;
        private HorizontalMove hm;
        private HorizontalMove.VelocityShift vs;
        private int formerDirSign = 0;

        private void Awake()
        {
            hm = player.gameObject.GetComponent<HorizontalMove>();
        }

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            vs = hm.CreateVelocityShift(horizontalMoveSpeed,4);
        }

        public override State Check()
        {
            if (!groundSensor.IsOnGround)
            {
                return GetComponent<PlayerStates.PlayerFlying>();
            }

            if (jumpInterval < 0 && Input.GetButtonDown("Jump"))
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

            if (player.DoesUmbrellaWork() && Input.GetButton("Open Umbrella")) 
            {
                rb.velocity = new Vector2(0, 0);
                return GetComponent<PlayerStates.PlayerGuard>();
            }

            return null;
        }

        public override void Initialize()
        {
            rb.velocity = Vector2.zero;
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

            //rb.velocity = Vector2.right * sign * horizontalMoveSpeed;//new Vector2(sign * horizontalMoveSpeed, 0);
            vs.velocity = sign * horizontalMoveSpeed;
            vs.Update();
            formerDirSign = sign;
        }

        public override void Terminate()
        {
            return;
        }
    }
}