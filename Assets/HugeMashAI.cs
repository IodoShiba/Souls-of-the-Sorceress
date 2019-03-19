using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HugeMashAI : AI
{
    [SerializeField] Transform playerTransform;
    [SerializeField] float maxChaseRadius;
    [SerializeField] float minChaseRadius;
    [SerializeField] float summonCycle;
    float t = 0;

    HorizontalMove horizontalMove;
    Summon summon;

    private void Awake()
    {
        horizontalMove = GetComponent<HorizontalMove>();
        summon = GetComponent<Summon>();
    }

    public override void AskDecision()
    {
        Vector2 vector = playerTransform.position - transform.position;
        if (minChaseRadius < vector.magnitude && vector.magnitude < maxChaseRadius)
        {
            horizontalMove.SetParams(System.Math.Sign(vector.x));
            horizontalMove.SendSignal();
        }

        t += Time.deltaTime;
        if (t > summonCycle)
        {
            t -= summonCycle;
            summon.SendSignal();
        }
    }
}
