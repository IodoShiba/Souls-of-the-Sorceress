using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActorState
{

    public abstract class ActorStateConnector : MonoBehaviour
    {
        protected void ConnectState(System.Func<bool> flagFunc, ActorState from, ActorState to) { from.Connect(flagFunc, to); }
        protected void ConnectAsSkill(System.Func<bool> flagFunc, ActorState toSkill) { _defaultState.Connect(flagFunc, toSkill); }

        //[System.NonSerialized] List<ActorState> actorStates;
        ActorState current;
        private ActorState _defaultState;

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
                ChangeState(next);
            }
        }

        public bool Interrupt(ActorState actorState)
        {
            ChangeState(actorState);
            return true;
        }

        void ChangeState(ActorState next)
        {
            Current._OnTerminate(true);
            Current = next;
            Current._OnInitialize();
        }

    }



    //string name;
    List<System.Func<ActorState>> nextStateCandidateFuncs = new List<System.Func<ActorState>>();
    [DisabledField,SerializeField] GameObject gameObject;
    [SerializeField, DisabledField, UnityEngine.Serialization.FormerlySerializedAs("representer")] ActorStateConnector connector;

    //public string Name { get => name; set => name = value; }

    public void Connect(System.Func<bool> flagfunc, ActorState to) { nextStateCandidateFuncs.Add(() => flagfunc() ? to : null); }
    protected virtual bool IsAvailable() => true;
    public ActorState NextState()
    {
        if (!ShouldCotinue()) { return null; }

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
    Default() { }
}




