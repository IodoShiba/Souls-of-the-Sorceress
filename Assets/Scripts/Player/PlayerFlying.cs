using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStates { 
    public class PlayerFlying : State
    {
        [SerializeField] GroundSensor groundSensor;
        [SerializeField] InputA inputA;
        [SerializeField] Player player;
        [SerializeField] float horizontalMoveSpeed;
        [SerializeField] float verticalMoveSpeed;
        [SerializeField] float maxFallSpeed;
        [SerializeField] float jumpMaxHeight;
        [SerializeField] Rigidbody2D rb;
        private float jumpBorder;
        private bool ableToJump;
        private bool jumping=true;

        private void Start()
        {
            ableToJump = false;
            rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            if (!ableToJump&& groundSensor.IsOnGround) {
                ableToJump = true;
            }
        }
        public override State Check()
        {
            if (/*!inputA.GetButton("Jump")*/!ableToJump && groundSensor.IsOnGround)
            {
                return GetComponent<PlayerStates.PlayerOnGround>();
            }

            if (player.DoesUmbrellaWork()&&Input.GetButton("Open Umbrella"))
            {
                ableToJump = false;
                if (rb.velocity.y > 0)
                {
                    rb.velocity = new Vector2(rb.velocity.x, 0);
                }
                return GetComponent<PlayerStates.PlayerGliding>();
            }

            if (inputA.GetMultiButtonDown("Attack", "Down"))
            {
                return GetComponent<PlayerStates.PlayerDropAttack>();
            }
            
            if (inputA.GetButtonShortDownUp("Attack"))
            {
                ableToJump = false;
                return GetComponent<PlayerStates.PlayerAerialSlash>();
            }

            if (Input.GetButtonDown("Magical Attack"))
            {
                ableToJump = false;
                return GetComponent<PlayerStates.PlayerMagicCharging>();
            }

            return null;
        }

        public override void Initialize()
        {
            if (!groundSensor.IsOnGround) {
                ableToJump = false;
            }
            /*if (ableToJump && Input.GetButton("Jump"))//ジャンプする
            {
                if (!jumping)
                {
                    jumpBorder = gameObject.transform.position.y + jumpMaxHeight;
                }
                jumping = true;
                rb.velocity = Vector2.up * verticalMoveSpeed;
            }*/
        }

        public override void Execute()
        {
            //Debug.Log("Flying");

            bool r = false;
            bool l = false;
            Vector2 newv;
            newv.x = newv.y = 0;

            //横移動
            if (r=inputA.GetButton("Right"))
            {
                newv.x += horizontalMoveSpeed;
            }
            if (l=inputA.GetButton("Left"))
            {
                newv.x -= horizontalMoveSpeed;
            }
            if(!(r||l))
            {
                newv.x = 0;
            }
            //newv.x = System.Math.Sign(Input.GetAxis("Horizontal"))*horizontalMoveSpeed;

            //ジャンプ
            if (ableToJump&&Input.GetButton("Jump"))//ジャンプする
            {
                if (!jumping)
                {
                    jumpBorder = gameObject.transform.position.y + jumpMaxHeight;
                }
                jumping = true;
                newv.y = verticalMoveSpeed;
            }
            else//ジャンプしない
            {
                jumping = false;
                newv.y = System.Math.Max(rb.velocity.y, -maxFallSpeed);
            }
            if ((Input.GetButtonUp("Jump")&& rb.velocity.y>0) || (transform.position.y > jumpBorder && jumping))//ジャンプやめる
            {
                newv.y = 0;
                ableToJump = false;
            }

            rb.velocity = newv;
            //Debug.Log(rb.velocity.y);
        }

        public override void Terminate()
        {
            if (jumping)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }
        }
    }
}
