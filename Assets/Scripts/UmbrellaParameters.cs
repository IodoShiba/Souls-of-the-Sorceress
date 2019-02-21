using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UmbrellaParameters : MonoBehaviour
{
    [SerializeField] GameObject _playerObj;
    [SerializeField] int durability;
    [SerializeField] int maxDurability;
    [SerializeField] float recoverCycle;
    [SerializeField] float breakTime;
    [SerializeField] int costOfGuard;
    [SerializeField] int costOfTackle;
    [SerializeField] int costOfRisingAttack;
    [SerializeField] int costOfGlidingPerSecond;
    [System.NonSerialized] private float breakRestTime = 0;
    [System.NonSerialized] private float t = 0;
    [System.NonSerialized] private float cycle = 100000;
    [System.NonSerialized] private int amount = 0;

    public int CostOfGuard { get => costOfGuard;}
    public int CostOfTackle { get => costOfTackle;}
    public int CostOfRisingAttack { get => costOfRisingAttack;}
    public int CostOfGlidingPerSecond { get => costOfGlidingPerSecond;  }
    public int Durability { get => durability; }
    public int MaxDurability { get => maxDurability; }

    private void Start()
    {
        /*
        _playerObj.GetComponent<PlayerStates.PlayerTackle>().RegisterTurnAction(
            () => { NudgeDurability(-CostOfTackle); _Consuming(); },
            _Recovering
            );
        _playerObj.GetComponent<PlayerStates.PlayerRisingAttack>().RegisterTurnAction(
            () => { NudgeDurability(-CostOfRisingAttack); _Consuming(); },
            _Recovering
            );
        _playerObj.GetComponent<PlayerStates.PlayerGliding>().RegisterTurnAction(
            _Gliding,
            _Recovering
            );
        _playerObj.GetComponent<PlayerStates.PlayerGuard>().RegisterTurnAction(
            _Consuming,
            _Recovering
            );
            */
    }

    private void Update()
    {
        if (breakRestTime <= 0)
        {
            breakRestTime = 0;
            if (t > cycle)
            {
                NudgeDurability(amount);
                t -= cycle;
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
    }

    public void ChangeDurabilityDifferential(float cycle, int amount)
    {
        this.cycle = cycle;
        this.amount = amount;
        t = 0;
    }
    public void _Recovering() { ChangeDurabilityDifferential(recoverCycle, 1); }
    public void _Gliding() { ChangeDurabilityDifferential(1, -costOfGlidingPerSecond); }
    public void _Consuming() { ChangeDurabilityDifferential(100000, 0); }

    public void NudgeDurability(int amount) //amountの文だけ傘耐久度を変更する
    {
        durability += amount;
        if (durability > maxDurability)
        {
            durability = maxDurability;
        }
        if (durability < 0)
        {
            breakRestTime = breakTime;
        }
    }

    public bool DoesUmbrellaWork() => breakRestTime <= 0;

    public string _DebugOutput() { return $"Umbrella Durability:{(DoesUmbrellaWork() ? $"{Durability}/{MaxDurability}" : $"(Recover in {breakRestTime} seconds)")}\n"; }

}