using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorBomb
{
    public class AscBomb : FightActorStateConector
    {
        // *** 状態 *** 

        [SerializeField] BombDefault bombDefault;
        [SerializeField] BombBlasting blasting;
        [SerializeField] BombSmashed smashed;
        [SerializeField] BombDead dead;

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
        class BombBlasting : ActorState
        {
            [SerializeField] float timeSpan;
            [SerializeField] AttackInHitbox explosion;
            float t = 0;

            public bool Complete { get => t > timeSpan; }

            protected override bool ShouldCotinue() => true;

            protected override void OnInitialize()
            {
                t = 0;
                explosion.Activate();
            }

            protected override void OnActive()
            {
                t += Time.deltaTime;
            }

            public override bool IsResistibleTo(System.Type actorStateType)
            {
                return typeof(SmashedState).IsAssignableFrom(actorStateType);
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