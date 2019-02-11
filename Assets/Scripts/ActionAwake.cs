using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Math;

[DisallowMultipleComponent]
public class ActionAwake : MonoBehaviour
{
    public enum AwakeGrades
    {
        ordinary,
        awaken,
        blueAwaken
    }
    [SerializeField,Range(0,1)] float awakeGauge;
    [SerializeField] float awakeGaugeDecreaseSpeed;
    [SerializeField] PlayerStates.Awakening.Ordinary _awakeningState_ordinary;
    [SerializeField] StateManager2 awakeningState2;
    [SerializeField] EnemyManager enemyManager;
    private bool isActing = false;
    AwakeGrades awakeGrade = AwakeGrades.ordinary;

    public bool IsActing { get => isActing;}
    public AwakeGrades AwakeGrade { get => awakeGrade; }

    private void Start()
    {
        enemyManager.AddEnemyDyingListener(() => AddAwakeGauge(.1f));
    }

    // Update is called once per frame
    void Update()
    {
        if (isActing)
        {
            awakeGauge = System.Math.Max(awakeGauge - awakeGaugeDecreaseSpeed * Time.deltaTime, 0);
            if(awakeGauge == 0)
            {
                isActing = false;
                awakeGrade = AwakeGrades.ordinary;
            }
        }
    }

    public void Action()
    {
        if (!isActing && awakeGauge >= .5) {
            isActing = true;
            if (awakeGauge>=1)
            {
                awakeGrade = AwakeGrades.blueAwaken;
            }
            else
            {
                awakeGrade = AwakeGrades.awaken;
            }
        }
        else if(isActing)
        {
            isActing = false;
            awakeGrade = AwakeGrades.ordinary;
        }
    }
    
    public void AddAwakeGauge(float amount)
    {
        if (!isActing)
        {
            awakeGauge = Max(0, Min(awakeGauge + amount, 1));
        }
    }

    public string _DebugOutput() { return $"Awake Gauge:{awakeGauge} (0.5 ≦ this value < 1 : Awake, this value = 1 : Blue Awake)\n"; }
}
