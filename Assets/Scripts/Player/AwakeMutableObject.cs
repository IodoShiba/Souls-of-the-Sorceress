using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AwakeMutableObject
{
    //[SerializeField] StateManager stateManager;
    [SerializeField] GameObject target;
    [SerializeField] GameObject ordinary;
    [SerializeField] GameObject awaken;
    [SerializeField] GameObject blueAwaken;
    
    public static implicit operator GameObject(AwakeMutableObject self)
    {
        if (self.target.GetComponent<PlayerStates.Awakening.Ordinary>().IsCurrent)
        {
            return self.ordinary;
        }
        else if (self.target.GetComponent<PlayerStates.Awakening.Awaken>().IsCurrent)
        {
            return self.awaken;
        }
        else if (self.target.GetComponent<PlayerStates.Awakening.BlueAwaken>().IsCurrent)
        {
            return self.blueAwaken;
        }
        return null;
    }

    public GameObject GetObject()
    {
        return (GameObject)this;
    }

    public void SynchronizeWith(System.Action<GameObject> substitutor)
    {
        PlayerStates.Awakening.Ordinary oc = target.GetComponent<PlayerStates.Awakening.Ordinary>();
        PlayerStates.Awakening.Awaken ac = target.GetComponent<PlayerStates.Awakening.Awaken>();
        PlayerStates.Awakening.BlueAwaken bc = target.GetComponent<PlayerStates.Awakening.BlueAwaken>();
        oc.RegisterInitialize(() => substitutor(ordinary));
        ac.RegisterInitialize(() => substitutor(awaken));
        bc.RegisterInitialize(() => substitutor(blueAwaken));
        oc.RegisterTerminate(() => substitutor(null));
        ac.RegisterTerminate(() => substitutor(null));
        bc.RegisterTerminate(() => substitutor(null));
        if (oc.IsCurrent) { substitutor(ordinary); }
        else if (ac.IsCurrent) { substitutor(awaken); }
        else if (bc.IsCurrent) { substitutor(blueAwaken); }
    }
    
    //public void Ordinary() { presentForm = ordinary; }
    //public void Awaken() { presentForm = awaken; }
    //public void BlueAwaken() { presentForm = blueAwaken; }
}

//StateMutable クラス
//状態(State)に依存して変化するオブジェクトを表す
//覚醒状態に応じて変化するダメージ倍率等に使う
public class StateMutable<T>
{
    public StateMutable(GameObject target,T defaultObj)
    {
        this.target = target;
        content = defaultContent = defaultObj;
    }

    [SerializeField] GameObject target;
    T defaultContent;
    T content;

    public T Content
    {
        get
        {
            return content;
        }
    }

    public void Assign<StateType>(T obj) where StateType : State
    {
        var s = target.GetComponent<StateType>();
        s.RegisterInitialize(() => { content = obj; });
        s.RegisterTerminate(() => { content = defaultContent; });
        if (target.GetComponent<StateType>().IsCurrent)
        {
            content = obj;
        }
    }
}