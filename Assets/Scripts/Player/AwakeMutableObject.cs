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
    ActionAwake targetAA;
    ActionAwake TargetAA { get => targetAA == null ? (targetAA = target.GetComponent<ActionAwake>()) : targetAA; }
    public static implicit operator GameObject(AwakeMutableObject self)
    {
        switch (self.TargetAA.AwakeLevel)
        {
            case ActionAwake.AwakeLevels.ordinary:
                return self.ordinary;
            case ActionAwake.AwakeLevels.awaken:
                return self.awaken;
            case ActionAwake.AwakeLevels.blueAwaken:
                return self.blueAwaken;

            default:
                return null;
        }
    }

    public GameObject GetObject()
    {
        return (GameObject)this;
    }

}

//StateMutable クラス
//状態(State)に依存して変化するオブジェクトを表す
//覚醒状態に応じて変化するダメージ倍率等に使う
public class AwakeMutable<T>
{
    [SerializeField] GameObject target;
    [SerializeField] T ordinary;
    [SerializeField] T awaken;
    [SerializeField] T blueAwaken;
    ActionAwake targetAA;
    ActionAwake TargetAA { get => targetAA == null ? (targetAA = target.GetComponent<ActionAwake>()) : targetAA; }

    public T Content
    {
        get
        {
            switch (TargetAA.AwakeLevel)
            {
                case ActionAwake.AwakeLevels.ordinary:
                    return ordinary;
                case ActionAwake.AwakeLevels.awaken:
                    return awaken;
                case ActionAwake.AwakeLevels.blueAwaken:
                    return blueAwaken;

                default:
                    return default(T);
            }
        }
    }

    public void Assign(T ordinary,T awaken,T blueAwaken)
    {
        this.ordinary = ordinary;
        this.awaken = awaken;
        this.blueAwaken = blueAwaken;
    }

    public void Apply(System.Action<T> action)
    {
        action(ordinary);
        action(awaken);
        action(blueAwaken);
    }
}
