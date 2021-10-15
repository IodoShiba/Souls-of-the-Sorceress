using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorBomb
{
    public class BombAI : AI
    {
        [SerializeField] float maxChaseDistance;
        [SerializeField] float minChaseDistance;
        [SerializeField] float timeToIgnite;
        [SerializeField] Color _def;
        [SerializeField] Color _igniting;
        [SerializeField] SpriteRenderer spriteRenderer;
        [SerializeField] bool ignitingColorChange;

        Actor player;
        Actor Player { get => player == null ? (player = ActorManager.PlayerActor) : player; }

        float detectingTime = 0;

        bool detectingPlayer = false;
        bool DetectingPlayer
        { 
            get => detectingPlayer;
            set 
            {
                if (detectingPlayer && !value) { detectingTime = 0; }
                detectingPlayer = value;
            } 
        }

        public bool Ignite
        {
            get => detectingTime > timeToIgnite;
        }

        int moveDir;
        public int MoveDir { get => moveDir; }

        public override void Decide()
        {
            float dx = Player.transform.position.x - transform.position.x;
            
            if(minChaseDistance < Mathf.Abs(dx) && Mathf.Abs(dx) < maxChaseDistance)
            {
                moveDir = System.Math.Sign(dx);
            }

            if (DetectingPlayer)
            {
                detectingTime += Time.deltaTime;
            }

            if(spriteRenderer!=null&&ignitingColorChange)
            {
                spriteRenderer.color = Color.Lerp(_def, _igniting, detectingTime / timeToIgnite);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.tag == TagName.player)
            {
                Debug.Log("Detecting Player...");
                DetectingPlayer = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if(collision.tag == TagName.player)
            {
                DetectingPlayer = false;
            }
        }
    }
}
