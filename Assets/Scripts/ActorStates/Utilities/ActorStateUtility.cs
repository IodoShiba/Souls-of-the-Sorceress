using UnityEngine;
using System.Collections;

namespace ActorStateUtility {

    /// <summary>
    /// ActorのStateを連続して遷移させるのを手助けするクラス
    /// 主に同ボタンのコンボ攻撃の状態遷移に使う
    /// </summary>
    public class ChainAttackStream
    {
        ActorState.ActorStateConnector owner;
        float chainWaitSpan;
        float waitTimeLimit;
        bool isRecepting = false;
        readonly ActorState[] states;
        int i = 0;

        public bool IsRecepting { get => isRecepting; }

        public ChainAttackStream(float chainWaitSpan, params ActorState[] states)
        {
            this.chainWaitSpan = chainWaitSpan;
            this.states = states;
        }

        /// <summary>
        /// 状態を1つ先に進める試行を行うメソッド
        /// これを実行する以前にStartReception()を実行してisReeptingがtrueであったならば状態遷移が起こる
        /// falseであったならば状態遷移は起きない
        /// このメソッドの実行がStartReception()の実行からchainWaitSpan秒以内なら次の状態に進み、それより遅いと最初の状態に戻る
        /// </summary>
        /// <returns>次の状態 nullは現在の状態を継続する意</returns>
        public ActorState TryProceed()
        {
            if (!isRecepting) { return null; }

            if (Time.time > waitTimeLimit) { Reset(); }
            isRecepting = false;
            int ii = i;
            i = (i + 1) % states.Length;
            return states[ii];
        }

        /// <summary>
        /// 状態遷移の受付を開始するメソッド
        /// 最後に状態遷移してからこのメソッドを呼び出すまではTryProceed()を呼び出しても状態は遷移しない
        /// </summary>
        public void StartReception()
        {
            if (isRecepting) return;
            waitTimeLimit = Time.time + chainWaitSpan;
            isRecepting = true;
        }

        /// <summary>
        /// 最初の状態の戻すメソッド
        /// </summary>
        public void Reset()
        {
            i = 0;
            isRecepting = false;
        }

        /// <summary>
        /// 与えられた関数が真のときに状態を遷移させようとする関数を返すメソッド
        /// </summary>
        /// <param name="condf">試行するかしないかを返す関数</param>
        /// <returns>与えられた関数が真のときに状態を遷移させようとする関数</returns>
        public System.Func<ActorState> ProceedsWhen(System.Func<bool> condf) => () => condf() ? TryProceed() : null;
    }


}
