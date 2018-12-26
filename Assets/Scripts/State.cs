using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StateManager))]
public abstract class State : MonoBehaviour
{
    StateManager owner;
    [SerializeField] UnityEngine.Events.UnityEvent InitializeCallbacks = new UnityEngine.Events.UnityEvent();
    [SerializeField] UnityEngine.Events.UnityEvent TerminateCallbacks = new UnityEngine.Events.UnityEvent();
    private List<System.Action> initializeActions = new List<System.Action>();
    private List<System.Action> terminateActions = new List<System.Action>();
    [SerializeField]private bool isCurrent = false;

    public bool IsCurrent
    {
        get
        {
            return isCurrent;
        }
    }


    private void Awake()
    {
        initializeActions.Add((System.Action)(() => { this.isCurrent = true; }));
        terminateActions.Add((System.Action)(() => { this.isCurrent = false; }));
    }

    private void Start()
    {
        owner = GetComponent<StateManager>();
    }
    public abstract void Initialize();
    public abstract State Check();
    public abstract void Execute();
    public abstract void Terminate();
    public void InvokeInitializeCallBack()
    {
        InitializeCallbacks.Invoke();
        foreach(var f in initializeActions)
        {
            f();
        }
    }
    public void InvokeTerminateCallBack()
    {
        TerminateCallbacks.Invoke();
        foreach (var f in terminateActions)
        {
            f();
        }
    }
    public void RegisterInitialize(System.Action listener) {
        initializeActions.Add(listener);
    }
    public void RegisterTerminate(System.Action listener) {
        terminateActions.Add(listener);
    }
    public void RegisterTurnAction(System.Action initializeListener,System.Action terminateListener)
    {
        RegisterInitialize(initializeListener);
        RegisterTerminate(terminateListener);
    }

    public State Precheck(State destination)
    {
        return destination.Check() == null ? destination : null;
    }

    
}

[RequireComponent(typeof(StateManager))]
public abstract class HorizontalDirectionDependentState : State
{
    protected int dirSign = 0;
    public void SetDirection(int sign)
    {
        dirSign = sign;
    }
}

