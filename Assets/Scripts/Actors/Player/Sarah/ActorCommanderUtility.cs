using UnityEngine;
using UnityEditor;

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
            float finallyPushedTime;
            bool allowedToStartCount = true;

            public float PushedTime { get => pushedTime; }
            public float delayedPushedTime { get => subject.eval ? pushedTime : finallyPushedTime; }
            public bool IsLongPushedUp { get => finallyPushedTime > thresholdTime; }
            public bool IsLongPushed { get => pushedTime > thresholdTime; }
            public bool AllowedToStartCount { get => allowedToStartCount; set => allowedToStartCount = value; }
            public float FinallyPushedTime { get => finallyPushedTime; }

            public LongPushClock(BoolExpressions subject, float thresholdTime)
            {
                this.subject = subject;
                pushedTime = 0;
                finallyPushedTime = 0;
                this.thresholdTime = thresholdTime;
            }

            public void Update()
            {
                finallyPushedTime = subject.IsUp ? pushedTime : 0;
                pushedTime = (pushedTime > 0 && subject.eval) || ( AllowedToStartCount && subject.isDown) ? pushedTime + Time.deltaTime : 0;
            }

            public void Reset()
            {
                pushedTime = 0;
                finallyPushedTime = 0;
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
        public override void Update() {}

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

        public MultiPushExpressions(float timeThreshold, params BoolExpressions[] boolInput)
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
            for (int i = 0; i < boolInputs.Length; ++i)
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
