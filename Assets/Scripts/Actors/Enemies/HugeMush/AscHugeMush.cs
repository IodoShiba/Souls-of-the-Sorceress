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
            [SerializeField] float initialNoSummonTime;
            [SerializeField] ActorFunction.HorizontalMove horizontalMove;
            [SerializeField] ActorFunction.Summon summon;
            [SerializeField] ActorFunction.Directionable directionable;
            IodoShibaUtil.ManualUpdateClass.ManualClock clockToSummonable = new IodoShibaUtil.ManualUpdateClass.ManualClock();

            AscHugeMush connectorHugeMush;
            AscHugeMush ConnectorHugeMush { get => connectorHugeMush == null ? (connectorHugeMush = Connector as AscHugeMush) : connectorHugeMush; }

            protected override void OnInitialize()
            {
                clockToSummonable.Reset();
            }
            protected override void OnActive()
            {
                horizontalMove.ManualUpdate(ConnectorHugeMush.ai.MoveSign);
                summon.ManualUpdate(clockToSummonable.Clock > initialNoSummonTime ? ConnectorHugeMush.ai.DoSummon : false);
                directionable.CurrentDirection = (ActorFunction.Directionable.Direction)ConnectorHugeMush.ai.MoveSign;

                clockToSummonable.Update();
            }
        }
    }
}