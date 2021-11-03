using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorSlime
{
    public class AscSlime : FightActorStateConector
    {
        [SerializeField] SlimeAI ai;

        [SerializeField, DisabledField] string _currentStateName;
        [SerializeField] SlimeDefault slimeDefault;
        [SerializeField] Spike spike;
        [SerializeField] SlimeSmashedState smashed;
        [SerializeField] Animator slimeAnimatorOld;
        [SerializeField] Animator slimeAnimatorAnima2D;
        [SerializeField] bool isHorizontal;

        const string ANIMATION_NAME_MOVE        = "Move";
        const string ANIMATION_NAME_ATTACK      = "Attack";
        const string ANIMATION_NAME_ATTACK_BACK = "Attack_Back";
        const string ANIMATION_NAME_SMASHED     = "Smashed";

        public override SmashedState Smashed => smashed;
        public override ActorState DefaultState => slimeDefault;

        protected override void BuildStateConnection()
        {
            ConnectStateFromDefault(()=>ai.Spike, spike);
        }

        protected override void BeforeStateUpdate()
        {
            ai.Decide();
            _currentStateName = Current.GetType().Name;
        }

        [System.Serializable]
        class SlimeDefault : DefaultState
        {
            [SerializeField] ActorFunction.HorizontalMove horizontalMove;

            AscSlime connectorSlime;

            AscSlime ConnectorSlime { get => connectorSlime == null ? (connectorSlime = Connector as AscSlime) : connectorSlime; }

            protected override void OnInitialize()
            {
                base.OnInitialize();
                ConnectorSlime.ManageSlimeAnimation(ANIMATION_NAME_MOVE);
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
                ConnectorSlime.ManageSlimeAnimation(ANIMATION_NAME_ATTACK);
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
                ConnectorSlime.ManageSlimeAnimation(ANIMATION_NAME_ATTACK_BACK);
            }
        }

        [System.Serializable]
        class SlimeSmashedState : SmashedState
        {
            AscSlime connectorSlime;

            AscSlime ConnectorSlime { get => connectorSlime == null ? (connectorSlime = Connector as AscSlime) : connectorSlime; }

            protected override void OnInitialize()
            {
                base.OnInitialize();
                ConnectorSlime.ManageSlimeAnimation(ANIMATION_NAME_SMASHED);
            }

            protected override void OnTerminate(bool isNormal)
            {
                base.OnTerminate(isNormal);
                ConnectorSlime.ManageSlimeAnimation(ANIMATION_NAME_MOVE);
            }
        }

        void ManageSlimeAnimation(string stateName)
        {
            if (isHorizontal)
            {
                if(stateName == ANIMATION_NAME_MOVE)
                {
                    slimeAnimatorOld.Play(stateName);
                    slimeAnimatorOld.gameObject.SetActive(false);
                    slimeAnimatorAnima2D.gameObject.SetActive(true);
                    slimeAnimatorAnima2D.Play(stateName);
                }
                else
                {
                    slimeAnimatorOld.gameObject.SetActive(true);
                    slimeAnimatorOld.Play(stateName);
                    slimeAnimatorAnima2D.Play(stateName);
                    slimeAnimatorAnima2D.gameObject.SetActive(false);
                }
            }
            else
            {
                slimeAnimatorOld.Play(stateName);
            }
        }
    }
}
