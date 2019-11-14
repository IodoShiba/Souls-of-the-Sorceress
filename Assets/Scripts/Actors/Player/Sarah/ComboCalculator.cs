using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

/// <summary>
/// 同時巻き込みで覚醒ゲージ上昇量を増加させる
/// </summary>
public class ComboCalculator : MonoBehaviour
{
    //[System.Serializable]
    //class AwakeIncome
    //{
    //    public int combo;
    //    public float amount;
    //}

    [System.Serializable] class UnityEvent_int : UnityEngine.Events.UnityEvent<int> { }

    //[SerializeField] float timeLimit;
    //[SerializeField] List<AwakeIncome> awakeBonuses;
    [SerializeField] float baseAwakeIncome;
    [SerializeField] float awakeIncomeCoef;
    [SerializeField] ActionAwake awake;
    [SerializeField] UnityEngine.Events.UnityEvent onComboIncremented;
    [SerializeField] UnityEvent_int onComboIncrementedInt;
    [SerializeField,DisabledField] int comboCount = 0;
    float addedAmount;

    //IodoShibaUtil.ManualUpdateClass.ManualClock clock = new IodoShibaUtil.ManualUpdateClass.ManualClock();
    bool IsInCombo { get => comboCount > 0; }
    
    void Awake()
    {
        comboCount = 0;
        //clock.Reset();
    }

    //private void Update()
    //{
    //    if (!IsInCombo) { return; }

    //    clock.Update();
    //    if(clock.Clock >= timeLimit)
    //    {
    //        ResetCombo();
    //    }
    //}

    private void OnGUI()
    {
        GUI.Label(new Rect(0, 0, Screen.width, Screen.height), comboCount.ToString());
    }

    public void ResetCombo()
    {
        //Debug.Log("Combo was Reset.");
        comboCount = 0;
        //clock.Reset();
    }

    public void IncrementCombo()
    {
        float add = AwakeAddAmount(comboCount);
        ++comboCount;

        onComboIncremented.Invoke();
        onComboIncrementedInt.Invoke(comboCount);
    }

    float AwakeAddAmount(int combos) => awakeIncomeCoef * combos + baseAwakeIncome;

    public void AddAwakeGauge(int comboCount)
    {
        float add = AwakeAddAmount(comboCount - 1);
        float income = add - addedAmount;
        addedAmount = add;
        //for(int i = awakeBonuses.Count - 1; i >= 0; --i)
        //{
        //    if(comboCount >= awakeBonuses[i].combo)
        //    {
        //        income = awakeBonuses[i].amount;
        //    }
        //}
        awake.AddAwakeGauge(income);
    }
}
