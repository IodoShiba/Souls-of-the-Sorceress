using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStates
{
    public class PlayerDamaged : State
    {
        [SerializeField] Rigidbody2D playerRb;
        [SerializeField] Vector2 knockBackImpact;
        [SerializeField] float _motionLength;
        private float t = 0;
        private Collider2D attacker;

        public override State Check()
        {
            if (t > _motionLength)
            {
                return GetComponent<PlayerStates.EndOfAction>();
            }
            return null;
        }

        public override void Initialize()
        {
            t = 0;
            //int sign = System.Math.Sign(playerRb.position.x - attacker.transform.position.x);
            //playerRb.velocity = new Vector2(0, 0);
            //Debug.Log("Sarah:Ouch.");
            //playerRb.AddForce(new Vector2(sign * knockBackImpact.x, knockBackImpact.y));
        }

        public override void Execute()
        {
            t += Time.deltaTime;
        }

        public override void Terminate()
        {
            t = 0;
        }

        /*public void TeachCollider(Collider2D attacker) {
            this.attacker = attacker;
        }*/

    }
}