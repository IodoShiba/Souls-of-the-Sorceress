using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActorState
{
    //
    // 複数のActorStateから構成されるが、ActorStateConnectorの子クラス1つが持つActorStateは
    // それが属するActorStateConnector固有の物であろうから
    // ActorStateの定義もActorStateConnectorクラス定義の一部と考える
    /// <summary>
    /// Actorの状態の接続と遷移を担う抽象コンポーネント
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class ActorStateConnector : MonoBehaviour , IodoShiba.ManualUpdateClass.IManualUpdate
    {


        //TODO:フィールドにActorStateを列挙する(EditorがReflectionで捕捉する)


        //MEMO:
        //
        //

        ActorState current;
        private ActorState _defaultState;


        protected void ConnectState(System.Func<bool> flagFunc, ActorState from, ActorState to) { from.Connect(flagFunc, to); }
        protected void ConnectState(System.Func<ActorState> branchFunc, ActorState fromState) { fromState.Connect(branchFunc);  }
        protected void ConnectStateFromDefault(System.Func<ActorState> branchFunc) { ConnectState(branchFunc, _defaultState); }
        protected void ConnectStateFromDefault(System.Func<bool> flagFunc, ActorState toSkill) { _defaultState.Connect(flagFunc, toSkill); }
        
        protected virtual List<ActorState> ConstructActorState() => new List<ActorState>();
        public ActorState Current
        {
            get
            {
                return current;
            }
            private set => current = value;
        }

        public abstract ActorState DefaultState { get ;}

        //public Default DefaultState { get => defaultState; }

        protected virtual void Awake()
        {
            //GetComponent<Actor>().StateConnectorUpdate = ManualUpdate;

            _defaultState = DefaultState;
            BuildStateConnection();
            current = _defaultState;
            current._OnInitialize();
        }

        /// <summary>
        /// TODO:ConnectState()やConnectAsSkill()を羅列してStateの接続を構築する
        /// </summary>
        protected virtual void BuildStateConnection() { }

        public void ManualUpdate()
        {
            BeforeStateUpdate();
            ActorState next = Current.NextState();//nullはDefaultStateに戻れという意味とする
            if (next == Current)
            {
                Current.OnActive();
            }
            else
            {
                if (next == null) { next = DefaultState; }
                ChangeState(next, true);
            }
        }
        protected virtual void BeforeStateUpdate() { }

        /// <summary>
        /// Stateを外部から強制的に変更する
        /// </summary>
        /// <param name="actorState">変更先のState</param>
        /// <returns></returns>
        public bool InterruptWith(ActorState actorState)
        {
            if (current.IsResistibleTo(actorState.GetType())) { return false; }
            ChangeState(actorState,false);
            return true;
        }

        void ChangeState(ActorState next,bool isNormalTermination)
        {
            Current._OnTerminate(isNormalTermination);
            BeforeChangeState(next, isNormalTermination);
            Current = next;
            Current._OnInitialize();
        }

        /// <summary>
        /// ActorのStateが遷移するときに呼ばれる関数
        /// この時点でプロパティCurrentの中身は遷移元のStateで、それはすでにOnTerminate()が呼ばれ、終了処理が済んでいる
        /// </summary>
        /// <param name="next">次の状態</param>
        /// <param name="isNormalTermination">
        /// 前の状態が正常終了だったかどうか(Stateの正常終了：ActorStateConnector.Interrupt()の呼び出し以外の理由でStateが終了すること)
        /// </param>
        protected virtual void BeforeChangeState(ActorState next, bool isNormalTermination)
        {
        }
    }


    //MEMO:
    //
    //

    List<System.Func<ActorState>> nextStateCandidateFuncs = new List<System.Func<ActorState>>();
    [DisabledField,SerializeField] GameObject gameObject;
    [SerializeField, DisabledField] ActorStateConnector connector;

    bool isCurrent = false;
    public GameObject GameObject { get => gameObject; }
    public ActorStateConnector Connector { get => connector; }
    public bool IsCurrent { get => isCurrent; set => isCurrent = value; }


    //public string Name { get => name; set => name = value; }

    /// <summary>
    /// 特定の条件を満たしたときにこの状態から与えた状態へ遷移するようにする
    /// </summary>
    /// <param name="flagfunc">条件を表すbool(void)型関数</param>
    /// <param name="to">遷移先のAcotrState</param>
    public void Connect(System.Func<bool> flagfunc, ActorState to) { nextStateCandidateFuncs.Add(() => flagfunc() ? to : null); }

    /// <summary>
    /// この関数の遷移先を表す関数を追加し、この状態から次の状態への遷移関係を作る
    /// </summary>
    /// <param name="branchFunc">
    /// 遷移先のActorStateを返すActorState(void)型関数
    /// この関数がnull以外のActorStateを返した場合に状態はその返り値のActorStateへ遷移する
    /// </param>
    public void Connect(Func<ActorState> branchFunc) { nextStateCandidateFuncs.Add(branchFunc); }

    protected virtual bool IsAvailable() => true;
    public ActorState NextState()
    {
        ActorState nextState = this;

        if (!ShouldCotinue()) { nextState = null; }//nullはDefaultStateに戻れという意味とする

        foreach (var f in nextStateCandidateFuncs)//一方、fの返り値がnullとは現在の状態をそのまま継続せよという意味とする
        {
            ActorState res = f();
            if (res != null && res.IsAvailable())
            {
                nextState = res;
                break;
            }
        }

        return nextState;
    }

    public void _OnInitialize()
    {
        isCurrent = true;
        OnInitialize();
    }

    /// <summary>
    /// このStateの開始時に呼ばれる仮想メソッド
    /// 開始処理を記述する
    /// </summary>
    protected virtual void OnInitialize() { }

    /// <summary>
    /// Actorがこの「状態」である間毎フレーム呼び出される仮想メソッド
    /// </summary>
    protected virtual void OnActive() { }

    public void _OnTerminate(bool isNormal)
    {
        isCurrent = false;
        OnTerminate(isNormal);
    }

    /// <summary>
    /// このStateの終了時に呼ばれる仮想メソッド
    /// 終了処理を記述する
    /// </summary>
    /// <param name="isNormal">
    /// Stateの終了が中断でない通常の条件によるものか(通常：ShouldCotinue()がfalseを返してDefaultに遷移する or State遷移の条件を満たして次のStateに遷移する)</param>
    protected virtual void OnTerminate(bool isNormal) { }

    /// <summary>
    /// Stateを継続するかどうか返す仮想メソッド　OnActive()の前に呼ばれる
    /// </summary>
    /// <returns>Stateを継続するかどうか　trueで継続する</returns>
    protected virtual bool ShouldCotinue() => false;

    /// <summary>
    /// ActorStateConnector.InterruptWith()が呼び出されたときに呼び出される仮想メソッド
    /// InterruptWith()に渡された引数のAcotrStateによって自ら（ActorState）が中断されるのを拒める場合trueを返す
    /// デフォルトでは常にfalseを返す（他のあらゆるActorStateによって中断できる意を表す）
    /// </summary>
    /// <param name="actorStateType">中断に使われるActorStateの型オブジェクト</param>
    /// <returns>ActorStateConnector.InterruptWith()による中断を拒否できるか否か</returns>
    public virtual bool IsResistibleTo(System.Type actorStateType) => false;

}


[System.Serializable]
public class DefaultState : ActorState
{
    protected override bool ShouldCotinue() => true;
    protected DefaultState() { }
}




