using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
namespace PlayerStates
{
    public class PlayerVerticalSlash : State
    {
        [SerializeField] float _motionLength;
        [SerializeField] float nextAttackRestrictedTime;
        float t = 0;

        public override State Check()
        {
            if (t > _motionLength)
            {
                return GetComponent<PlayerStates.EndOfAction>();
            }
            else if (t > nextAttackRestrictedTime && Input.GetButtonDown("Attack"))
            {
                return GetComponent<PlayerStates.PlayerReturnSlash>();
            }
            
            return null;
        }

        public override void Initialize()
        {
            t = 0;
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
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
public class VerticalSlash : ArtsAbility
{
    [SerializeField] float _motionLength;
    [SerializeField] AttackInHitbox attack;
    [SerializeField] Umbrella umbrella;
    float t = 0;

    protected override bool ShouldContinue(bool ordered)
    {
        return t < _motionLength;
    }

    protected override void OnInitialize()
    {
        attack.Activate();
        umbrella.StartCoroutineForEvent("PlayerVerticalSlash");
        t = 0;
    }

    protected override void OnActive(bool ordered)
    {
        t += Time.deltaTime;
    }

    protected override void OnTerminate()
    {
        attack.Inactivate();
        umbrella.StopCoroutine("PlayerVerticalSlash");
        umbrella.Default();
        t = 0;
    }
}