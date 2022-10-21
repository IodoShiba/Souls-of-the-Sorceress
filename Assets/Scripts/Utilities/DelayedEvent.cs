using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.Events;

public class DelayedEvent : MonoBehaviour
{
    [SerializeField] float dueTimeSecond;
    [SerializeField] UnityEvent events;

    public void Do()
    {
        UniRx.Observable.Return(Unit.Default).Delay(System.TimeSpan.FromSeconds(dueTimeSecond)).Subscribe(_=>events.Invoke());
    }

    public void SetDelay(float dueTimeSecond)
    {
        this.dueTimeSecond = dueTimeSecond;
    }
}
