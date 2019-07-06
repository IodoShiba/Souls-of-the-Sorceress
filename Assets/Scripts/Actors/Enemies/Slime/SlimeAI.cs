using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeAI : AI
{
    [SerializeField] float searchXRange;
    [SerializeField] Vector2 changeDirectionIntervalRange;
    Transform player;
    float leftTimeToChangeInterval;
    int direction = 0;
    int moveSign;
    bool spike;

    public int MoveSign { get => moveSign; }
    public bool Spike { get => spike; }

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
        ResetChangeInterval();
        direction = Random.Range(0, 2) * 2 - 1;
        ResetOutputs();
    }

    public override void Decide()
    {
        ResetOutputs();

        if (System.Math.Abs(player.position.x - transform.position.x) < searchXRange)
        {
            spike = true;
        }

        if (leftTimeToChangeInterval <= 0)
        {
            direction *= -1;
            ResetChangeInterval();
        }
        moveSign = direction;

        leftTimeToChangeInterval -= Time.deltaTime;
    }

    public void ResetOutputs()
    {
        moveSign = 0;
        spike = false;
    }
    private void ResetChangeInterval() => leftTimeToChangeInterval = Random.Range(changeDirectionIntervalRange.x, changeDirectionIntervalRange.y);
}
