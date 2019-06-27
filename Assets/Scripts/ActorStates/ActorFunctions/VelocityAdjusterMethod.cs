using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IodoShiba.RigidbodySetVelocity;

namespace ActorFunction
{
    [System.Serializable]
    public class VelocityAdjusterFields : ActorFunction.ActorFunctionFields
    {
        [SerializeField] Vector2 maxForce;
        [SerializeField] Vector2 velocity;
        [SerializeField] bool activeX;
        [SerializeField] bool activeY;
        public Vector2 MaxForce { get => maxForce; set => maxForce = value; }
        public Vector2 Velocity { get => velocity; set => velocity = value; }
        public bool ActiveX { get => activeX; set => activeX = value; }
        public bool ActiveY { get => activeY; set => activeY = value; }
        public class Method : ActorFunctionMethod<VelocityAdjusterFields>
        {
            new Rigidbody2D rigidbody;
            Rigidbody2D Rigidbody { get => rigidbody == null ? (rigidbody = GetComponent<Rigidbody2D>()) : rigidbody; }
            VelocityAdjusterFields fields;
            public override void ManualUpdate(in VelocityAdjusterFields fields)
            {
                this.fields = fields;
            }
            private void FixedUpdate()
            {
                if (fields == null) { return; }
                if (fields.activeX) { Rigidbody.SetVelocityX(fields.velocity.x, fields.maxForce.x); }
                if (fields.activeY) { Rigidbody.SetVelocityY(fields.velocity.y, fields.maxForce.y); }
            }
        }
    }

    public class VelocityAdjusterMethod : VelocityAdjusterFields.Method { }

    [System.Serializable]
    public class VelocityAdjuster : ActorFunction<VelocityAdjusterFields, VelocityAdjusterFields.Method> 
    { }

}
//namespace PlayerStates
//{
//    public class PlayerGliding : State
//    {
//        [SerializeField] GroundSensor groundSensor;
//        [SerializeField] InputA inputA;
//        [SerializeField] Player player;
//        //[SerializeField] AwakeMutableObject UmbrellaUpward;
//        [SerializeField] float horizontalMoveSpeed;
//        [SerializeField] float maxFallSpeed;

//        private Rigidbody2D rb;

//        // Use this for initialization
//        void Start()
//        {
//            rb = GetComponent<Rigidbody2D>();
//        }
//        public override State Check()
//        {
//            if (groundSensor.IsOnGround)
//            {
//                return GetComponent<PlayerStates.PlayerOnGround>();
//            }
//            if (!player.DoesUmbrellaWork())
//            {
//                return GetComponent<EndOfAction>();
//            }

//            if (!Input.GetButton("Open Umbrella"))
//            {
//                return GetComponent<PlayerStates.PlayerFlying>();
//            }

//            if (Input.GetButtonDown("Attack"))
//            {
//                //return GetComponent<PlayerStates.PlayerRisingAttack>();
//            }

//            return null;
//        }
//        public override void Initialize()
//        {
//        }

//        public override void Execute()
//        {

//            bool r = false;
//            bool l = false;
//            Vector2 newv;
//            newv.x = newv.y = 0;

//            //横移動
//            if (r = inputA.GetButton("Right"))
//            {
//                newv.x += horizontalMoveSpeed;
//            }
//            if (l = inputA.GetButton("Left"))
//            {
//                newv.x -= horizontalMoveSpeed;
//            }
//            if (!(r || l))
//            {
//                newv.x = 0;
//            }
//            //newv.x = System.Math.Sign(Input.GetAxis("Horizontal")) * horizontalMoveSpeed;

//            newv.y = System.Math.Max(rb.velocity.y, -maxFallSpeed);

//            rb.velocity = newv;
//            //Debug.Log(rb.velocity.y);
//        }

//        public override void Terminate()
//        {
//        }
//    }
//}

//public class Glide : BasicAbility
//{
//    [SerializeField] Player player;
//    [SerializeField] float horizontalMoveSpeed;
//    [SerializeField] float maxFallSpeed;
//    [SerializeField] Umbrella umbrella;
//    [SerializeField] GroundSensor groundSensor;
//    [SerializeField] HorizontalMove horizontalMove;

//    private Rigidbody2D rb;

//    void Start()
//    {
//        rb = GetComponent<Rigidbody2D>();
//    }
//    private void FixedUpdate()
//    {
//        if (Activated) { rb.AddForce(Vector2.up * (rb.mass * (System.Math.Max(rb.velocity.y, -maxFallSpeed) - rb.velocity.y) / Time.deltaTime)); }
//    }

//    protected override bool ShouldContinue(bool ordered)
//    {
//        return ordered && !groundSensor.IsOnGround && player.DoesUmbrellaWork();
//    }
//    protected override void OnInitialize()
//    {
//        umbrella.PlayerGliding();
//        horizontalMove.moveSpeed = horizontalMoveSpeed;
//    }
//    protected override void OnTerminate()
//    {
//        umbrella.Default();
//        horizontalMove.ResetSpeed();
//    }
//}