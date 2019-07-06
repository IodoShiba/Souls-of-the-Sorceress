using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStates
{
    public class PlayerDropAttack : State
    {
        [SerializeField] GroundSensor groundSensor;
        [SerializeField] float dropSpeed;
        private Rigidbody2D rb;

        // Use this for initialization
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }
        public override State Check()
        {
            if (groundSensor.IsOnGround)
            {
                return GetComponent<PlayerStates.PlayerOnGround>();
            }
            return null;
        }
        public override void Initialize()
        {
        }

        public override void Execute()
        {
            rb.velocity = new Vector2(0, -dropSpeed);
        }

        public override void Terminate()
        {
        }
    }
}

public class DropAttack : ArtsAbility
{
    [SerializeField] float dropSpeed;
    [SerializeField] float maxMotionLength;
    [SerializeField] AttackInHitbox attack;
    [SerializeField] GroundSensor groundSensor;
    [SerializeField] Umbrella umbrella;
    private Rigidbody2D rb;
    private float t = 0;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    protected override bool ShouldContinue(bool ordered)
    {
        return !(groundSensor.IsOnGround || t > maxMotionLength);
    }
    protected override void OnInitialize()
    {
        attack.Activate();
        umbrella.StartCoroutine("PlayerDropAttack");
        t = 0;
    }
    protected override void OnActive(bool ordered)
    {
        rb.velocity = new Vector2(0, -dropSpeed);
        t += Time.deltaTime;
    }
    protected override void OnTerminate()
    {
        attack.Inactivate();
        umbrella.StopCoroutine("PlayerDropAttack");
        umbrella.Default();
        t = 0;
    }
}

