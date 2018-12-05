using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AwakeMutableObject
{
    [SerializeField] StateManager stateManager;
    [SerializeField] GameObject ordinary;
    [SerializeField] GameObject awaken;
    [SerializeField] GameObject blueAwaken;
    
    public static implicit operator GameObject(AwakeMutableObject self)
    {
        //self.awakeState.CurrentState.GetType
        var sn = self.stateManager.CurrentState;
        if(sn as PlayerStates.Awakening.Ordinary != null)
        {
            return self.ordinary;
        }
        else if (sn as PlayerStates.Awakening.Awaken != null)
        {
            return self.awaken;
        }
        else if (sn as PlayerStates.Awakening.BlueAwaken != null)
        {
            return self.blueAwaken;
        }
        return null;
    }

    public GameObject GetObject()
    {
        return (GameObject)this;
    }

    //public void Ordinary() { presentForm = ordinary; }
    //public void Awaken() { presentForm = awaken; }
    //public void BlueAwaken() { presentForm = blueAwaken; }
}

public class StateMutable<T>
{
    //StateManager monitor;
    T content = default(T);

    public T Content
    {
        get
        {
            return content;
        }
    }

    public void Assign<StateType>(T obj, GameObject target) where StateType : State
    {
        target.GetComponent<StateType>().RegisterInitialize(() => { content = obj; });
        if(target.GetComponent<StateManager>().CurrentState is StateType) //実行時型情報 好かん
        {
            content = obj;
        }
    }
}