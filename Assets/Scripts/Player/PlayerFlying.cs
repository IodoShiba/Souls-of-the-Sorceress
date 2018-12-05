﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStates { 
    public class PlayerFlying : State
    {
        [SerializeField] GroundSensor groundSensor;
        [SerializeField] InputA inputA;
        [SerializeField] float horizontalMoveSpeed;
        [SerializeField] float verticalMoveSpeed;
        [SerializeField] float maxFallSpeed;
        [SerializeField] float jumpMaxHeight;
        private Rigidbody2D rb;
        private float jumpBorder;
        private bool ableToJump;
        private bool jumping=true;

        private void Start()
        {
            ableToJump = true;
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
            if (!inputA.GetButton("Jump") && groundSensor.IsOnGround)
            {
                return GetComponent<PlayerStates.PlayerOnGround>();
            }

            if (Input.GetButton("Open Umbrella"))
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
                return GetComponent<PlayerStates.PlayerAerialSlash>();
            }

            if (Input.GetButtonDown("Magical Attack"))
            {
                return GetComponent<PlayerStates.PlayerMagicCharging>();
            }

            return null;
        }

        public override void Initialize()
        {

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
        }
    }
}
