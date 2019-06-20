using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActorStateUtility;
using ActorCommanderUtility;
using System;
using UniRx;

/*
namespace ActorCommanderUtility
{
    /// <summary>
    /// 入力に対し、意味のある規則性（※）を判定し取得するメソッドを提供するクラス
    /// ※例えばボタンを押した瞬間、離した瞬間、複数のボタンを同時押しした瞬間、連打、レバガチャなど　入力表現と名付ける
    /// </summary>
    /// <typeparam name="InputType">対応する入力の型 bool,Vector2など</typeparam>
    public abstract class InputExpressions<InputType>
    {
        protected InputType eval;

        /// <summary>
        /// TODO:これを呼ぶ前にEvaluate()を呼び出す
        /// </summary>
        public abstract void Update();
        public void Evaluate(InputType value) { eval = value; }

        public InputType Evaluation { get => eval; }
    }

    /// <summary>
    /// bool型で表される入力表現のクラス
    /// これ自体はボタンを連想するとよい
    /// </summary>
    public class BoolExpressions : InputExpressions<bool>
    {
        public class LongPushClock
        {
            BoolExpressions subject;
            float thresholdTime;
            float pushedTime;
            float lastPushedTime;

            public float PushedTime { get => pushedTime;}
            public bool IsLongPushedUp { get => lastPushedTime > thresholdTime; }
            public bool IsLongPushed { get => pushedTime > thresholdTime; }

            public LongPushClock(BoolExpressions subject,float thresholdTime)
            {
                this.subject = subject;
                pushedTime = 0;
                lastPushedTime = 0;
                this.thresholdTime = thresholdTime;
            }

            public void Update()
            {
                lastPushedTime = subject.IsUp ? pushedTime : 0;
                pushedTime = subject.eval ? pushedTime + Time.deltaTime : 0;
            }

            public void Reset()
            {
                pushedTime = 0;
                lastPushedTime = 0;
            }
        }

        protected float pushedTime = 0;
        protected bool isDown = false;
        protected bool isUp = false;


        public bool IsDown { get => isDown; }
        public bool IsUp { get => isUp; }
        public float PushedTime { get => pushedTime; }

        public override void Update()
        {
            isDown = eval && pushedTime == 0;
            isUp = !eval && pushedTime > 0;
            pushedTime = eval ? pushedTime + Time.deltaTime : 0;
        }

        public LongPushClock GetLongPushClock(float thresholdTime) => new LongPushClock(this, thresholdTime);

    }

    /// <summary>
    /// Vector2型で表される入力表現のクラス
    /// これ自体はジョイスティックを連想するとよい
    /// </summary>
    public class AnalogueExpressions : InputExpressions<Vector2>
    {
        public override void Update() { eval = eval.normalized; }

        public Vector2 Analogue { get => eval; }
        
    }

    /// <summary>
    /// ボタン同時押しの入力表現のクラス
    /// ボタンと同様に押し始め、押し終わりを判定できる
    /// 押し始め：判定対象の全てのBoolExpressionsの評価が判定時間以内に真になり、うち1つ以上が押し始め(Down)で他は真が続行中
    /// 押し終わり：同時押し継続中、判定対象の全てのBoolExpressionsのうち1つの評価が偽に転じた(Up)
    /// 注：このクラスのEvaluate()は使わない
    /// </summary>
    public class MultiPushExpressions : BoolExpressions
    {
        //図解
        //
        // -:non pushed
        // |:pushed
        // threshold : 1 sec
        //
        // time : 0   1   2   3   4   5   6   7   8   sec
        // in 1 : ----|||||||||------|||||||||||-----
        // in 2 : -------|||||||||||||||-------------
        // in 3 : ------|||||||||||||||||||----------
        // in 4 : ------||||||||-----|||||||||||-----
        // down :        A
        // up   :              A

        BoolExpressions[] boolInputs;
        readonly float timeThreshold;

        public MultiPushExpressions(float timeThreshold,params BoolExpressions[] boolInput)
        {
            eval = false;
            boolInputs = boolInput;
            this.timeThreshold = timeThreshold;
        }

        public override void Update()
        {
            bool allArePushed = true;
            bool allAreEarly = true;
            bool anyIsDown = false;
            for(int i=0; i < boolInputs.Length; ++i)
            {
                BoolExpressions bei = boolInputs[i];

                allArePushed = allArePushed && bei.Evaluation;
                allAreEarly = allAreEarly && bei.PushedTime < timeThreshold;
                anyIsDown = anyIsDown || bei.IsDown;
            }
            isDown = allArePushed && allAreEarly && anyIsDown;
            isUp = eval && !allArePushed;
            eval = isDown || (eval && allArePushed);

            pushedTime = eval ? pushedTime + Time.deltaTime : 0;
        }
    }
}
*/
namespace ActorSarah
{

    public class ActorStateConnectorSarah : ActorState.ActorStateConnector
    {
        [SerializeField] PlayerCommander commands;
        [SerializeField] GroundSensor groundSensor;
        
        [SerializeField] SarahDefault sarahDefault;
        [SerializeField] VerticalSlash verticalSlash;
        [SerializeField] ReturnSmash returnSlash;
        [SerializeField] SmashSlash smashSlash;
        [SerializeField] AerialSlash aerialSlash;
        [SerializeField] Guard guard;
        [SerializeField] Tackle tackle;
        [SerializeField] RisingAttack risingAttack;
        [SerializeField] DropAttack dropAttack;
        [SerializeField] Glide glide;

        ChainAttackStream tripleSlashAttackStream;

        public override ActorState DefaultState => sarahDefault;

        protected override void BuildStateConnection()
        {
            Func<ActorState> proceedFunc;
            ConnectStateFromDefault(
                proceedFunc = 
                (tripleSlashAttackStream = new ChainAttackStream(.4f, new ActorState[] { verticalSlash, returnSlash, smashSlash }))
                .ProceedsWhen(() => groundSensor.IsOnGround && commands.Attack.IsDown));
            ConnectStateFromDefault(
                () => !groundSensor.IsOnGround && commands.Attack.IsDown,
                aerialSlash);

            ConnectState(proceedFunc, verticalSlash);
            ConnectState(tripleSlashAttackStream.ProceedsWhen(() => (groundSensor.IsOnGround || tripleSlashAttackStream.IsRecepting) && commands.Attack.IsDown), returnSlash);

            ConnectStateFromDefault(
                () => groundSensor.IsOnGround && commands.OpenUmbrella.IsDown,
                guard);
            ConnectState(
                () => groundSensor.IsOnGround && commands.Attack.IsDown, 
                guard,
                tackle);
            ConnectStateFromDefault(
                () => !groundSensor.IsOnGround && commands.OpenUmbrella.IsDown,
                glide);
            ConnectState(
                () => !groundSensor.IsOnGround && commands.Attack.IsDown, 
                glide,
                risingAttack);
            
        }

        protected override void BeforeStateUpdate()
        {
            commands.Decide();
        }
        protected override void OnChangeState(ActorState next, bool isNormalTermination)
        {
            if (!isNormalTermination) { tripleSlashAttackStream.Reset(); }
        }

        [System.Serializable]
        public class SarahDefault : Default
        {
            [DisabledField] public PlayerCommander commands;
            [SerializeField] ActorFunction.HorizontalMove horizontalMove;
            [SerializeField] ActorFunction.Jump jump;
            public ChainAttackStream attackStream;
            ActorStateConnectorSarah connectorSarah;

            public ActorStateConnectorSarah ConnectorSarah
            {
                get => connectorSarah == null ?
                    (connectorSarah = Connector as ActorStateConnectorSarah) :
                    connectorSarah;
            }

            protected override void OnInitialize()
            {
                Debug.Log("State Changed : Default");
                ConnectorSarah.tripleSlashAttackStream.StartReception();
            }
            public override void OnActive()
            {
                horizontalMove.Update(System.Math.Sign(commands.Directional.Evaluation.x));
                jump.Update(commands.Jump.Evaluation);
            }
        }

        [System.Serializable]
        private class SarahState : ActorState
        {
            ActorStateConnectorSarah connectorSarah;

            public ActorStateConnectorSarah ConnectorSarah
            {
                get => connectorSarah == null ?
                    (connectorSarah = Connector as ActorStateConnectorSarah) :
                    connectorSarah;
            }

            [DisabledField] public PlayerCommander commands;
        }

        [System.Serializable]
        private class VerticalSlash : SarahState
        {
            [SerializeField] AttackInHitbox verticalSlashAttack;
            [SerializeField] float receptionStartTime;
            [SerializeField] Umbrella umbrella;
            IodoShiba.Utilities.ManualClock receptionStartClock = new IodoShiba.Utilities.ManualClock();

            protected override bool ShouldCotinue() => verticalSlashAttack.IsAttackActive;
            protected override void OnInitialize()
            {
                receptionStartClock.Reset();
                Debug.Log("State Changed : Vertical Slash");
                verticalSlashAttack.Activate();
                umbrella.StartMotion("Player"+nameof(VerticalSlash));
            }

            public override void OnActive()
            {
                receptionStartClock.Update();
                if(!ConnectorSarah.tripleSlashAttackStream.IsRecepting && receptionStartClock.Clock > receptionStartTime)
                {
                    ConnectorSarah.tripleSlashAttackStream.StartReception();
                }
            }
            protected override void OnTerminate(bool isNormal)
            {
                receptionStartClock.Reset();
                umbrella.Default();
                verticalSlashAttack.Inactivate();
            }
        }

        [System.Serializable]
        private class ReturnSmash : SarahState
        {
            [SerializeField] AttackInHitbox retuenSlashAttack;
            [SerializeField] float receptionStartTime;
            [SerializeField] Vector2 jumpUpImpulse;
            [SerializeField] Rigidbody2D selfRigidBody;
            [SerializeField] Umbrella umbrella;
            IodoShiba.Utilities.ManualClock receptionStartClock = new IodoShiba.Utilities.ManualClock();

            protected override bool ShouldCotinue() => retuenSlashAttack.IsAttackActive;
            protected override void OnInitialize()
            {
                Debug.Log("State Changed : Return Slash");
                retuenSlashAttack.Activate();
                selfRigidBody.AddForce(jumpUpImpulse, ForceMode2D.Impulse);
                umbrella.StartMotion("Player" + nameof(ReturnSlash));
            }
            public override void OnActive()
            {
                receptionStartClock.Update();
                if (!ConnectorSarah.tripleSlashAttackStream.IsRecepting && receptionStartClock.Clock > receptionStartTime)
                {
                    ConnectorSarah.tripleSlashAttackStream.StartReception();
                }
            }
            protected override void OnTerminate(bool isNormal)
            {
                receptionStartClock.Reset();
                umbrella.Default();
                retuenSlashAttack.Inactivate();
            }
        }

        [System.Serializable]
        private class SmashSlash : SarahState
        {
            [SerializeField] AttackInHitbox smashSlashAttack;
            [SerializeField] Umbrella umbrella;

            protected override bool ShouldCotinue() => smashSlashAttack.IsAttackActive;
            protected override void OnInitialize()
            {
                Debug.Log("State Changed : Smash Slash");
                smashSlashAttack.Activate();
                umbrella.StartMotion("Player" + nameof(SmashSlash));
            }
            protected override void OnTerminate(bool isNormal)
            {
                umbrella.Default();
                smashSlashAttack.Inactivate();
            }
        }

        [System.Serializable]
        private class AerialSlash : SarahState
        {
            [SerializeField] AttackInHitbox aerialSlashAttack;
            [SerializeField] Umbrella umbrella;
            [SerializeField] GroundSensor groundSensor;

            protected override bool ShouldCotinue() => aerialSlashAttack.IsAttackActive && !groundSensor.IsOnGround;

            protected override void OnInitialize()
            {
                Debug.Log("State Changed : Aerial Slash");
                aerialSlashAttack.Activate();
                umbrella.StartMotion("Player" + nameof(AerialSlash));
            }

            protected override void OnTerminate(bool isNormal)
            {
                umbrella.Default();
                aerialSlashAttack.Inactivate();
            }
        }

        [System.Serializable]
        private class Guard : SarahState
        {

        }

        [System.Serializable]
        private class Tackle : SarahState
        {

        }

        [System.Serializable]
        private class RisingAttack : SarahState
        {

        }

        [System.Serializable]
        private class DropAttack : SarahState
        {

        }

        [System.Serializable]
        private class Glide : SarahState
        {

        }
    }

}