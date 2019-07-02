using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorFunction
{
    [Serializable]
    public class GuardFields : ActorFunctionFields
    {
        [SerializeField] float multiplier;
        [SerializeField] float degreeRangeStart;
        [SerializeField] float degreeRangeWidth;

        public float DegreeRangeStart { get => degreeRangeStart; set => degreeRangeStart = value; }
        public float DegreeRangeWidth { get => degreeRangeWidth; set => degreeRangeWidth = value; }

        public class Method : ActorFunctionMethod<GuardFields>
        {
            [SerializeField] bool activated;

            GuardFields fields;
            bool isAllSucceed = true;
            /// <summary>
            /// これまでのガードがすべて成功だったか
            /// </summary>
            public bool IsAllSucceed { get => isAllSucceed; }
            public bool Activated { get => activated; set => activated = value; }

            public override void ManualUpdate(in GuardFields fields)
            {
                this.fields = fields;
                //ResetSucceedState();
            }

            /// <summary>
            /// ガードできる場合は与えられたデータを倍率分減衰し、
            /// ガードできない場合は何も変更せず終了する
            /// </summary>
            /// <param name="dealt"></param>
            /// <param name="relativePosition"></param>
            public void TryGuard(AttackData dealt, in Vector2 relativePosition)
            {
                if (fields == null || !Activated) { return; }
                bool c = GuardCondition(relativePosition);
                isAllSucceed = isAllSucceed && c;
                if (c)
                {
                    dealt.damage *= fields.multiplier;
                    dealt.knockBackImpulse *= fields.multiplier;
                }
            }

            private bool GuardCondition(in Vector2 relativePosition)
            {
                float da0to360 =
                    Mathf.DeltaAngle(
                        fields.degreeRangeStart + 180,
                        Mathf.Atan2(relativePosition.y, relativePosition.x) * Mathf.Rad2Deg
                        ) + 180;
                return 0 < da0to360 && da0to360 < fields.degreeRangeWidth;
            }

            /// <summary>
            /// ガードの成功・失敗の状態をリセットする
            /// </summary>
            private void ResetSucceedState() { isAllSucceed = true; }

            public bool GetIsAllSucceedAndReset() { bool ret = isAllSucceed; ResetSucceedState();return ret; }
        }

    }

    public class GuardMethod : GuardFields.Method { }

    /// <summary>
    /// Actorのガード機能を担うクラス
    /// ガードの成否は攻撃対象に対し攻撃がどの方向にあるかよって決まる
    /// </summary>
    [System.Serializable]
    public class Guard : ActorFunction<GuardFields,GuardFields.Method>
    {

    }
}
namespace PlayerStates
{
    public class PlayerGuard : State
    {
        [SerializeField] Rigidbody2D player;
        [SerializeField] Player playerData;
        
        public override State Check()
        {
            if(!(playerData.DoesUmbrellaWork() && Input.GetButton("Open Umbrella")))
            {
                return GetComponent<PlayerStates.PlayerOnGround>();
            }
            if (Input.GetButtonDown("Attack"))
            {
                return GetComponent<PlayerStates.PlayerTackle>();
            }
            return null;
        }

        public override void Initialize()
        {
        }

        public override void Execute()
        {
            player.velocity = new Vector2(0, 0);
        }

        public override void Terminate()
        {
        }
    }
}

//Playerに依存しないように書き直す
//public class Guard : BasicAbility
//{
//    [SerializeField] Rigidbody2D player;
//    [SerializeField] Player playerData;
//    [SerializeField] Collider2D extensionCollider;
//    [SerializeField] AttackInHitbox knockbackAttack;
//    [SerializeField] Umbrella umbrella;

//    protected override bool ShouldContinue(bool ordered)
//    {
//        return ordered && playerData.DoesUmbrellaWork();
//    }
//    protected override void OnInitialize()
//    {
//        extensionCollider.enabled = true;
//        knockbackAttack.Activate();
//        umbrella.PlayerGuard();
//    }

//    protected override void OnActive(bool ordered)
//    {
//        player.velocity = new Vector2(0, 0);
//    }

//    protected override void OnTerminate()
//    {
//        extensionCollider.enabled = false;
//        knockbackAttack.Inactivate();
//        umbrella.Default();
//    }
//}