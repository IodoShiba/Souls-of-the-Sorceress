using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorBomb
{
    public class AscBombOfBossMecha : FightActorStateConector
    {
        // *** 状態 *** 
        [SerializeField] Rigidbody2D rigidbody;
        [SerializeField] BombDefault bombDefault;
        [SerializeField] CommonActorState.Explosion blasting;
        [SerializeField] BombSmashed smashed;
        [SerializeField] BombDead dead;

        [SerializeField] GameObject explosionEffect;

        public override SmashedState Smashed => smashed;

        public override ActorState DefaultState => bombDefault;

        public override DeadState Dead => dead;
        // *** フィールド ***

        bool ignited = false;

        public float GravityScale
        {
            get => rigidbody.gravityScale;
            set
            {
                rigidbody.gravityScale = value;
            }
        }

        AscBossMecha ownerBoss;
        public AscBossMecha OwnerBoss { get => ownerBoss; set => ownerBoss = value; }

        protected override void BuildStateConnection()
        {
            ConnectStateFromDefault(() => ignited, blasting);
            ConnectState(() => blasting.Complete, blasting, Dead);
        }

        public void Ignite()
        {
            ignited = true;
            GameObject obj = Instantiate(explosionEffect, gameObject.transform.position, Quaternion.identity);
            obj.transform.localScale = new Vector3(0.5f,0.5f,1f);
        }

        [System.Serializable]
        class BombDefault : DefaultState
        {
            [SerializeField] ActorBomb.BombOfBossMechaAI ai;

            AscBombOfBossMecha connectorBomb = null;
            AscBombOfBossMecha ConnectorBomb { get => connectorBomb == null ? (connectorBomb = Connector as AscBombOfBossMecha) : connectorBomb; }

            protected override void OnActive()
            {
                if(ConnectorBomb!=null && ConnectorBomb.OwnerBoss.IsDead)
                {
                    ConnectorBomb.InterruptWith(ConnectorBomb.Dead);
                }

                ai.Decide();

                if (ai.DetectingPlayer || (ConnectorBomb.GravityScale>0 && ai.HitToGround))
                {
                    ConnectorBomb.Ignite();
                }
            }
        }

        [System.Serializable]
        class BombSmashed : FightActorStateConector.SmashedState
        {

        }

        [System.Serializable]
        class BombDead : FightActorStateConector.DeadState
        {
            protected override void OnInitialize()
            {
                base.OnInitialize();
            }
        }
    }
}