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

    public class UmbrellaAttackConverter : AttackConverter
    {
        [SerializeField] UmbrellaParameters target;
        public override bool Convert(AttackData value)
        {
            if (target.DoesUmbrellaWork()) { return true; }
            value.damage /= 2;
            value.knockBackImpulse /= 2;
            return true;
        }
    }

    [SerializeField] GameObject _playerObj;
    [SerializeField] int durability;
    [SerializeField] int maxDurability;
    [SerializeField] float recoverCycle;
    [SerializeField] float breakTime;
    [SerializeField] UnityEngine.Events.UnityEvent onUmbrellaBreoken;
    [SerializeField] bool cheat_InfinityDurability;
    [System.NonSerialized] private float breakRestTime = 0;
    [System.NonSerialized] private float t = 0;
    [System.NonSerialized] private float changeCycle = float.PositiveInfinity;
    [System.NonSerialized] private int changeAmount = 0;

    public int Durability { get => durability; }
    public int MaxDurability { get => maxDurability; }
    public float BreakRestTime { get => breakRestTime;
        private set
        {
            if(breakRestTime <= 0 && value > 0)
            {
                onUmbrellaBreoken.Invoke();
            }
            breakRestTime = value;
        }
    }

    private void Awake()
    {
        changeCycle = float.PositiveInfinity;
    }
    private void Update()
    {
        if (DoesUmbrellaWork())
        {
            BreakRestTime = 0;
            t += Time.deltaTime;
            if (t > changeCycle)
            {
                AddDurability(changeAmount);
                t -= changeCycle;
            }
        }
        else //破損状態
        {
            BreakRestTime -= Time.deltaTime;
            if (BreakRestTime <= 0)
            {
                durability = maxDurability;
            }
        }
        if (cheat_InfinityDurability) { durability = maxDurability; }
    }

    public void ChangeDurabilityGradually(float cycle, int amount, bool inheritCount = true)
    {
        //if (inheritCount)
        //{
        //    if (amount * changeAmount < 0)
        //    {
        //        t = cycle * (1 - t / changeCycle);
        //    }
        //    else if (amount * changeAmount > 0)
        //    {
        //        t = cycle * (t / changeCycle);
        //    }
        //}
        //else
        {
            t = 0;
        }

        if(cycle == 0) { throw new System.ArgumentOutOfRangeException("parameter 'cycle' cannot be 0."); }
        this.changeCycle = cycle;
        this.changeAmount = amount;
    }

    public void StopChangeDurabilityGradually(bool inheritCount = true)
    {
        ChangeDurabilityGradually(float.PositiveInfinity, 0, inheritCount);
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
            BreakRestTime = breakTime;
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

    public bool DoesUmbrellaWork() => BreakRestTime <= 0;

    public string _DebugOutput() { return $"Umbrella Durability:{(DoesUmbrellaWork() ? $"{Durability}/{MaxDurability}" : $"(Recover in {BreakRestTime} seconds)")}\n"; }


}