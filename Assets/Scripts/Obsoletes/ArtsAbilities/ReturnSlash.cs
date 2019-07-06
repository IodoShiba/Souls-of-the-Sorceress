using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
namespace PlayerStates
{
    public class PlayerReturnSlash : State
    {
        [SerializeField] Rigidbody2D playerRb;
        [SerializeField] float hopImpact;
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
                return GetComponent<PlayerStates.PlayerSmashSlash>();
            }
            return null;
        }

        public override void Initialize()
        {
            t = 0;
            playerRb.AddForce(new Vector2(0, hopImpact));
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
public class ReturnSlash : ArtsAbility
{
    [SerializeField] float hopImpact;
    [SerializeField] float _motionLength;
    [SerializeField] AttackInHitbox attack;
    [SerializeField] Umbrella umbrella;
    [SerializeField] Rigidbody2D playerRb;
    float t = 0;

    protected override bool ShouldContinue(bool ordered)
    {
        return t < _motionLength;
    }

    protected override void OnInitialize()
    {
        attack.Activate();
        umbrella.StartCoroutineForEvent("PlayerReturnSlash");
        t = 0;
        playerRb.AddForce(new Vector2(0, hopImpact));
    }

    protected override void OnActive(bool ordered)
    {
        t += Time.deltaTime;
    }

    protected override void OnTerminate()
    {
        attack.Inactivate();
        umbrella.StopCoroutine("PlayerReturnSlash");
        umbrella.Default();
        t = 0;
    }
}
