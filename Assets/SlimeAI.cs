using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeAI : AI
{
    [SerializeField] float searchXRange;
    Transform player;
    Spike spike;

    private void Awake()
    {
        spike = GetComponent<Spike>();
        player = GameObject.FindWithTag("Player").transform;
    }

    public override void AskDecision()
    {
        if (System.Math.Abs(player.position.x - transform.position.x) < searchXRange)
        {
            spike.SendSignal();
        }
    }
}
