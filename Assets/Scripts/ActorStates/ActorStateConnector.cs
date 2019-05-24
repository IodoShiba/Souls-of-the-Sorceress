using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActorState
{
    public abstract class ActorStateConnector : MonoBehaviour
    {
        //TODO:フィールドにActorStateを列挙する(EditorがReflectionで捕捉する)

        //[System.NonSerialized] List<ActorState> actorStates;
        ActorState current;
        private ActorState _defaultState;

        protected void ConnectState(System.Func<bool> flagFunc, ActorState from, ActorState to) { from.Connect(flagFunc, to); }
        protected void ConnectState(System.Func<ActorState> branchFunc, ActorState fromState) { fromState.Connect(branchFunc);  }
        protected void ConnectState(System.Func<ActorState> branchFunc) { ConnectState(branchFunc, _defaultState); }
        protected void ConnectAsSkill(System.Func<bool> flagFunc, ActorState toSkill) { _defaultState.Connect(flagFunc, toSkill); }
        //protected ActorRepresenter() { actorStates = ConstructActorState(); }
        protected virtual List<ActorState> ConstructActorState() => new List<ActorState>();
        public ActorState Current
        {
            get
            {
                if (current == null) current = _defaultState;
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
        }

        //TODO:ConnectState()やConnectAsSkill()を羅列してStateの接続を構築する
        protected virtual void BuildStateConnection() { }

        private void Update()
        {
            ActorState next = Current.NextState();
            if (next == Current)
            {
                Current.OnActive();
            }
            else
            {
                ChangeState(next,true);
            }
        }

        public bool Interrupt(ActorState actorState)
        {
            ChangeState(actorState,false);
            return true;
        }

        void ChangeState(ActorState next,bool isNormalTermination)
        {
            Current._OnTerminate(isNormalTermination);
            Current = next;
            OnChangeState(next, isNormalTermination);
            Current._OnInitialize();
        }

        protected virtual void OnChangeState(ActorState next, bool isNormalTermination)
        {
        }
    }



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
        if (!ShouldCotinue()) { return connector.DefaultState; }

        ActorState nextState = this;
        foreach (var f in nextStateCandidateFuncs)
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




