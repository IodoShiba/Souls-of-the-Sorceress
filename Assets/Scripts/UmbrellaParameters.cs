using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UmbrellaParameters : MonoBehaviour
{
    public class Viewer : MonoBehaviour
    {
        [SerializeField] protected UmbrellaParameters target;
        protected float Health { get => target.durability; }
        protected float MaxHealth { get => target.maxDurability; }
    }

    [SerializeField] GameObject _playerObj;
    [SerializeField] int durability;
    [SerializeField] int maxDurability;
    [SerializeField] float recoverCycle;
    [SerializeField] float breakTime;
    [SerializeField] bool cheat_InfinityDurability;
    [System.NonSerialized] private float breakRestTime = 0;
    [System.NonSerialized] private float t = 0;
    [System.NonSerialized] private float changeCycle = 100000;
    [System.NonSerialized] private int changeAmount = 0;

    public int Durability { get => durability; }
    public int MaxDurability { get => maxDurability; }

    private void Update()
    {
        if (DoesUmbrellaWork())
        {
            breakRestTime = 0;
            if (t > changeCycle)
            {
                AddDurability(changeAmount);
                t -= changeCycle;
            }
            t += Time.deltaTime;
        }
        else //破損状態
        {
            breakRestTime -= Time.deltaTime;
            if (breakRestTime <= 0)
            {
                durability = maxDurability;
            }
        }
        if (cheat_InfinityDurability) { durability = maxDurability; }
    }

    public void ChangeDurabilityGradually(float cycle, int amount)
    {
        this.changeCycle = cycle;
        this.changeAmount = amount;
        t = 0;
    }

    /// <summary>
    /// amountの文だけ傘耐久度を加算する
    /// </summary>
    /// <param name="amount">変化量</param>
    public void AddDurability(int amount) 
    {
        durability += amount;
        if (durability < 0 && DoesUmbrellaWork())
        {
            breakRestTime = breakTime;
        }
        durability = Mathf.Clamp(durability, 0, maxDurability);
    }

    /// <summary>
    /// amountの分だけ傘耐久度を消費する
    /// </summary>
    /// <param name="amount"></param>
    /// <returns>実際に消費された量</returns>
    public int TryConsumeDurability(int amount)
    {
        int d = durability;
        AddDurability(-amount);
        return d - durability;
    }

    public void RecoverEntirely() { AddDurability(maxDurability); }

    public bool DoesUmbrellaWork() => breakRestTime <= 0;

    public string _DebugOutput() { return $"Umbrella Durability:{(DoesUmbrellaWork() ? $"{Durability}/{MaxDurability}" : $"(Recover in {breakRestTime} seconds)")}\n"; }


}