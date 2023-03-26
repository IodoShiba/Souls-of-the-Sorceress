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
        //private bool ableToJump;
        //private bool jumping=true;
        Vector2 newv;
        /*
        abstract class JumpState
        {
            public readonly string name;
            protected PlayerFlying owner;

            public JumpState(PlayerFlying owner,string name) { this.owner = owner;this.name = name; }
            public abstract JumpState Check();
            public abstract void Execute();
        }
        private JumpState jumpState = null; //nullは「未定」の意

        class Jumping : JumpState
        {
            private float jumpBorder;
            public Jumping(PlayerFlying owner) : base(owner,nameof(Jumping))
            {
                jumpBorder = owner.gameObject.transform.position.y + owner.jumpMaxHeight;
            }

            public override JumpState Check()
            {
                if (Input.GetButtonUp("Jump") || (owner.transform.position.y > jumpBorder))//ジャンプやめる条件
                {
                    return new NotJumping(owner);
                }
                return null;
            }
            public override void Execute()
            {
                owner.newv.y = owner.rb.velocity.y;//verticalMoveSpeed;
            }
        }

        class NotJumping : JumpState
        {
            public NotJumping(PlayerFlying owner) : base(owner,nameof(NotJumping)) { }
            public override JumpState Check()
            {
                return null;
            }
            public override void Execute()
            {
                owner.newv.y = System.Math.Max(owner.rb.velocity.y, -owner.maxFallSpeed);
            }
        }*/

        private void Start()
        {
            //ableToJump = false;
            rb = GetComponent<Rigidbody2D>();
        }

        public override State Check()
        {/*
            bool onGround;
            if (groundSensor.IsOnGround)
            {
                if (jumpState == null)
                {
                    onGround = !Input.GetButton("Jump");
                }
                else
                {
                    onGround = jumpState.name != nameof(Jumping);//ジャンプしている間は地面センサーが地面を検知しても空中に居る扱い
                }
            }
            else
            {
                onGround = false;
            }
            if (onGround)
            {
                return GetComponent<PlayerStates.PlayerOnGround>();
            }*/

            if (player.DoesUmbrellaWork()&&InputDaemon.IsPressed("Open Umbrella"))
            {
                //ableToJump = false;
                if (rb.velocity.y > 0)
                {
                    rb.velocity = new Vector2(rb.velocity.x, 0);
                }
                //return GetComponent<PlayerStates.PlayerGliding>();
            }

            if (inputA.GetMultiButtonDown("Attack", "Down"))
            {
                return GetComponent<PlayerStates.PlayerDropAttack>();
            }
            
            if (inputA.GetButtonShortDownUp("Attack"))
            {
                //ableToJump = false;
                return GetComponent<PlayerStates.PlayerAerialSlash>();
            }

            if (InputDaemon.WasPressedThisFrame("Magical Attack"))
            {
                //ableToJump = false;
                return GetComponent<PlayerStates.PlayerMagicCharging>();
            }

            return null;
        }

        public override void Initialize()
        {
            /*
            if (groundSensor.IsOnGround && Input.GetButton("Jump"))
            {
                jumpState = new Jumping(this);
            }
            else
            {
                jumpState = new NotJumping(this);
            }
            */
        }

        public override void Execute()
        {
            //Debug.Log("Flying");
            /*
            bool r = false;
            bool l = false;
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

            JumpState next;
            while ((next = jumpState.Check()) != null)
            {
                jumpState = next;
            }
            jumpState.Execute();
            //Debug.Log(jumpState.name);

            rb.velocity = Vector2.up * newv.y + Vector2.right * rb.velocity.x;
            //rb.velocity = newv;
            //Debug.Log(rb.velocity.y);
            */
        }

        public override void Terminate()
        {
            //jumpState = null;
        }
        /*
        public string CurrentJumpState()
        {
            return jumpState.name;
        }
        */
    }
}

