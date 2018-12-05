using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStates
{
    public class PlayerDropAttack : State
    {
        [SerializeField] GroundSensor groundSensor;
        [SerializeField] float dropSpeed;
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
            return null;
        }
        public override void Initialize()
        {
        }

        public override void Execute()
        {
            rb.velocity = new Vector2(0, -dropSpeed);
        }

        public override void Terminate()
        {
        }
    }
}
