using UnityEngine;
using System.Collections;
using ActorCommanderUtility;

namespace ActorSarah
{
    public abstract class PlayerCommander : MonoBehaviour
    {
        [SerializeField] float attackLongPushThreshold;

        private AnalogueExpressions directional = new AnalogueExpressions();
        private BoolExpressions attack = new BoolExpressions();
        private BoolExpressions jump = new BoolExpressions();
        private BoolExpressions openUmbrella = new BoolExpressions();
        private BoolExpressions awakeButton = new BoolExpressions();
        private BoolExpressions analogueUp = new BoolExpressions();
        private BoolExpressions analogueDown = new BoolExpressions();
        private MultiPushExpressions downAttackMultiPush;
        private MultiPushExpressions upAttackMultiPush;

        private void Awake()
        {
            downAttackMultiPush = new MultiPushExpressions(.2f, analogueDown, attack);
            upAttackMultiPush= new MultiPushExpressions(.2f, analogueUp, attack);
        }
        public AnalogueExpressions Directional { get => directional; }
        public BoolExpressions Attack { get => attack; }
        public BoolExpressions Jump { get => jump; }
        public BoolExpressions OpenUmbrella { get => openUmbrella; }
        public BoolExpressions AwakeButton { get => awakeButton; }
        public BoolExpressions AnalogueUp { get => analogueUp; }
        public BoolExpressions AnalogueDown { get => analogueDown; }
        public MultiPushExpressions DownAttackMultiPush { get => downAttackMultiPush; }
        public MultiPushExpressions UpAttackMultiPush { get => upAttackMultiPush; }

        public void Decide()
        {
            DecideOverride();
            ManualUpdate();
        }

        public abstract void DecideOverride();
        protected void ManualUpdate()
        {
            Directional.Update();
            Attack.Update();
            Jump.Update();
            OpenUmbrella.Update();
            AwakeButton.Update();
            AnalogueUp.Update();
            AnalogueDown.Update();
            DownAttackMultiPush.Update();
            UpAttackMultiPush.Update();
        }
    }
}