using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// イベント発生のアイテム
/// </summary>
public class OneUpItem : ActionItem
{
    void Awake()
    {
        actions.AddListener(()=>SotS.ReviveController.AddRemainingCount(1));
    }
}
