using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HugeMashAI : AI
{
    [SerializeField] Transform playerTransform;
    [SerializeField] float maxChaseRadius;
    [SerializeField] float minChaseRadius;
    [SerializeField] float summonCycle;
    [SerializeField] int maxNearbyEnemyCount;
    [SerializeField] Sensor sensor;
    float t = 0;

    int moveSign;
    bool doSummon;

    public int MoveSign { get => moveSign; }
    public bool DoSummon { get => doSummon; }

    public override void Decide()
    {
        ResetOutputs();
        Vector2 vector = playerTransform.position - transform.position;
        if (minChaseRadius < vector.magnitude && vector.magnitude < maxChaseRadius)
        {
            moveSign = System.Math.Sign(vector.x);

        }

        t += Time.deltaTime;
        if (t > summonCycle)
        {
            if (sensor.DetectCount < maxNearbyEnemyCount + 1)
            {
                doSummon = true;
            }
            t -= summonCycle;
        }
    }

    void ResetOutputs()
    {
        moveSign = 0;
        doSummon = false;
    }
}
