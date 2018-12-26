using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStates
{
    public class PlayerGuard : State
    {
        [SerializeField] Rigidbody2D player;
        [SerializeField] Player playerData;
        
        public override State Check()
        {
            if(!(playerData.DoesUmbrellaWork() && Input.GetButton("Open Umbrella")))
            {
                return GetComponent<PlayerStates.PlayerOnGround>();
            }
            if (Input.GetButtonDown("Attack"))
            {
                return GetComponent<PlayerStates.PlayerTackle>();
            }
            return null;
        }

        public override void Initialize()
        {
        }

        public override void Execute()
        {
            player.velocity = new Vector2(0, 0);
        }

        public override void Terminate()
        {
        }
    }
}
