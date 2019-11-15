using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

/// <summary>
/// 同時巻き込みで覚醒ゲージ上昇量を増加させる
/// </summary>
public class ComboCalculator : MonoBehaviour
{

    [System.Serializable] class UnityEvent_int : UnityEngine.Events.UnityEvent<int> { }

    [SerializeField] AnimationCurve awakeIncomeCurve;
    [SerializeField] ActionAwake awake;
    [SerializeField] UnityEngine.Events.UnityEvent onComboIncremented;
    [SerializeField] UnityEvent_int onComboIncrementedInt;
    [SerializeField,DisabledField] int comboCount = 0;
    float addedAmount;

    bool IsInCombo { get => comboCount > 0; }
    
    void Awake()
    {
        comboCount = 0;
        addedAmount = 0;
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(0, 0, Screen.width, Screen.height), comboCount.ToString());
    }

    public void ResetCombo()
    {
        comboCount = 0;
        addedAmount = 0;
    }

    public void IncrementCombo()
    {
        float add = AwakeAddAmount(comboCount);
        ++comboCount;

        onComboIncremented.Invoke();
        onComboIncrementedInt.Invoke(comboCount);
    }

    float AwakeAddAmount(int combos)
    {
        if(awakeIncomeCurve.length == 0) { return 0; }
        if (combos < 1) { combos = 1; }

        float maxTime = awakeIncomeCurve.keys[awakeIncomeCurve.length - 1].time;
        return awakeIncomeCurve.Evaluate(Mathf.Min(combos, maxTime));
    }

    public void AddAwakeGauge(int comboCount)
    {
        float add = AwakeAddAmount(comboCount);
        float income = add - addedAmount;
        addedAmount = add;
        Debug.Log("Awake add:" + add);
        awake.AddAwakeGauge(income);
    }
}
