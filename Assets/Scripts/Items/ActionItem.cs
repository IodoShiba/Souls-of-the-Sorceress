using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// イベント発生のアイテム
/// </summary>
public class ActionItem : ItemBase
{
    [SerializeField] protected UnityEvent actions;

    public void InvokeAction()
    {
        actions.Invoke();
    }
}
