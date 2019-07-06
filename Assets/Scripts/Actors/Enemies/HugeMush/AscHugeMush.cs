using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorHugeMashroom
{
    public class AscHugeMush : FightActorStateConector
    {
        [SerializeField] HugeMashAI ai;

        [SerializeField] HugeMushDefault hugeMushDefault;
        [SerializeField] SmashedState smashed;
        public override SmashedState Smashed => smashed;

        public override ActorState DefaultState => hugeMushDefault;

        protected override void BeforeStateUpdate()
        {
            ai.Decide();
        }

        [System.Serializable]
        class HugeMushDefault : DefaultState
        {
            [SerializeField] ActorFunction.HorizontalMove horizontalMove;
            [SerializeField] ActorFunction.Summon summon;

            AscHugeMush connectorHugeMush;
            AscHugeMush ConnectorHugeMush { get => connectorHugeMush == null ? (connectorHugeMush = Connector as AscHugeMush) : connectorHugeMush; }
            protected override void OnActive()
            {
                horizontalMove.ManualUpdate(ConnectorHugeMush.ai.MoveSign);
                summon.ManualUpdate(ConnectorHugeMush.ai.DoSummon);
            }
        }
    }
}