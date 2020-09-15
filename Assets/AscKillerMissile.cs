using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorKillerMissile
{
    public class AscKillerMissile : FightActorStateConector
    {
        [SerializeField] MissileDefault missileDefault;
        [SerializeField] Detecting detecting;
        [SerializeField] CommonActorState.Explosion exploding;
        [SerializeField] MisileSmashed smashed;

        public override SmashedState Smashed => smashed;

        public override ActorState DefaultState => missileDefault;

        protected override void BuildStateConnection()
        {
            base.BuildStateConnection();
            ConnectStateFromDefault(()=>missileDefault.DidStaightDistancePass(), detecting);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            // if(other.CompareTag(TagName.attack) && other.transform.parent != null && other.transform.parent.CompareTag(TagName.player))
            // {
            //     // InterruptWith(smashed);
            // }
            //else
            if(other.CompareTag(TagName.player))
            {
                InterruptWith(exploding);
            }
        }

        [System.Serializable]
        class MissileDefault : DefaultState
        {
            [SerializeField] Rigidbody2D rigidbody;
            [SerializeField] float distanceGoStraight;
            Vector2 initPos;

            public bool DidStaightDistancePass() => Vector2.Distance(initPos, GameObject.transform.position) >= distanceGoStraight;

            AscKillerMissile connectorMissile;
            AscKillerMissile ConnectorMissile 
            {
                get => connectorMissile == null ? (connectorMissile = (AscKillerMissile)Connector) : connectorMissile;
            }

            protected override void OnInitialize()
            {
                initPos = GameObject.transform.position;
            }
        }

        [System.Serializable]
        class Detecting : ActorState
        {
            [SerializeField] Rigidbody2D rigidbody;
            [SerializeField] float accel;
            Vector2 dir;
            protected override void OnInitialize()
            {
                base.OnInitialize();
                dir = (ActorManager.PlayerActor.transform.position - GameObject.transform.position).normalized;
                rigidbody.velocity = Vector2.zero;
            }

            protected override void OnActive()
            {
                base.OnActive();
                rigidbody.AddForce(dir*(accel*rigidbody.mass));
            }

            protected override bool ShouldCotinue() => true;
        }


        [System.Serializable]
        class MisileSmashed : SmashedState 
        {
            [SerializeField] Rigidbody2D rigidbody;
            [SerializeField] float gravityScale;

            protected override void OnInitialize()
            {
                base.OnInitialize();
                rigidbody.gravityScale = gravityScale;
            }
        }
    }
}