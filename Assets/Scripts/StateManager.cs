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
