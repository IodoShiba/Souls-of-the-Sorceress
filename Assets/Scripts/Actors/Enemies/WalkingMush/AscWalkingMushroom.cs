using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorWalkingMushroom
{
    public class AscWalkingMushroom : FightActorStateConector
    {
        [SerializeField] WalkingMushAI ai;
        [SerializeField] ActorFunction.Directionable direction;

        [SerializeField] MushDefault mushDefault;
        [SerializeField] MushSmashed smashed;

        public override SmashedState Smashed => smashed;

        public override ActorState DefaultState => mushDefault;
        protected override void BeforeStateUpdate()
        {
            ai.Decide();
        }

        [System.Serializable]
        private class MushDefault : DefaultState
        {
            AscWalkingMushroom connectorMush;
            AscWalkingMushroom ConnectorMush { get => connectorMush == null ? (connectorMush = Connector as AscWalkingMushroom) : connectorMush; }

            [SerializeField] ActorFunction.HorizontalMove horizontalMove;

            protected override void OnInitialize()
            {
                horizontalMove.Method.enabled = true;
            }
            protected override void OnActive()
            {
                if(ConnectorMush.ai.MoveSign * (int)ConnectorMush.direction.CurrentDirection < 0)
                {
                    ConnectorMush.direction.ChangeDirection(ConnectorMush.ai.MoveSign);
                }
                horizontalMove.ManualUpdate(ConnectorMush.ai.MoveSign);
                
            }
            protected override void OnTerminate(bool isNormalTermination)
            {
                horizontalMove.Method.enabled = false;
            }
        }

        [System.Serializable]
        private class MushSmashed : SmashedState
        {

        }
    }
}
