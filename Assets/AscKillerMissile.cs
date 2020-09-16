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
        [SerializeField] DeadState dead;

        public override SmashedState Smashed => smashed;

        public override ActorState DefaultState => missileDefault;
        public override DeadState Dead => dead;

        protected override void BuildStateConnection()
        {
            base.BuildStateConnection();
            ConnectStateFromDefault(()=>missileDefault.DidStaightDistancePass(), detecting);
            ConnectState(()=>exploding.Complete, exploding, Dead);
            ConnectState(()=>DetectingBranch(), detecting);
        }

        enum DetectedType {player, ground, others}
        void OnTriggerEnter2D(Collider2D other)
        {
            if(other.CompareTag(TagName.player))
            {
                detectedType = DetectedType.player;
                finalContacted = other;
            }
            else if(other.CompareTag(TagName.ground))
            {
                detectedType = DetectedType.ground;
                finalContacted = other;
            }
        }
        Collider2D finalContacted = null;
        DetectedType detectedType = DetectedType.others;
        ActorState DetectingBranch()
        {
            if(finalContacted == null) {return ActorState.continueCurrentState;}
            if(detectedType == DetectedType.ground){return exploding;}
            if(detectedType == DetectedType.player)
            {
                var ascSarah = (ActorSarah.ActorStateConnectorSarah)ActorManager.PlayerActor.FightAsc;
                bool c = ascSarah.isGuard && ascSarah.ShouldbeGuarded(transform.position - finalContacted.transform.position);
                finalContacted = null;
                detectedType = DetectedType.others;
                return c ? (ActorState)smashed : (ActorState)exploding;
            }
            return ActorState.continueCurrentState;
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

            protected override void OnTerminate(bool isNormal)
            {
                rigidbody.velocity = Vector2.zero;
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
                rigidbody.velocity = Vector2.zero;
            }
        }
    }
}