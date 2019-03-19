using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStates {
    public class PlayerAerialSlash : State
    {
        [SerializeField] Rigidbody2D playerRb;
        [SerializeField] GroundSensor groundSensor;
        [SerializeField] float _motionLength;
        float t = 0;

        public override State Check()
        {
            if (t > _motionLength || groundSensor.IsOnGround) 
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
            //playerRb.velocity = Vector2.zero;
            t += Time.deltaTime;
        }

        public override void Terminate()
        {
            t = 0;
        }
    }
}

public class AerialSlash : ArtsAbility
{
    [SerializeField] float _motionLength;
    [SerializeField] Rigidbody2D playerRb;
    [SerializeField] GroundSensor groundSensor;
    [SerializeField] AttackInHitbox attack;
    [SerializeField] Umbrella umbrella;
    float t = 0;
    protected override bool CanContinue(bool ordered)
    {
        return t < _motionLength && !groundSensor.IsOnGround;
    }
    protected override void OnInitialize()
    {
        attack.Activate();
        umbrella.StartCoroutineForEvent("PlayerAerialSlash");
        t = 0;
    }
    protected override void OnActive(bool ordered)
    {
        t += Time.deltaTime;
    }
    protected override void OnTerminate()
    {
        umbrella.StopCoroutine("PlayerAerialSlash");
        umbrella.Default();
        t = 0;
    }
}
