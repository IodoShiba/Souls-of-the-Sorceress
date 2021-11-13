using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorBomb
{
    public class AscBomb : FightActorStateConector
    {
        // *** 状態 *** 

        [SerializeField] BombDefault bombDefault;
        [SerializeField] CommonActorState.Explosion blasting;
        [SerializeField] BombSmashed smashed;
        [SerializeField] BombDead dead;

        [SerializeField] GameObject explosionEffect;
        [SerializeField] AnimationClip explosionClip;

        public override SmashedState Smashed => smashed;

        public override ActorState DefaultState => bombDefault;

        public override DeadState Dead => dead;
        // *** フィールド ***

        bool detectingPlayer = false;
        float detectingTime;
        bool ignited = false;

        protected override void BuildStateConnection()
        {
            ConnectStateFromDefault(() => ignited, blasting);
            ConnectState(() => blasting.Complete, blasting, Dead);
        }

        public void Ignite()
        {
            ignited = true;
            EffectAnimationManager.Play(explosionClip,transform.position,new Vector3(0.5f,0.5f,0.5f));
        }

        [System.Serializable]
        class BombDefault : DefaultState
        {
            [SerializeField] ActorBomb.BombAI ai;
            [SerializeField] ActorFunction.HorizontalMove horizontalMove;
            [SerializeField] ActorFunction.Directionable directionable;

            AscBomb connectorBomb = null;
            AscBomb ConnectorBomb { get => connectorBomb == null ? (connectorBomb = Connector as AscBomb) : connectorBomb; }

            protected override void OnActive()
            {
                ai.Decide();

                directionable.CurrentDirection = (ActorFunction.Directionable.Direction)ai.MoveDir;
                horizontalMove.ManualUpdate(ai.MoveDir);

                if (ai.Ignite)
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

        }
    }
}