using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStates
{
    public class PlayerTackle : State
    {
        [SerializeField] float speed;
        [SerializeField] float _motionLength;
        [SerializeField] Rigidbody2D playerRb;
        [SerializeField] Sensor wallSensor;
        private int dirSign = 0;
        private float t=0;
        private Vector2 v;
        private bool initialized = false;

        public override State Check()
        {
            if (initialized)
            {
                if (t > _motionLength || wallSensor.IsDetecting)
                {
                    playerRb.velocity = new Vector2(0, 0);
                    return GetComponent<PlayerStates.PlayerOnGround>();
                    //return GetComponent<PlayerStates.PlayerGuard>();
                }
            }
            return null;
        }

        public override void Initialize()
        {
            t = 0;
            v= new Vector2(dirSign * speed, 0);
            //wallSensor.Reset();
            initialized = true;
        }

        public override void Execute()
        {
            playerRb.velocity = v;
            t += Time.deltaTime;
        }

        public override void Terminate()
        {
            wallSensor.Reset();
            playerRb.velocity = new Vector2(0, 0);
            t = 0;
            initialized = false;
        }

        public void SetDirection(int sign)
        {
            dirSign = sign;
        }
        
        /*private void OnCollisionEnter2D(Collision2D collision)
        {
            if(collision.transform.tag == "Ground")
            {
                expired = true;
            }
        }*/
    }
}
