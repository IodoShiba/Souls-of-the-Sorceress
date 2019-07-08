using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class DropsLoot : MonoBehaviour
{
    [SerializeField] LootTable lootTable;

    public void Activate(float time = 0)
    {
        if(time == 0)
        {
            DropAnItem();
        }
        else
        {
            Observable.Timer(TimeSpan.FromSeconds(time)).Subscribe(_ => DropAnItem()).AddTo(gameObject);
        }
    }

    private void DropAnItem()
    {
        var i = lootTable.GetItem();
        if (i != null)
        {
            Instantiate(i, transform.position, Quaternion.identity);
        }
    }
}
