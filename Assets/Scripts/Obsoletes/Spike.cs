using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : ArtsAbility
{
    [SerializeField] AttackInHitbox spikeAttack;
    [SerializeField] SpriteRenderer spikeSpriteRenderer;

    protected override bool ShouldContinue(bool ordered)
    {
        return ordered;
    }

    protected override void OnActive(bool ordered)
    {
        spikeAttack.Activate();
        spikeSpriteRenderer.enabled = true;
    }

    protected override void OnTerminate()
    {
        spikeAttack.Inactivate();
        spikeSpriteRenderer.enabled = false;
    }
}
