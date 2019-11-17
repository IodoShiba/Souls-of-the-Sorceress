using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorWalkingMushroom
{
    public class AscWalkingMushroom : FightActorStateConector
    {
        [SerializeField] WalkingMushAI ai;
        [SerializeField] ActorFunction.Directionable direction;

        [SerializeField, DisabledField] string _currentStateName;
        [SerializeField] MushDefault mushDefault;
        [SerializeField] MushSmashed smashed;
        [SerializeField] Animator smallMushAnimator;

        public override SmashedState Smashed => smashed;

        public override ActorState DefaultState => mushDefault;
        protected override void BeforeStateUpdate()
        {
            ai.Decide();
            _currentStateName = Current.GetType().Name;
        }

        [System.Serializable]
        private class MushDefault : DefaultState
        {
            AscWalkingMushroom connectorMush;
            AscWalkingMushroom ConnectorMush { get => connectorMush == null ? (connectorMush = Connector as AscWalkingMushroom) : connectorMush; }

            [SerializeField] ActorFunction.HorizontalMove horizontalMove;

            protected override void OnInitialize()
            {
                //horizontalMove.Use = true;
                ConnectorMush.smallMushAnimator.Play("Move");
            }
            protected override void OnActive()
            {
                if (ConnectorMush.ai.MoveSign * (int)ConnectorMush.direction.CurrentDirection < 0)
                {
                    ConnectorMush.direction.ChangeDirection(ConnectorMush.ai.MoveSign);
                }
                //Debug.Log(GameObject.name + ConnectorMush.ai.MoveSign);
                horizontalMove.ManualUpdate(ConnectorMush.ai.MoveSign);

            }
            protected override void OnTerminate(bool isNormalTermination)
            {
                //horizontalMove.Use = false;
            }
        }

        [System.Serializable]
        private class MushSmashed : SmashedState
        {
            AscWalkingMushroom connectorMush;
            AscWalkingMushroom ConnectorMush { get => connectorMush == null ? (connectorMush = Connector as AscWalkingMushroom) : connectorMush; }

            protected override void OnInitialize()
            {
                base.OnInitialize();
            }

            protected override void OnActive()
            {
                base.OnActive();
                ConnectorMush.smallMushAnimator.Play("Smashed");
            }
        }
    }
}
