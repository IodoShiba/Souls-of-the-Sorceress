using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Math;

[DisallowMultipleComponent]
public class ActionAwake : MonoBehaviour,SaveData.IPlayerAwakeCareer
{
    public class Viewer : MonoBehaviour
    {
        [SerializeField] protected ActionAwake target;
        protected float AwakeGauge { get => target.awakeGauge; }
    }
    public enum AwakeLevels
    {
        ordinary,
        awaken,
        blueAwaken
    }

    [SerializeField,Range(0,1)] float awakeGauge;
    [SerializeField] float awakeGaugeDecreaseSpeed;
    [SerializeField, Range(0, 4)] int requiredProgressLevelToBreakLimitation;
    [SerializeField] ActorSarah.ActorStateConnectorSarah ascSarah;
    [SerializeField,DisabledField]private bool isActive = false;
    AwakeLevels awakeLevel = AwakeLevels.ordinary;

    public bool IsActive { get => isActive;}
    public AwakeLevels AwakeLevel { get => awakeLevel; }
    public float MaxGauge { get => ascSarah.ProgressLevel >= requiredProgressLevelToBreakLimitation ? 1 : .5f; }

    private void Start()
    {
        awakeLevel = AwakeLevels.ordinary;
        //enemyManager.AddEnemyDyingListener(() => { AddAwakeGauge(.1f);Debug.Log("Awake Gauge Added"); });
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            awakeGauge = System.Math.Max(awakeGauge - awakeGaugeDecreaseSpeed * Time.deltaTime, 0);
            if(awakeGauge == 0)
            {
                isActive = false;
                awakeLevel = AwakeLevels.ordinary;
            }
        }
    }

    public void SwitchActivate()
    {
        if (!isActive && awakeGauge >= .5f) {
            isActive = true;
            if (awakeGauge >= 1) 
            {
                awakeLevel = AwakeLevels.blueAwaken;
            }
            else
            {
                awakeLevel = AwakeLevels.awaken;
            }
        }
        else if(isActive)
        {
            isActive = false;
            awakeLevel = AwakeLevels.ordinary;
        }
    }
    
    public void AddAwakeGauge(float amount)
    {
        if (!isActive)
        {
            awakeGauge = Mathf.Clamp(awakeGauge + amount, 0, MaxGauge);
        }
    }

    public string _DebugOutput() { return $"Awake Gauge:{awakeGauge} (0.5 ≦ a < 1 : Awake, a = 1 : Blue Awake)\n"; }

    public void Restore(float data)
    {
        awakeGauge = data;
    }

    public void Store(SaveData target, Action<float> setter)
    {
        setter(awakeGauge);
    }
}
