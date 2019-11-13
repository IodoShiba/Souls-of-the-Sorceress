using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class ComboCalculator : MonoBehaviour
{
    [System.Serializable]
    class AwakeIncome
    {
        public int combo;
        public float amount;
    }

    [System.Serializable] class UnityEvent_int : UnityEngine.Events.UnityEvent<int> { }

    [SerializeField] float timeLimit;
    [SerializeField] List<AwakeIncome> awakeBonuses;
    [SerializeField] ActionAwake awake;
    [SerializeField] UnityEngine.Events.UnityEvent onComboIncremented;
    [SerializeField] UnityEvent_int onComboIncrementedInt;
    [SerializeField,DisabledField] int comboCount = 0;

    IodoShibaUtil.ManualUpdateClass.ManualClock clock = new IodoShibaUtil.ManualUpdateClass.ManualClock();
    bool IsInCombo { get => comboCount > 0; }
    
    void Awake()
    {
        comboCount = 0;
        clock.Reset();
    }

    private void Update()
    {
        if (!IsInCombo) { return; }

        clock.Update();
        if(clock.Clock >= timeLimit)
        {
            ResetCombo();
        }
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(0, 0, Screen.width, Screen.height), comboCount.ToString());
    }

    public void ResetCombo()
    {
        //Debug.Log("Combo was Reset.");
        comboCount = 0;
        clock.Reset();
    }

    public void IncrementCombo()
    {
        ++comboCount;
        clock.Reset();

        onComboIncremented.Invoke();
        onComboIncrementedInt.Invoke(comboCount);
    }

    public void AddAwakeGauge(int comboCount)
    {
        float income = 0;
        for(int i = awakeBonuses.Count - 1; i >= 0; --i)
        {
            if(comboCount >= awakeBonuses[i].combo)
            {
                income = awakeBonuses[i].amount;
            }
        }
        awake.AddAwakeGauge(income);
    }
}
