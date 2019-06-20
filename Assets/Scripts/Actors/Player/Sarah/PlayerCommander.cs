using UnityEngine;
using System.Collections;
using ActorCommanderUtility;

namespace ActorSarah
{
    public abstract class PlayerCommander : MonoBehaviour
    {

        private AnalogueExpressions directional = new AnalogueExpressions();
        private BoolExpressions attack = new BoolExpressions();
        private BoolExpressions jump = new BoolExpressions();
        private BoolExpressions openUmbrella = new BoolExpressions();
        private BoolExpressions awakeButton = new BoolExpressions();
        private BoolExpressions analogueUp = new BoolExpressions();
        private BoolExpressions analogueDown = new BoolExpressions();
        private MultiPushExpressions downAttackMultiPush;

        private void Awake()
        {
            downAttackMultiPush = new MultiPushExpressions(.2f, analogueDown, attack);
        }
        public AnalogueExpressions Directional { get => directional; }
        public BoolExpressions Attack { get => attack; }
        public BoolExpressions Jump { get => jump; }
        public BoolExpressions OpenUmbrella { get => openUmbrella; }
        public BoolExpressions AwakeButton { get => awakeButton; }
        public BoolExpressions AnalogueUp { get => analogueUp; }
        public BoolExpressions AnalogueDown { get => analogueDown; }
        public MultiPushExpressions DownAttackMultiPush { get => downAttackMultiPush; }

        public abstract void Decide();

    }
}