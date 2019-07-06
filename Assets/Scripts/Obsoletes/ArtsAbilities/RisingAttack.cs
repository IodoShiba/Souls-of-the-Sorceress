using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
namespace PlayerStates
{
    public class PlayerRisingAttack : State
    {
        [SerializeField] float risingSpeed;
        [SerializeField] float _motionLength;
        [SerializeField] AwakeMutableObject attackTrigger;
        private Rigidbody2D rb;
        private float t;
        // Use this for initialization
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }
        public override State Check()
        {
            if (t > _motionLength)
            {
                return GetComponent<PlayerGliding>();
            }
            else
            {
                return null;
            }
        }
        public override void Initialize()
        {

        }

        public override void Execute()
        {
            rb.velocity = new Vector2(0, risingSpeed);
            t += Time.deltaTime;
        }

        public override void Terminate()
        {
            rb.velocity = new Vector2(0, 0);
            t = 0;
        }
    }
}
*/
public class RisingAttack : ArtsAbility
{
    [SerializeField] float risingSpeed;
    [SerializeField] float _motionLength;
    [SerializeField] AttackInHitbox attack;
    [SerializeField] Umbrella umbrella;
    private Rigidbody2D rb;
    private float t;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    protected override bool ShouldContinue(bool ordered)
    {
        return t <= _motionLength;
    }

    protected override void OnInitialize()
    {
        attack.Activate();
        umbrella.StartCoroutine("PlayerRisingAttack");
        t = 0;
    }

    protected override void OnActive(bool ordered)
    {
        rb.velocity = Vector2.up * risingSpeed;
        t += Time.deltaTime;
    }

    protected override void OnTerminate()
    {
        attack.Inactivate();
        umbrella.StopCoroutine("PlayerRisingAttack");
        umbrella.Default();
        rb.velocity = Vector2.zero;
        t = 0;
        Debug.Log("a");
    }
}