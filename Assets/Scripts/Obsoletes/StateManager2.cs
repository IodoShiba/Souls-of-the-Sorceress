using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StateManager2 : MonoBehaviour
{
    [System.Serializable]
    protected class State
    {
        private class Branch
        {
            public int targetIndex;
            public System.Func<bool> cond;
            public Branch(int ti, System.Func<bool> c)
            {
                targetIndex = ti;
                cond = c;
            }
        }

        int selfIndex;
        [SerializeField] string name;
        private List<Branch> branches = new List<Branch>();
        [SerializeField] UnityEvent initializeActions = new UnityEvent();
        [SerializeField] UnityEvent executionActions = new UnityEvent();
        [SerializeField] UnityEvent terminateActions = new UnityEvent();
        bool valid = false;

        public int SelfIndex
        {
            get
            {
                return selfIndex;
            }
        }

        public bool Valid
        {
            get
            {
                return valid;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public State(int selfIndex,string name="") { this.selfIndex = selfIndex;this.name = name; }

        public State ConnectedTo(int targetStateIndex, System.Func<bool> condition)
        {
            if (!valid)
            {
                if (branches == null)
                {
                    branches = new List<Branch>();
                }
                branches.Add(new Branch(targetStateIndex, condition));
                return this;
            }
            else
            {
                throw new System.InvalidOperationException();
            }
        }

        public void EndDefine() { valid = true; }

        public void RegisterInitialize(UnityAction listener) { initializeActions.AddListener(listener); }
        public void RegisterExecute(UnityAction listener) { executionActions.AddListener(listener); }
        public void RegisterTerminate(UnityAction listener) { terminateActions.AddListener(listener); }


        public void Initialize()
        {
            if (valid)
            {
                initializeActions.Invoke();
            }
            else
            {
                throw new System.InvalidOperationException();
            }
        }
        public void Execute()
        {
            if (valid)
            {
                executionActions.Invoke();
            }
            else
            {
                throw new System.InvalidOperationException();
            }
        }
        public void Terminate()
        {
            if (valid)
            {
                terminateActions.Invoke();
            }
            else
            {
                throw new System.InvalidOperationException();
            }
        }

        public int JumpCheck() //　-1は「状態変化なし」の意
        {
            int jumpTo = -1;
            for (int i = 0; i < branches.Count; i++)
            {
                Branch ib = branches[i];
                jumpTo = ib.cond() ? ib.targetIndex : -1;
                if (jumpTo != -1)
                {
                    break;
                }
            }
            return jumpTo;
        }

    }

    public class StateConnector
    {
        StateManager2 owner;
        int departure;
        public StateConnector(StateManager2 owner,int departureState) { this.owner = owner;this.departure = departureState; }
        public StateConnector To(int destination,System.Func<bool> condition)
        {
            owner.states[departure].ConnectedTo(destination, condition);
            return this;
        }
        public StateConnector ConnectState(int departure) {
            this.departure = departure;
            return this;
        }
    }

    [SerializeField] protected string name;
    [SerializeField] protected List<State> states = null;//new List<State>();
    [SerializeField] UnityEngine.Events.UnityEvent stateChangeCallbacks;
    private State currentState;
    protected bool activated = false;

    public int CurrentStateIndex
    {
        get
        {
            return currentState.SelfIndex;
        }
    }

    public string CurrentStateName
    {
        get
        {
            return currentState.Name;
        }
    }

    protected void Start()
    {
        currentState = states[0];
    }
    // Update is called once per frame
    void Update()
    {
        if (activated)
        {
            this.Execute();
        }
    }

    public void RegisterInitialize(int targetStateIndex, UnityAction listener)
    {
        states[targetStateIndex].RegisterInitialize(listener);
    }
    public void RegisterExecute(int targetStateIndex, UnityAction listener)
    {
        states[targetStateIndex].RegisterExecute(listener);
    }
    public void RegisterTerminate(int targetStateIndex, UnityAction listener)
    {
        states[targetStateIndex].RegisterTerminate(listener);
    }
    public void RegisterStateChangeCallback(UnityAction listener)
    {
        stateChangeCallbacks.AddListener(listener);
    }

    public void Execute()
    {
        int jumpTo = -1;
        if ((jumpTo = currentState.JumpCheck()) != -1)
        {
            currentState.Terminate();
            do
            {
                currentState = states[jumpTo];
                jumpTo = currentState.JumpCheck();
            } while (jumpTo != -1);
            currentState.Initialize();
            stateChangeCallbacks.Invoke();
        }
        currentState.Execute();
    }

    public void ManuallyChangeState(int targetState)
    {
        if (currentState != null) { currentState.Terminate(); }
        currentState = states[targetState];
    }

    public void Activate(int initialStateName)
    {
        if (!activated)
        {
            ManuallyChangeState(initialStateName);
            activated = true;
        }
        else
        {
            throw new System.InvalidOperationException();
        }
    }

    public StateManager2 ConnectState(int departureState, int destinationState, System.Func<bool> condition)
    {
        states[departureState].ConnectedTo(destinationState, condition);
        return this;
    }

    public StateConnector ConnectState(int departureState) { return new StateConnector(this,departureState); }

    public StateManager2 EndDefine(int targetState)
    {
        states[targetState].EndDefine();
        return this;
    }
    
    public void EndDefineAll()
    {
        for(int i=0; i < states.Count; ++i)
        {
            states[i].EndDefine();
        }
    }

    public class StateMutable2<T>
    {
        public StateMutable2(StateManager2 target, T defaultObj)
        {
            this.target = target;
            content = defaultContent = defaultObj;
        }

        StateManager2 target;
        T defaultContent;
        T content;
        Dictionary<int, T> contents;

        public void Assign(T obj, int state)
        {
            contents[state] = obj;
        }

        public T Content
        {
            get
            {
                if (contents.ContainsKey(target.CurrentStateIndex))
                {
                    return contents[target.CurrentStateIndex];
                }
                else
                {
                    return defaultContent;
                }
            }
        }
    }
}