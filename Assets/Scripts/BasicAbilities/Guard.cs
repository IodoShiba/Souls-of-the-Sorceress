using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStates
{
    public class PlayerGuard : State
    {
        [SerializeField] Rigidbody2D player;
        [SerializeField] Player playerData;
        
        public override State Check()
        {
            if(!(playerData.DoesUmbrellaWork() && Input.GetButton("Open Umbrella")))
            {
                return GetComponent<PlayerStates.PlayerOnGround>();
            }
            if (Input.GetButtonDown("Attack"))
            {
                return GetComponent<PlayerStates.PlayerTackle>();
            }
            return null;
        }

        public override void Initialize()
        {
        }

        public override void Execute()
        {
            player.velocity = new Vector2(0, 0);
        }

        public override void Terminate()
        {
        }
    }
}

//Playerに依存しないように書き直す
public class Guard : BasicAbility
{
    [SerializeField] Rigidbody2D player;
    [SerializeField] Player playerData;
    [SerializeField] Collider2D extensionCollider;
    [SerializeField] AttackInHitbox knockbackAttack;
    [SerializeField] Umbrella umbrella;

    protected override bool ShouldContinue(bool ordered)
    {
        return ordered && playerData.DoesUmbrellaWork();
    }
    protected override void OnInitialize()
    {
        extensionCollider.enabled = true;
        knockbackAttack.Activate();
        umbrella.PlayerGuard();
    }

    protected override void OnActive(bool ordered)
    {
        player.velocity = new Vector2(0, 0);
    }

    protected override void OnTerminate()
    {
        extensionCollider.enabled = false;
        knockbackAttack.Inactivate();
        umbrella.Default();
    }
}