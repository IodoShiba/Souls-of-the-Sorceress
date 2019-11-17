using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwakeChargeItemReceiver : ItemReceiver<AwakeChargeItem>
{
    [SerializeField] ActionAwake actionAwake;
    [SerializeField] UnityEngine.Events.UnityEvent onReceivedItem;

    protected override void OnReceiveItem(AwakeChargeItem item)
    {
        onReceivedItem.Invoke();
        actionAwake.AddAwakeGauge(item.Amount);
    }
}
