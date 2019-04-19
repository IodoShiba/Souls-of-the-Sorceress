using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeAI : AI
{
    [SerializeField] float searchXRange;
    [SerializeField] Vector2 changeDirectionIntervalRange;
    Transform player;
    HorizontalMove horizontalMove;
    Spike spike;
    float leftTimeToChangeInterval;
    int direction = 0;

    private void Awake()
    {
        horizontalMove = GetComponent<HorizontalMove>();
        spike = GetComponent<Spike>();
        player = GameObject.FindWithTag("Player").transform;
        ResetChangeInterval();
        direction = Random.Range(0, 2) * 2 - 1;
    }

    public override void AskDecision()
    {
        if (System.Math.Abs(player.position.x - transform.position.x) < searchXRange)
        {
            spike.SendSignal();
        }

        if (leftTimeToChangeInterval <= 0)
        {
            direction *= -1;
            ResetChangeInterval();
        }
        horizontalMove.SetParams(direction);
        horizontalMove.SendSignal();

        leftTimeToChangeInterval -= Time.deltaTime;
    }

    private void ResetChangeInterval() => leftTimeToChangeInterval = Random.Range(changeDirectionIntervalRange.x, changeDirectionIntervalRange.y);
}
