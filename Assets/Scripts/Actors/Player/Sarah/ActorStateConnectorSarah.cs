using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActorStateUtility;
using ActorCommanderUtility;
using System;
using UniRx;
using DG.Tweening;

namespace ActorSarah
{

    public class ActorStateConnectorSarah : FightActorStateConector,SaveData.IPlayerProgressLevelCareer
    {

        [Serializable]
        class AwakeMutableShootObject : AwakeMutable<ActorFunction.ShootObject> { }

        [SerializeField] float attackLongPushThreshold;
        [SerializeField] int durabilityCostOnShootMagic;
        [SerializeField] float proceedImpulseOnAttack;
        [SerializeField, Range(0, 4)] int progressLevel;
        [SerializeField, Range(0, 4)] int reqiredProgressLevelToShootMagic;
        [SerializeField] UnityEngine.Events.UnityEvent beforeStateChange;
        [SerializeField] AwakeMutableShootObject shootObject;
        [SerializeField] PlayerCommander commands;
        [SerializeField] GroundSensor groundSensor;
        [SerializeField] ActorFunction.Directionable direction;
        [SerializeField] ActionAwake actionAwake;
        [SerializeField] UmbrellaParameters umbrellaParameters;
        [SerializeField, DisabledField] string currentStateName;
        [SerializeField, DisabledField] float lpt;
        [SerializeField] public Animator sarahAnimator;

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
        [SerializeField] SarahDead dead;

        ChainAttackStream tripleSlashAttackStream;
        ActorState formerState;
        ActorCommanderUtility.BoolExpressions.LongPushClock attackLongPushClock;
        HashSet<ActorState> magicChargeRegion;

        public override ActorState DefaultState => sarahDefault;
        public override SmashedState Smashed => smashed;
        public override DeadState Dead => dead;
        public bool isGuard { get => guard.IsCurrent; }

        
        protected Rigidbody2D selfRigidbody;
        protected Rigidbody2D SelfRigidbody { get => selfRigidbody == null ? (selfRigidbody = GetComponent<Rigidbody2D>()) : selfRigidbody; }
        public int ProgressLevel { get => progressLevel; }

        protected override void Awake()
        {
            attackLongPushClock = commands.Attack.GetLongPushClock(attackLongPushThreshold);
            magicChargeRegion = new HashSet<ActorState> { sarahDefault, verticalSlash, aerialSlash };
            base.Awake();
        }
        protected override void BuildStateConnection()
        {

            ConnectStateFromDefault(
                () => !groundSensor.IsOnGround && commands.AnalogueDown.Evaluation && commands.Attack.IsDown,//commands.DownAttackMultiPush.IsDown,
                dropAttack);

            ConnectStateFromDefault(
                () => groundSensor.IsOnGround && commands.UpAttackMultiPush.IsDown,//commands.AnalogueUp.Evaluation && commands.Attack.IsDown,
                risingAttack);

            ConnectStateFromDefault(
                () => !groundSensor.IsOnGround && (commands.Attack.IsDown || (tripleSlashAttackStream.NextIndex() == 0 && attackLongPushClock.FinallyPushedTime > 0)),
                aerialSlash);

            Func<ActorState> proceedFunc;
            ConnectStateFromDefault(
                proceedFunc =
                (tripleSlashAttackStream = new ChainAttackStream(.4f, true, new ActorState[] { verticalSlash, returnSlash, smashSlash }))
                .ProceedsWhen(() => groundSensor.IsOnGround &&
                                    (commands.Attack.IsDown || (tripleSlashAttackStream.NextIndex() == 0 && attackLongPushClock.FinallyPushedTime > 0)) &&
                                    !sarahDefault.currentState.Equals(SarahDefault.StateInDefaultNum.IsInAir) &&
                                    !sarahDefault.currentState.Equals(SarahDefault.StateInDefaultNum.IsOnLanding)
                             )
                );
            ConnectState(proceedFunc, verticalSlash);
            ConnectState(tripleSlashAttackStream.ProceedsWhen(
                () => (groundSensor.IsOnGround || tripleSlashAttackStream.IsRecepting) && commands.Attack.IsDown),
                returnSlash);

            ConnectStateFromDefault(
                () => groundSensor.IsOnGround && commands.OpenUmbrella.Evaluation,
                guard);
            ConnectState(
                () => groundSensor.IsOnGround && commands.Attack.IsDown, 
                guard,
                tackle);
            ConnectState(() => !tackle.ContinueCond() && commands.OpenUmbrella.Evaluation, tackle, guard);
            ConnectStateFromDefault(
                () => !groundSensor.IsOnGround && commands.OpenUmbrella.Evaluation,
                glide);
            ConnectState(
                () => !groundSensor.IsOnGround && commands.Attack.IsDown, 
                glide,
                risingAttack);
            ConnectState(() => !risingAttack.ContinueCond() && commands.OpenUmbrella.Evaluation, risingAttack, glide);

            sarahDefault.attackLongPushClock = verticalSlash.attackLongPushClock = aerialSlash.attackLongPushClock = attackLongPushClock;

        }

        protected override void BeforeStateUpdate()
        {
            commands.Decide();
            attackLongPushClock.Update();
            if (commands.AwakeButton.IsDown)
            {
                actionAwake.SwitchActivate();
            }

            lpt = attackLongPushClock.PushedTime;
            currentStateName = Current.GetType().Name;
        }
        protected override void BeforeChangeState(ActorState next, bool isNormalTermination)
        {
            if (!isNormalTermination) { tripleSlashAttackStream.Reset(); }
            if (!magicChargeRegion.Contains(next))
            {
                attackLongPushClock.Reset();
            }
            beforeStateChange.Invoke();
            //Debug.Log($"State has changed:{next.GetType().Name}");
        }

        void TryShootMagic()
        {
            if (attackLongPushClock.IsLongPushedUp)
            {
                if (reqiredProgressLevelToShootMagic <= progressLevel && umbrellaParameters.TryConsumeDurability(durabilityCostOnShootMagic) > 0)
                {
                    shootObject.Content.Fields.InitialVelocity = (int)direction.CurrentDirection * Mathf.Abs(shootObject.Content.Fields.InitialVelocity.x) * Vector2.right;
                    shootObject.Content.Fields.RelativePosition = new Vector2((int)direction.CurrentDirection * Mathf.Abs(shootObject.Content.Fields.RelativePosition.x), shootObject.Content.Fields.RelativePosition.y);
                    shootObject.Content.ManualUpdate();
                    shootObject.Content.Method.Use();
                }
                attackLongPushClock.Reset();
            }

        }

        public void Restore(int data)
        {
            progressLevel = data;
        }

        public void Store(SaveData target, Action<int> setter)
        {
            setter(progressLevel);
        }

        [System.Serializable]
        public class SarahDefault : DefaultState
        {

            [SerializeField] private float umbrellaRecoverCycle;
            [SerializeField] private int umbrellaRecoverAmount;
            [SerializeField,DisabledField] PlayerCommander commands;
            [SerializeField] ActorFunction.HorizontalMove horizontalMove;
            [SerializeField] ActorFunction.Jump jump;
            [SerializeField] ActorFunction.Directionable directionable;
            [SerializeField] PassPlatform passPlatform;
            [SerializeField] GroundSensor groundSensor, groundSensorForOnLanding;
            [SerializeField] AudioSource runningAudioSource;
            [SerializeField] AudioClip runningClip;
            [DisabledField] public StateInDefaultNum currentState;
            //[SerializeField] UmbrellaParameters umbrellaParameters;
            public ChainAttackStream attackStream;
            ActorStateConnectorSarah connectorSarah;
            public BoolExpressions.LongPushClock attackLongPushClock;
            Animator sarahAnimator;

            bool IsInterruptJump;

            bool isRunning = false;
            bool IsRunning {
                set
                {
                    if(!isRunning && value)
                    {
                        runningAudioSource.clip = runningClip;
                        runningAudioSource.Play();
                    }
                    if (isRunning && !value)
                    {
                        runningAudioSource.clip = null;
                        runningAudioSource.Play();
                    }
                    isRunning = value;
                }
            }

            public ActorStateConnectorSarah ConnectorSarah
            {
                get => connectorSarah == null ?
                    (connectorSarah = Connector as ActorStateConnectorSarah) :
                    connectorSarah;
            }

            protected override void OnInitialize()
            {
                ConnectorSarah.tripleSlashAttackStream.StartReception();
                horizontalMove.Use = true;
                attackLongPushClock.AllowedToStartCount = true;
                ConnectorSarah.umbrellaParameters.ChangeDurabilityGradually(umbrellaRecoverCycle, umbrellaRecoverAmount);
                sarahAnimator = ConnectorSarah.sarahAnimator;
                IsInterruptJump = false;
                sarahAnimator.SetTrigger("DefaultTrigger");
                ResetDefaultStateTriggers();
                currentState = StateInDefaultNum.IsWaiting;
                if (sarahAnimator.GetBool("InterruptToInAir"))
                {
                    sarahAnimator.SetBool("InterruptToInAir",false);
                    currentState = StateInDefaultNum.IsInAir;
                    sarahAnimator.SetTrigger("InAirTrigger");
                }
                IsRunning = false;

            }
            protected override void OnActive()
            {
                int inSign = System.Math.Sign(commands.Directional.Evaluation.x);
                if (inSign!=0)
                {
                    ConnectorSarah.tripleSlashAttackStream.Reset();
                }
                horizontalMove.ManualUpdate(inSign);
                jump.Update(commands.Jump.Evaluation);
                if (commands.Directional.Evaluation.x * (int)directionable.CurrentDirection < 0)
                {
                    directionable.ChangeDirection(System.Math.Sign(commands.Directional.Evaluation.x));
                }
                passPlatform.Use(commands.AnalogueDown.Evaluation);
                IsRunning = groundSensor.IsOnGround && jump.Method.Activatable && horizontalMove.Method.IsMoving;

                StateInDefaultJudge();

            }

            protected override void OnTerminate(bool isNormal)
            {
                horizontalMove.Use = false;
                attackLongPushClock.AllowedToStartCount = false;
                ConnectorSarah.umbrellaParameters.StopChangeDurabilityGradually();
                passPlatform.Use(false);
                ResetDefaultStateTriggers();
                sarahAnimator.ResetTrigger("DefaultTrigger");
                IsRunning = false;
            }

            void StateInDefaultJudge()
            {
                StateInDefaultNum nextState = JudgeNextStateInDefault();
                //Debug.Log(currentState);
                //AnimatorStateInfo stateInfo = sarahAnimator.GetCurrentAnimatorStateInfo(0);
                //Debug.Log(stateInfo.fullPathHash);
                if (nextState != currentState)
                {
                    ResetDefaultStateTriggers();
                    switch (nextState)
                    {
                        case StateInDefaultNum.IsWaiting:
                            //Debug.Log("WaitingTrigger");
                            sarahAnimator.SetTrigger("WaitingTrigger");
                            break;
                        case StateInDefaultNum.IsRunning:
                            //Debug.Log("RunningTrigger");
                            sarahAnimator.SetTrigger("RunningTrigger");
                            break;
                        case StateInDefaultNum.IsJumping:
                            //Debug.Log("JumpingTrigger");
                            sarahAnimator.SetTrigger("JumpingTrigger");
                            break;
                        case StateInDefaultNum.IsInAir:
                            //Debug.Log("InAirTrigger");
                            sarahAnimator.SetTrigger("InAirTrigger");
                            break;
                        case StateInDefaultNum.IsOnLanding:
                            //Debug.Log("OnLandingTrigger");
                            sarahAnimator.SetTrigger("OnLandingTrigger");
                            break;
                    }
                    currentState = nextState;
                }
            }
            StateInDefaultNum JudgeNextStateInDefault()
            {
                AnimatorStateInfo stateInfo = sarahAnimator.GetCurrentAnimatorStateInfo(0);
                switch (currentState)
                {
                    case StateInDefaultNum.IsWaiting:
                        //Jumping遷移判定
                        if (jump.Method.IsActivated || IsInterruptJump) { IsInterruptJump = false; return StateInDefaultNum.IsJumping; }
                        //Running遷移判定
                        if (horizontalMove.Method.IsMoving && groundSensor.IsOnGround) { return StateInDefaultNum.IsRunning; }
                        return StateInDefaultNum.IsWaiting;
                    case StateInDefaultNum.IsRunning:
                        //Jumping遷移判定
                        if (jump.Method.IsActivated) { return StateInDefaultNum.IsJumping; }
                        //Waiting遷移判定
                        if (!horizontalMove.Method.IsMoving) { return StateInDefaultNum.IsWaiting; }
                        return StateInDefaultNum.IsRunning;
                    case StateInDefaultNum.IsJumping:
                        //InAir遷移判定
                        if (stateInfo.normalizedTime > 1 && !jump.Method.IsActive) { return StateInDefaultNum.IsInAir; }
                        return StateInDefaultNum.IsJumping;
                    case StateInDefaultNum.IsInAir:
                        //OnLanding遷移判定
                        if (groundSensorForOnLanding.IsOnGround) { return StateInDefaultNum.IsOnLanding; }
                        return StateInDefaultNum.IsInAir;
                    case StateInDefaultNum.IsOnLanding:
                        //割り込みIsJumpingのためのboolをセット
                        if (jump.Method.IsActivated) { IsInterruptJump = true; }
                        //IsWaiting遷移判定
                        if (stateInfo.normalizedTime > 1) {return StateInDefaultNum.IsWaiting; }
                        return StateInDefaultNum.IsOnLanding;
                }
                return StateInDefaultNum.IsWaiting;
            }

            void ResetDefaultStateTriggers()
            {
                string[] names = {"WaitingTrigger","RunningTrigger","JumpingTrigger","InAirTrigger","OnLandingTrigger"};
                foreach (string name in names)
                {
                    sarahAnimator.ResetTrigger(name);
                }
            }

            public enum StateInDefaultNum {IsWaiting = 0,IsRunning,IsJumping,IsInAir,IsOnLanding}

        }

        [System.Serializable]
        private class SarahState : ActorState
        {
            [SerializeField, Range(0, 4)] int requiredProgressLevel;
            [SerializeField, DisabledField] protected PlayerCommander commands;
            [SerializeField] protected UnityEngine.Events.UnityEvent onInitialize;
            ActorStateConnectorSarah connectorSarah;

            protected float proceedDistance = 1f;
            protected float proceedSpan = .3f;

            protected override bool IsAvailable() => requiredProgressLevel <= ConnectorSarah.progressLevel;

            public ActorStateConnectorSarah ConnectorSarah
            {
                get => connectorSarah == null ?
                    (connectorSarah = Connector as ActorStateConnectorSarah) :
                    connectorSarah;
            }

            protected override void OnInitialize()
            {
                onInitialize.Invoke();
                //Debug.Log("Player:" + this.GetType().Name);
            }

            protected void ProceedOnAttack()
            {
                //試験的な部分 方向キー入力中に斬撃入力で前進しながら攻撃
                float h;
                if ((System.Math.Abs(h = commands.Directional.Evaluation.x)) > 0.3)
                {
                    //GameObject.transform.position += System.Math.Sign(h) * Vector3.right * proceedDistance;
                    //GameObject.transform.DOMoveX(GameObject.transform.position.x + System.Math.Sign(h) * proceedDistance, proceedSpan).SetEase(Ease.OutExpo);

                    ConnectorSarah.SelfRigidbody.AddForce(System.Math.Sign(h) * Vector2.right * connectorSarah.proceedImpulseOnAttack, ForceMode2D.Impulse);
                }
                //試験的な部分 ここまで
            }
        }

        [System.Serializable]
        private class VerticalSlash : SarahState
        {
            [SerializeField] float receptionStartTime;
            [SerializeField] int amountConsumeUmbrellaDurability;
            [SerializeField] AttackInHitbox verticalSlashAttack;
            [SerializeField] ActorFunction.HorizontalMove horizontalMove;
            [SerializeField] Umbrella umbrella;

            IodoShibaUtil.ManualUpdateClass.ManualClock receptionStartClock = new IodoShibaUtil.ManualUpdateClass.ManualClock();
            public BoolExpressions.LongPushClock attackLongPushClock;
            
            protected override bool ShouldCotinue() => verticalSlashAttack.IsAttackActive;
            protected override void OnInitialize()
            {
                base.OnInitialize();

                receptionStartClock.Reset();
                verticalSlashAttack.Activate();

                //試験的な部分 方向キー入力中に斬撃入力で前進しながら攻撃
                ConnectorSarah.SelfRigidbody.velocity = Vector2.zero;
                ProceedOnAttack();
                //試験的な部分 ここまで

                umbrella.StartMotion("Player"+nameof(VerticalSlash));
                //Debug.Log(attackLongPushClock.IsLongPushedUp);
                horizontalMove.ManualUpdate();
                horizontalMove.Method.StopActorOnDisabled(.1f);
                ConnectorSarah.TryShootMagic();
                ConnectorSarah.sarahAnimator.SetTrigger("VerticalSlashTrigger");
                ConnectorSarah.sarahAnimator.ResetTrigger("DefaultTrigger");//これらは本質的でないがAnimatorControllerと内部Stateの同期ズレによるバグを防ぐ
                ConnectorSarah.sarahAnimator.ResetTrigger("JumpingTrigger");//
                ConnectorSarah.sarahAnimator.ResetTrigger("ReturnSmashTrigger");//
                ConnectorSarah.sarahAnimator.ResetTrigger("SmashSlashTrigger");//
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
            IodoShibaUtil.ManualUpdateClass.ManualClock receptionStartClock = new IodoShibaUtil.ManualUpdateClass.ManualClock();

            protected override bool ShouldCotinue() => retuenSlashAttack.IsAttackActive;
            protected override void OnInitialize()
            {
                base.OnInitialize();

                retuenSlashAttack.Activate();
                ConnectorSarah.SelfRigidbody.velocity = Vector2.zero;
                ConnectorSarah.SelfRigidbody.AddForce(jumpUpImpulse, ForceMode2D.Impulse);

                //試験的な部分 方向キー入力中に斬撃入力で前進しながら攻撃
                ProceedOnAttack();
                //試験的な部分 ここまで

                umbrella.StartMotion("Player" + nameof(ReturnSlash));
                ConnectorSarah.sarahAnimator.SetTrigger("ReturnSmashTrigger");
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
                base.OnInitialize();

                ConnectorSarah.SelfRigidbody.velocity = Vector2.up * ConnectorSarah.SelfRigidbody.velocity.y;
                smashSlashAttack.Activate();

                //試験的な部分 方向キー入力中に斬撃入力で前進しながら攻撃
                ProceedOnAttack();
                //試験的な部分 ここまで

                umbrella.StartMotion("Player" + nameof(SmashSlash));
                ConnectorSarah.sarahAnimator.SetTrigger("SmashSlashTrigger");
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
            [SerializeField] int amountConsumeUmbrellaDurability;
            [SerializeField] AttackInHitbox aerialSlashAttack;
            [SerializeField] Umbrella umbrella;
            [SerializeField] GroundSensor groundSensor;

            public BoolExpressions.LongPushClock attackLongPushClock;
            protected override bool ShouldCotinue() => aerialSlashAttack.IsAttackActive && !groundSensor.IsOnGround;

            protected override void OnInitialize()
            {
                base.OnInitialize();

                aerialSlashAttack.Activate();
                umbrella.StartMotion("Player" + nameof(AerialSlash));

                ConnectorSarah.TryShootMagic();
                ConnectorSarah.sarahAnimator.SetTrigger("AerialSlashTrigger");
                ConnectorSarah.sarahAnimator.SetBool("InterruptToInAir",false);
            }

            protected override void OnTerminate(bool isNormal)
            {
                //DecideNextStateInDefaultState
                ConnectorSarah.sarahAnimator.SetBool("InterruptToInAir",!groundSensor.IsOnGround);
                umbrella.Default();
                aerialSlashAttack.Inactivate();
            }
        }

        [System.Serializable]
        private class Guard : SarahState
        {
            [SerializeField] int amountConsumeUmbrellaDurability;
            [SerializeField] ActorFunction.Directionable direction;
            [SerializeField] ActorFunction.Guard guard;
            [SerializeField] Collider2D extraCollider;
            [SerializeField] ActorFunction.HorizontalMove horizontalMove;
            [SerializeField] Umbrella umbrella;
            int dirSign;

            protected override bool IsAvailable() => base.IsAvailable() && ConnectorSarah.umbrellaParameters.DoesUmbrellaWork();

            protected override bool ShouldCotinue() => commands.OpenUmbrella.Evaluation && ConnectorSarah.umbrellaParameters.DoesUmbrellaWork();

            protected override void OnInitialize()
            {
                ConnectorSarah.SelfRigidbody.velocity = Vector2.zero;
                extraCollider.enabled = true;
                ConnectorSarah.BearAgainstAttack = true;
                dirSign = (int)direction.CurrentDirection;
                guard.Fields.DegreeRangeStart = -dirSign * 90;
                guard.Fields.DegreeRangeWidth = 180;
                guard.Method.Activated = true;
                umbrella.PlayerGuard();
                guard.Method.GetIsAllSucceedAndReset();
                horizontalMove.ManualUpdate();
                horizontalMove.Method.StopActorOnDisabled(.1f);
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

            //MEMO:
            //攻撃がAttakcInHitbox.OnTriggerEnter2D()の呼び出しによってのみ行われる場合に限って
            //Mortal.OnTriedAttack(),ActorState.OnActive(),Mortal.ManualUpdate()の実行順はこの順であり、
            //Mortal.OnTriedAttack()の時点でガードの成否判定の処理が、
            //Mortal.ManualUpdate()の時点で被ダメージ状態への遷移を試行する処理が行われる。
            //ActorState.OnActive()内部でGuardMethod.IsAllSucceedをtrueにリセットしてしまうとガードの成否に拠らず
            //被ダメージ状態への遷移を拒否してしまう
            //実際にはMortal.TryAttack()がAttakcInHitbox.OnTriggerEnter2D()からのみ呼ばれるとは限らないが、
            //ガード状態維持判定時にGuardMethod.IsAllSucceedをリセットする処理を行うように変更すれば
            //いつMortal.TryAttack()が呼び出されてもそこからガード状態維持判定に至る間GuardMethod.IsAllSucceedがリセットされることはないだろう。
            public override bool IsResistibleTo(Type actorStateType)
            {
                bool guardAllSucceed = guard.Method.GetIsAllSucceedAndReset();
                if(guardAllSucceed)
                {
                    ConnectorSarah.umbrellaParameters.TryConsumeDurability(amountConsumeUmbrellaDurability);
                }

                return guardAllSucceed && typeof(SmashedState).IsAssignableFrom(actorStateType);
            }
        }

        [System.Serializable]
        private class Tackle : SarahState
        {
            [System.Serializable]
            class StateChangeEvent : UnityEngine.Events.UnityEvent<bool> { }

            [SerializeField] float distance;
            [SerializeField] float speed;
            [SerializeField] float degreeWidthOfGuardArea;
            [SerializeField] float unguardTime;
            [SerializeField] int amountConsumeUmbrellaDurability;
            [SerializeField] Umbrella umbrella;
            [SerializeField] AttackInHitbox attack;
            [SerializeField] ActorFunction.VelocityAdjuster velocityAdjuster;
            [SerializeField] ActorFunction.Directionable direction;
            [SerializeField] ActorFunction.Guard guard;
            [SerializeField] StateChangeEvent onChangeStateCallbacks;
            [SerializeField] GroundSensor wallSensor;

            IodoShibaUtil.ManualUpdateClass.ManualClock unguardClock = new IodoShibaUtil.ManualUpdateClass.ManualClock();
            float x0;
            int state = 0;
            float initialGravity = 0;

            protected override bool IsAvailable() => base.IsAvailable() && ConnectorSarah.umbrellaParameters.DoesUmbrellaWork();
            protected override bool ShouldCotinue()
            {
                //if(Mathf.Abs(ConnectorSarah.SelfRigidbody.velocity.x ) < Mathf.Epsilon) { return false; }
                //float x = GameObject.transform.position.x;
                //return x0 - distance < x && x < x0 + distance;
                return ContinueCond();
            }
            public bool ContinueCond() => state == 0 || unguardClock.Clock < unguardTime;

            protected override void OnInitialize()
            {
                ConnectorSarah.umbrellaParameters.TryConsumeDurability(amountConsumeUmbrellaDurability);
                attack.Activate();
                x0 = GameObject.transform.position.x;
                velocityAdjuster.Method.enabled = true;
                int dirSign = (int)direction.CurrentDirection;
                velocityAdjuster.Fields.Velocity = dirSign * speed * Vector2.right;
                initialGravity = ConnectorSarah.SelfRigidbody.gravityScale;
                ConnectorSarah.SelfRigidbody.gravityScale = 0;
                umbrella.PlayerGuard();
                onChangeStateCallbacks.Invoke(true);
                guard.Method.Activated = true;
                guard.Fields.DegreeRangeStart = (dirSign > 0 ? 0 : 180) - degreeWidthOfGuardArea / 2;
                guard.Fields.DegreeRangeWidth = degreeWidthOfGuardArea;
                unguardClock.Reset();
                state = 0;
            }
            protected override void OnActive()
            {
                velocityAdjuster.ManualUpdate();
                guard.ManualUpdate();
                if (state == 0 && 
                    (wallSensor.IsOnGround ||
                    Mathf.Abs(GameObject.transform.position.x - x0) > distance
                    ))
                {
                    state = 1;
                    guard.Method.Activated = false;
                    velocityAdjuster.Fields.Velocity = Vector2.zero;
                }
                if(state == 1)
                {
                    unguardClock.Update();
                }
            }
            protected override void OnTerminate(bool isNormal)
            {
                attack.Inactivate();
                //velocityAdjuster.Method.enabled = false;
                ConnectorSarah.SelfRigidbody.gravityScale = initialGravity;
                velocityAdjuster.Method.Disable();
                umbrella.Default();
                onChangeStateCallbacks.Invoke(false);
                guard.Method.Activated = false;
                unguardClock.Reset();
                state = 0;
            }

        }

        [System.Serializable]
        private class Glide : SarahState
        {
            [SerializeField] float fallSpeed;
            [SerializeField] int amountConsumeUmbrellaDurabilityOnGuard;
            [SerializeField] private float umbrellaConsumeCycle;
            [SerializeField] private int umbrellaConsumeAmount;
            [SerializeField] GroundSensor groundSensor;
            [SerializeField] Umbrella umbrella;
            [SerializeField] ActorFunction.VelocityAdjuster velocityAdjuster;
            [SerializeField] ActorFunction.HorizontalMove horizontalMove;
            [SerializeField] ActorFunction.Guard guard;

            protected override bool IsAvailable() => base.IsAvailable() && ConnectorSarah.umbrellaParameters.DoesUmbrellaWork();
            protected override bool ShouldCotinue() => commands.OpenUmbrella.Evaluation && !groundSensor.IsOnGround && ConnectorSarah.umbrellaParameters.DoesUmbrellaWork();
            protected override void OnInitialize()
            {
                umbrella.PlayerGliding();
                velocityAdjuster.Method.enabled = ConnectorSarah.SelfRigidbody.velocity.y <= velocityAdjuster.Fields.Velocity.y + Mathf.Epsilon;
                horizontalMove.Use = true;
                guard.Method.Activated = true;
                guard.Method.GetIsAllSucceedAndReset();
                ConnectorSarah.umbrellaParameters.ChangeDurabilityGradually(umbrellaConsumeCycle, -umbrellaConsumeAmount);
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
                //velocityAdjuster.Method.enabled = false;
                velocityAdjuster.Method.Disable();
                horizontalMove.Use = false;
                ConnectorSarah.umbrellaParameters.StopChangeDurabilityGradually();
            }

            public override bool IsResistibleTo(Type actorStateType)
            {
                bool guardAllSucceed = guard.Method.GetIsAllSucceedAndReset();
                if (guardAllSucceed)
                {
                    ConnectorSarah.umbrellaParameters.TryConsumeDurability(amountConsumeUmbrellaDurabilityOnGuard);
                }

                return guardAllSucceed && typeof(SmashedState).IsAssignableFrom(actorStateType);
            }
        }

        [System.Serializable]
        private class RisingAttack : SarahState
        {
            [SerializeField] float riseHeight;
            [SerializeField] float abilityTime = 0;
            [SerializeField] int amountConsumeUmbrellaDurability;
            [SerializeField] AttackInHitbox attack;
            [SerializeField] ActorFunction.VelocityAdjuster velocityAdjuster;
            [SerializeField] ActorFunction.Guard guard;
            [SerializeField] Umbrella umbrella;

            float limitHeight;
            IodoShibaUtil.ManualUpdateClass.ManualClock clock = new IodoShibaUtil.ManualUpdateClass.ManualClock();
            protected override bool IsAvailable() => base.IsAvailable() && ConnectorSarah.umbrellaParameters.DoesUmbrellaWork();

            protected override bool ShouldCotinue() => ContinueCond();
            public bool ContinueCond() => clock.Clock < abilityTime;
            protected override void OnInitialize()
            {
                attack.Activate();
                limitHeight = GameObject.transform.position.y + riseHeight;
                velocityAdjuster.Method.enabled = true;
                umbrella.PlayerRisingAttack();
                guard.Method.Activated = true;
                clock.Reset();
                ConnectorSarah.umbrellaParameters.TryConsumeDurability(amountConsumeUmbrellaDurability);
            }
            protected override void OnActive()
            {
                if(GameObject.transform.position.y > limitHeight && velocityAdjuster.Method.enabled)
                {
                    ConnectorSarah.SelfRigidbody.velocity = Vector2.right * ConnectorSarah.SelfRigidbody.velocity.x;
                    //velocityAdjuster.Method.enabled = false;
                    velocityAdjuster.Method.Disable();
                }
                velocityAdjuster.ManualUpdate();
                guard.ManualUpdate();

                clock.Update();
            }
            protected override void OnTerminate(bool isNormal)
            {
                attack.Inactivate();
                //velocityAdjuster.Method.enabled = false;
                velocityAdjuster.Method.Disable();
                umbrella.Default();
                guard.Method.Activated = false;
                clock.Reset();
            }
        }

        [System.Serializable]
        private class DropAttack : SarahState
        {
            [SerializeField] float abilityTime;
            [SerializeField] int amountConsumeUmbrellaDurability;
            [SerializeField] AttackInHitbox attack;
            [SerializeField] ActorFunction.VelocityAdjuster velocityAdjuster;
            [SerializeField] ActorFunction.Guard guard;
            [SerializeField] GroundSensor groundSensor;
            [SerializeField] Umbrella umbrella;

            IodoShibaUtil.ManualUpdateClass.ManualClock clock = new IodoShibaUtil.ManualUpdateClass.ManualClock();
            //float originalPlatformContactorHeight;
            Player player;
            Player Player { get => player == null ? (player = GameObject.GetComponent<Player>()) : player; }

            protected override bool IsAvailable() => base.IsAvailable() && ConnectorSarah.umbrellaParameters.DoesUmbrellaWork();
            protected override bool ShouldCotinue() => !groundSensor.IsOnGround && clock.Clock < abilityTime;
            protected override void OnInitialize()
            {
                base.OnInitialize();

                attack.Activate();
                velocityAdjuster.Method.enabled = true;
                umbrella.PlayerDropAttack();
                clock.Reset();
                ConnectorSarah.umbrellaParameters.TryConsumeDurability(amountConsumeUmbrellaDurability);
                Player.IsInvulnerable = true;
                ConnectorSarah.sarahAnimator.SetTrigger("DropAttackTrigger");
            }

            protected override void OnActive()
            {
                velocityAdjuster.ManualUpdate();
                guard.ManualUpdate();
                clock.Update();
            }
            protected override void OnTerminate(bool isNormal)
            {
                attack.Inactivate();
                //velocityAdjuster.Method.enabled = false;
                velocityAdjuster.Method.Disable();
                umbrella.Default();
                clock.Reset();
                Player.IsInvulnerable = false;
            }
        }

        [System.Serializable]
        private class SarahSmashed : SmashedState
        {
            [SerializeField] UnityEngine.Events.UnityEvent onInitialize;
            protected override void OnInitialize()
            {
                base.OnInitialize();
                ActorStateConnectorSarah actorStateConnectorSarah = Connector as ActorStateConnectorSarah;
                actorStateConnectorSarah.sarahAnimator.SetTrigger("SarahSmashedTrigger");

                onInitialize.Invoke();
            }
        }

        [System.Serializable]
        private class SarahDead : DeadState
        {
            [SerializeField] SpriteRenderer spriteRenderer;
            [SerializeField] ActorFunction.HorizontalMove horizontalMove;
            protected override void OnInitialize()
            {
                //死んだ通知出す
                spriteRenderer.color = new Color(1, 0.5f, 0.5f, 0.5f);
                GameObject.GetComponent<Player>().IsInvulnerable = true;

                horizontalMove.Method.StopActorOnDisabled(.1f);

                var rb2d = GameObject.GetComponent<Rigidbody2D>();
                rb2d.constraints = rb2d.constraints | RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
            }
        }
    }
}