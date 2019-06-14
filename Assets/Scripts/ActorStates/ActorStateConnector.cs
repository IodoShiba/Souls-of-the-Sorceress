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
    public abstract class ActorStateConnector : MonoBehaviour
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

        private void Awake()
        {
            _defaultState = DefaultState;
            BuildStateConnection();
            current = _defaultState;
            current._OnInitialize();
        }

        /// <summary>
        /// TODO:ConnectState()やConnectAsSkill()を羅列してStateの接続を構築する
        /// </summary>
        protected virtual void BuildStateConnection() { }

        private void Update()
        {
            ActorState next = Current.NextState();//nullはDefaultStateに戻れという意味とする
            if (next == Current)
            {
                Current.OnActive();
            }
            else
            {
                if (next == null) { next = DefaultState; }
                ChangeState(next,true);
            }
        }

        /// <summary>
        /// Stateを外部から強制的に変更する
        /// </summary>
        /// <param name="actorState">変更先のState</param>
        /// <returns></returns>
        public bool Interrupt(ActorState actorState)
        {
            ChangeState(actorState,false);
            return true;
        }

        void ChangeState(ActorState next,bool isNormalTermination)
        {
            Current._OnTerminate(isNormalTermination);
            OnChangeState(next, isNormalTermination);
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
        protected virtual void OnChangeState(ActorState next, bool isNormalTermination)
        {
        }
    }


    //MEMO:
    //
    //

    //string name;
    List<System.Func<ActorState>> nextStateCandidateFuncs = new List<System.Func<ActorState>>();
    [DisabledField,SerializeField] GameObject gameObject;
    [SerializeField, DisabledField] ActorStateConnector connector;


    //public string Name { get => name; set => name = value; }

    public void Connect(System.Func<bool> flagfunc, ActorState to) { nextStateCandidateFuncs.Add(() => flagfunc() ? to : null); }
    public void Connect(Func<ActorState> branchFunc) { nextStateCandidateFuncs.Add(branchFunc); }

    protected virtual bool IsAvailable() => true;
    public ActorState NextState()
    {
        if (!ShouldCotinue()) { return null; }//nullはDefaultStateに戻れという意味とする

        ActorState nextState = this;
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

        OnInitialize();
    }

    protected virtual void OnInitialize() { }

    public virtual void OnActive() { }

    public void _OnTerminate(bool isNormal)
    {
        OnTerminate(isNormal);
    }

    protected virtual void OnTerminate(bool isNormal) { }

    protected virtual bool ShouldCotinue() => false;
}


[System.Serializable]
public class Default : ActorState
{
    protected override bool ShouldCotinue() => true;
    protected Default() { }
}




