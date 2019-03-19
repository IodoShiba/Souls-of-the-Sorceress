using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
namespace PlayerStates
{
    public class PlayerSmashSlash : State
    {
        [SerializeField] float _motionLength;
        float t = 0;

        public override State Check()
        {
            if (t > _motionLength)
            {
                return GetComponent<PlayerStates.EndOfAction>();
            }
            return null;
        }

        public override void Initialize()
        {
            t = 0;
        }

        public override void Execute()
        {
            t += Time.deltaTime;
        }

        public override void Terminate()
        {
            t = 0;
        }
    }
}
*/
public class SmashSlash : ArtsAbility
{
    [SerializeField] float _motionLength;
    [SerializeField] AttackInHitbox attack;
    [SerializeField] Umbrella umbrella;
    float t = 0;

    protected override bool CanContinue(bool ordered)
    {
        return t < _motionLength;
    }

    protected override void OnInitialize()
    {
        attack.Activate();
        umbrella.StartCoroutineForEvent("PlayerSmashSlash");
        t = 0;
    }

    protected override void OnActive(bool ordered)
    {
        t += Time.deltaTime;
    }

    protected override void OnTerminate()
    {
        umbrella.StopCoroutine("PlayerSmashSlash");
        umbrella.Default();
        t = 0;
    }
}