using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorSlime
{
    public class AscSlime : FightActorStateConector
    {
        [SerializeField] SlimeAI ai;

        [SerializeField] SlimeDefault slimeDefault;
        [SerializeField] Spike spike;
        [SerializeField] SmashedState smashed;
        [SerializeField] Animator slimeAnimator;

        public override SmashedState Smashed => smashed;
        public override ActorState DefaultState => slimeDefault;

        protected override void BuildStateConnection()
        {
            ConnectStateFromDefault(()=>ai.Spike, spike);
        }

        protected override void BeforeStateUpdate()
        {
            ai.Decide();
        }

        [System.Serializable]
        class SlimeDefault : DefaultState
        {
            [SerializeField] ActorFunction.HorizontalMove horizontalMove;

            AscSlime connectorSlime;

            AscSlime ConnectorSlime { get => connectorSlime == null ? (connectorSlime = Connector as AscSlime) : connectorSlime; }

            protected override void OnInitialize()
            {
                ConnectorSlime.slimeAnimator.Play("Attack_Back");
            }

            protected override void OnActive()
            {
                horizontalMove.ManualUpdate(ConnectorSlime.ai.MoveSign);
            }
        }

        [System.Serializable]
        class Spike : ActorState
        {
            [SerializeField] AttackInHitbox spikeAttack;
            [SerializeField] SpriteRenderer spikeSpriteRenderer;
            [SerializeField] ActorFunction.HorizontalMove horizontalMove;

            AscSlime connectorSlime;

            AscSlime ConnectorSlime { get => connectorSlime == null ? (connectorSlime = Connector as AscSlime) : connectorSlime; }

            protected override bool ShouldCotinue() => ConnectorSlime.ai.Spike;

            protected override void OnInitialize()
            {
                ConnectorSlime.slimeAnimator.Play("Attack");
            }

            protected override void OnActive()
            {
                horizontalMove.ManualUpdate(0);
                spikeAttack.Activate();
                spikeSpriteRenderer.enabled = true;
            }

            protected override void OnTerminate(bool isNormal)
            {
                spikeAttack.Inactivate();
                spikeSpriteRenderer.enabled = false;
            }
        }
    }
}
