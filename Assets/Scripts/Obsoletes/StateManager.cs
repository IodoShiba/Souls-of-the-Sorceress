using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StateManager : MonoBehaviour
{
    /*[System.Serializable]
    public class Gate
    {
        [SerializeField] bool condition;
        [SerializeField] string destination;
    }

    [System.Serializable]
    public class Junction
    {
        [SerializeField] string stateName;
        [SerializeField] State reference;
        [SerializeField] List<Gate> to;
    }
    [SerializeField] List<Junction> stateBranch;*/

    [SerializeField] string name;
    [SerializeField] State currentState = null;

    public State CurrentState
    {
        get
        {
            return currentState;
        }
    }

    public void /*Start*/Initialize(State initialState = null)
    {
        if (initialState != null) currentState = initialState;
        if (currentState != null)
        {
            State nns = null;
            while ((nns = currentState.Check()/*Initialize()*/) != null)
            {
                currentState = nns;
            }
            currentState.Initialize();
            currentState._InvokeInitializeCallBack();
        }
    }

    public void /*Update*/Execute()
    {
        if (currentState != null)
        {
            var ns = currentState.Check();
            if(ns == null)
            {
                currentState.Execute();
            }
            else
            {
                currentState._InvokeTerminateCallBack();
                currentState.Terminate();
                State nns = null;
                while ((nns = ns.Check()) != null)
                {
                    ns = nns;
                }
                ns.Initialize();
                ns._InvokeInitializeCallBack();
                currentState = ns;
            }
        }
        /*if (currentState != null)
        {
            var ns = currentState.Execute();
            if (ns != null)
            {
                currentState.Terminate();
                currentState = ns;
                State nns = null;
                while ((nns = ns.Initialize()) != null)
                {
                    ns = nns;
                }
                ns.InvokeInitializeCallBack();
            }
        }*/
    }

    public void ManuallyChange(State targetState)
    {
        currentState._InvokeTerminateCallBack();
        currentState.Terminate();
        State nns = null;
        while ((nns = targetState.Check()) != null)
        {
            targetState = nns;
        }
        targetState.Initialize();
        targetState._InvokeInitializeCallBack();
        currentState = targetState;
    }

    public bool CheckCurrent<T>() where T : State
    {
        return gameObject.GetComponent<T>().IsCurrent;
    }
}

/*
public class StateManager : MonoBehaviour
{
    [System.Serializable]
    private class State
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

        public State(int selfIndex) { this.selfIndex = selfIndex; }

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
            for(int i = 0; i < branches.Count; i++)
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

    [SerializeField]private List<State> states = null;//new List<State>();
    private State currentState;
    private bool activated = false;

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

    // Update is called once per frame
    void Update()
    {
        if (activated)
        {
            this.Execute();
        }
    }

    public void RegisterExecute(int targetStateIndex, UnityAction listener)
    {
        states[targetStateIndex].RegisterExecute(listener);
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

    public StateManager ConnectState(int departureState,int destinationState,System.Func<bool> condition)
    {
        states[departureState].ConnectedTo(destinationState, condition);
        return this;
    }

    public StateManager EndDefine(int targetState)
    {
        states[targetState].EndDefine();
        return this;
    }
}
 */
