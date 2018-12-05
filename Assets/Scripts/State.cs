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
}

[RequireComponent(typeof(StateManager))]
public abstract class HorizontalDirectionalState : State
{
    protected int dirSign = 0;
    public void SetDirection(int sign)
    {
        dirSign = sign;
    }
}

