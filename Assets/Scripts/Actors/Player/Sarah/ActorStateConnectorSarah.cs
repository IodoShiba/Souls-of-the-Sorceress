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

    public class ActorStateConnectorSarah : FightActorStateConector//ActorState.ActorStateConnector
    {
        [SerializeField] PlayerCommander commands;
        [SerializeField] GroundSensor groundSensor;
        [SerializeField] ActorFunction.Directionable direction;
        [SerializeField, DisabledField] string currentStateName;

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

        [SerializeField] SarahSmashed smashed;

        ChainAttackStream tripleSlashAttackStream;

        public override ActorState DefaultState => sarahDefault;
        public override SmashedState Smashed => smashed;

        public bool isGuard { get => guard.IsCurrent; }
        
        protected Rigidbody2D selfRigidbody;
        protected Rigidbody2D SelfRigidbody { get => selfRigidbody == null ? (selfRigidbody = GetComponent<Rigidbody2D>()) : selfRigidbody; }
        protected override void BuildStateConnection()
        {
            Func<ActorState> proceedFunc;
            ConnectStateFromDefault(
                proceedFunc = 
                (tripleSlashAttackStream = new ChainAttackStream(.4f, new ActorState[] { verticalSlash, returnSlash, smashSlash }))
                .ProceedsWhen(() => groundSensor.IsOnGround && commands.Attack.IsDown));

            ConnectStateFromDefault(
                () => !groundSensor.IsOnGround && commands.DownAttackMultiPush.IsDown,
                dropAttack);

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
            currentStateName = Current.GetType().Name;
        }
        protected override void OnChangeState(ActorState next, bool isNormalTermination)
        {
            if (!isNormalTermination) { tripleSlashAttackStream.Reset(); }
            Debug.Log($"State has changed:{next.GetType().Name}");
        }


        [System.Serializable]
        public class SarahDefault : Default
        {
            [SerializeField,DisabledField] PlayerCommander commands;
            [SerializeField] ActorFunction.HorizontalMove horizontalMove;
            [SerializeField] ActorFunction.Jump jump;
            [SerializeField] ActorFunction.Directionable directionable;
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
                ConnectorSarah.tripleSlashAttackStream.StartReception();
                horizontalMove.Method.enabled = true;
            }
            protected override void OnActive()
            {
                horizontalMove.ManualUpdate(System.Math.Sign(commands.Directional.Evaluation.x));
                jump.Update(commands.Jump.Evaluation);
                if (commands.Directional.Evaluation.x * (int)directionable.CurrentDirection < 0)
                {
                    directionable.ChangeDirection(System.Math.Sign(commands.Directional.Evaluation.x));
                }
            }

            protected override void OnTerminate(bool isNormal)
            {
                horizontalMove.Method.enabled = false;
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

            [SerializeField,DisabledField] protected PlayerCommander commands;

        }

        [System.Serializable]
        private class VerticalSlash : SarahState
        {
            [SerializeField] AttackInHitbox verticalSlashAttack;
            [SerializeField] float receptionStartTime;
            [SerializeField] Umbrella umbrella;
            IodoShiba.ManualUpdateClass.ManualClock receptionStartClock = new IodoShiba.ManualUpdateClass.ManualClock();

            protected override bool ShouldCotinue() => verticalSlashAttack.IsAttackActive;
            protected override void OnInitialize()
            {
                receptionStartClock.Reset();
                verticalSlashAttack.Activate();
                umbrella.StartMotion("Player"+nameof(VerticalSlash));
            }

            protected override void OnActive()
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
            IodoShiba.ManualUpdateClass.ManualClock receptionStartClock = new IodoShiba.ManualUpdateClass.ManualClock();

            protected override bool ShouldCotinue() => retuenSlashAttack.IsAttackActive;
            protected override void OnInitialize()
            {
                retuenSlashAttack.Activate();
                ConnectorSarah.SelfRigidbody.AddForce(jumpUpImpulse, ForceMode2D.Impulse);
                umbrella.StartMotion("Player" + nameof(ReturnSlash));
            }
            protected override void OnActive()
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
            [SerializeField] Collider2D extraCollider;
            [SerializeField] Umbrella umbrella;
            [SerializeField] ActorFunction.Directionable direction;
            [SerializeField] ActorFunction.Guard guard;
            int dirSign;

            protected override bool ShouldCotinue() => commands.OpenUmbrella.Evaluation;

            protected override void OnInitialize()
            {
                extraCollider.enabled = true;
                ConnectorSarah.BearAgainstAttack = true;
                dirSign = (int)direction.CurrentDirection;
                guard.Fields.DegreeRangeStart = -dirSign * 90;
                guard.Fields.DegreeRangeWidth = 180;
                guard.Method.Activated = true;
                umbrella.PlayerGuard();
            }

            protected override void OnActive()
            {
                guard.ManualUpdate();
            }

            protected override void OnTerminate(bool isNormal)
            {
                extraCollider.enabled = false;
                ConnectorSarah.BearAgainstAttack = false;
                umbrella.Default();
                guard.Method.Activated = false;
            }

            public override bool IsResistibleTo(Type actorStateType) => guard.Method.IsSucceed && typeof(SmashedState).IsAssignableFrom(actorStateType);
        }

        [System.Serializable]
        private class Tackle : SarahState
        {
            [System.Serializable]
            class StateChangeEvent : UnityEngine.Events.UnityEvent<bool> { }

            [SerializeField] float distance;
            [SerializeField] float speed;
            [SerializeField] Umbrella umbrella;
            [SerializeField] AttackInHitbox attack;
            [SerializeField] ActorFunction.VelocityAdjuster velocityAdjuster;
            [SerializeField] ActorFunction.Directionable direction;
            [SerializeField] StateChangeEvent onChangeStateCallbacks;

            float x0;

            protected override bool ShouldCotinue()
            {
                if(Mathf.Abs(ConnectorSarah.SelfRigidbody.velocity.x ) < Mathf.Epsilon) { return false; }
                float x = GameObject.transform.position.x;
                return x0 - distance < x && x < x0 + distance;
            }
            protected override void OnInitialize()
            {
                attack.Activate();
                x0 = GameObject.transform.position.x;
                velocityAdjuster.Method.enabled = true;
                velocityAdjuster.Fields.Velocity = (int)direction.CurrentDirection * Mathf.Abs(velocityAdjuster.Fields.Velocity.x) * Vector2.right;
                umbrella.PlayerGuard();
                onChangeStateCallbacks.Invoke(true);

            }
            protected override void OnActive()
            {
                velocityAdjuster.ManualUpdate();
            }
            protected override void OnTerminate(bool isNormal)
            {
                attack.Inactivate();
                velocityAdjuster.Method.enabled = false;
                umbrella.Default();
                onChangeStateCallbacks.Invoke(false);
            }


        }

        [System.Serializable]
        private class Glide : SarahState
        {
            [SerializeField] float fallSpeed;
            [SerializeField] GroundSensor groundSensor;
            [SerializeField] Umbrella umbrella;
            [SerializeField] ActorFunction.VelocityAdjuster velocityAdjuster;
            [SerializeField] ActorFunction.HorizontalMove horizontalMove;
            [SerializeField] ActorFunction.Guard guard;
            protected override bool ShouldCotinue() => commands.OpenUmbrella.Evaluation && !groundSensor.IsOnGround;
            protected override void OnInitialize()
            {
                umbrella.PlayerGliding();
                velocityAdjuster.Method.enabled = ConnectorSarah.SelfRigidbody.velocity.y <= velocityAdjuster.Fields.Velocity.y + Mathf.Epsilon;
                horizontalMove.Method.enabled = true;
                guard.Method.Activated = true;
            }

            protected override void OnActive()
            {
                horizontalMove.ManualUpdate(System.Math.Sign(commands.Directional.Evaluation.x));
                velocityAdjuster.ManualUpdate();
                velocityAdjuster.Method.enabled = ConnectorSarah.SelfRigidbody.velocity.y <= velocityAdjuster.Fields.Velocity.y + Mathf.Epsilon;
                guard.ManualUpdate();
            }

            protected override void OnTerminate(bool isNormal)
            {
                umbrella.Default();
                guard.Method.Activated = false;
                velocityAdjuster.Method.enabled = false;
                horizontalMove.Method.enabled = false;
            }

            public override bool IsResistibleTo(Type actorStateType) => guard.Method.IsSucceed && typeof(SmashedState).IsAssignableFrom(actorStateType);
        }

        [System.Serializable]
        private class RisingAttack : SarahState
        {
            [SerializeField] float riseHeight;
            [SerializeField] float abilityTime = 0;
            [SerializeField] AttackInHitbox attack;
            [SerializeField] ActorFunction.VelocityAdjuster velocityAdjuster;
            [SerializeField] Umbrella umbrella;
            float limitHeight;

            IodoShiba.ManualUpdateClass.ManualClock clock = new IodoShiba.ManualUpdateClass.ManualClock();

            protected override bool ShouldCotinue() => clock.Clock < abilityTime;
            protected override void OnInitialize()
            {
                attack.Activate();
                limitHeight = GameObject.transform.position.y + riseHeight;
                velocityAdjuster.Method.enabled = true;
                umbrella.PlayerRisingAttack();
                clock.Reset();
            }
            protected override void OnActive()
            {
                if(GameObject.transform.position.y > limitHeight && velocityAdjuster.Method.enabled)
                {
                    ConnectorSarah.SelfRigidbody.velocity = Vector2.right * ConnectorSarah.SelfRigidbody.velocity.x;
                    velocityAdjuster.Method.enabled = false;
                }
                velocityAdjuster.ManualUpdate();

                clock.Update();
            }
            protected override void OnTerminate(bool isNormal)
            {
                attack.Inactivate();
                velocityAdjuster.Method.enabled = false;
                umbrella.Default();
                clock.Reset();
            }
        }

        [System.Serializable]
        private class DropAttack : SarahState
        {
            [SerializeField] float abilityTime;
            [SerializeField] AttackInHitbox attack;
            [SerializeField] ActorFunction.VelocityAdjuster velocityAdjuster;
            [SerializeField] GroundSensor groundSensor;
            [SerializeField] Umbrella umbrella;
            IodoShiba.ManualUpdateClass.ManualClock clock = new IodoShiba.ManualUpdateClass.ManualClock();

            protected override bool ShouldCotinue() => !groundSensor.IsOnGround && clock.Clock < abilityTime;
            protected override void OnInitialize()
            {
                attack.Activate();
                velocityAdjuster.Method.enabled = true;
                umbrella.PlayerDropAttack();
                clock.Reset();
            }

            protected override void OnActive()
            {
                velocityAdjuster.ManualUpdate();
                clock.Update();
            }
            protected override void OnTerminate(bool isNormal)
            {
                attack.Inactivate();
                velocityAdjuster.Method.enabled = false;
                umbrella.Default();
                clock.Reset();
            }
        }

        [System.Serializable]
        private class SarahSmashed : SmashedState
        {

        }
    }

}