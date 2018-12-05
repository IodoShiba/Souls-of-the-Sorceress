using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStates
{
    public class PlayerRisingAttack : State
    {
        [SerializeField] float risingSpeed;
        [SerializeField] float _motionLength;
        [SerializeField] AwakeMutableObject attackTrigger;
        private Rigidbody2D rb;
        private float t;
        // Use this for initialization
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }
        public override State Check()
        {
            if (t > _motionLength)
            {
                return GetComponent<PlayerGliding>();
            }
            else
            {
                return null;
            }
        }
        public override void Initialize()
        {

        }

        public override void Execute()
        {
            rb.velocity = new Vector2(0, risingSpeed);
            t += Time.deltaTime;
        }

        public override void Terminate()
        {
            rb.velocity = new Vector2(0, 0);
            t = 0;
        }
    }
}
