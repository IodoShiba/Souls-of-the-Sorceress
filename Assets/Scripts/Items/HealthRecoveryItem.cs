using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 体力回復のアイテム
/// </summary>
public class HealthRecoveryItem : ItemBase
{
    /// <summary>
    /// 回復量
    /// </summary>
    [SerializeField] float amount;

    public float Amount { get => amount; }
}
