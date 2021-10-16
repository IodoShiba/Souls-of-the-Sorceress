using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorBomb
{
    public class BombOfBossMechaAI : AI
    {
        [SerializeField] SpriteRenderer spriteRenderer;

        Actor player;
        Actor Player { get => player == null ? (player = ActorManager.PlayerActor) : player; }


        bool detectingPlayer = false;
        public bool DetectingPlayer
        { 
            get => detectingPlayer;
            private set 
            {
                detectingPlayer = value;
            } 
        }
        bool hitToGround = false;
        public bool HitToGround => hitToGround;


        public override void Decide()
        {
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.CompareTag(TagName.player))
            {
                DetectingPlayer = true;
            }

            if(collision.CompareTag(TagName.ground))
            {
                hitToGround = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if(collision.CompareTag(TagName.player))
            {
                DetectingPlayer = false;
            }
        }
        
    }
}
