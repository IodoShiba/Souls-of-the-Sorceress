using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorEnemyElement {
    public class AscEnemyElement : FightActorStateConector
    {
        [SerializeField] float bulletSpeed;
        [SerializeField] float noAttackTimeAfterSmashed;
        [SerializeField] ElementAI ai;

        [SerializeField] ElementDefault elementDefault;
        [SerializeField] ElementSmashed smashed;
        [SerializeField] AttackInHitbox attack;
        [SerializeField] Animator elementAnimator;

        float attackInterval = 0;

        public override ActorState DefaultState => elementDefault;
        public override SmashedState Smashed => smashed;

        protected override void BeforeStateUpdate()
        {
            ai.Decide();
            if(attackInterval > 0)
            {
                attackInterval -= Time.deltaTime;
                if(attackInterval <= 0)
                {
                    attackInterval = 0;
                    attack.Activate();
                }
            }
        }

        public void StartAttackInterval()
        {
            attack.Inactivate();
            attackInterval = noAttackTimeAfterSmashed;
        }

        [System.Serializable]
        private class ElementDefault : DefaultState
        {
            [SerializeField] float moveSpeedX;
            [SerializeField] float moveSpeedY;
            [SerializeField] ActorFunction.VelocityAdjuster velocityAdjuster;
            [SerializeField] ShootBullet shootBullet;

            AscEnemyElement connectorElem;
            AscEnemyElement ConnectorElem { get => connectorElem == null ? (connectorElem = Connector as AscEnemyElement) : connectorElem; }

            protected override void OnInitialize()
            {
                base.OnInitialize();
                velocityAdjuster.Method.enabled = true;
                ConnectorElem.elementAnimator.Play("Idle");
            }
            protected override void OnActive()
            {
                Vector2Int ms = ConnectorElem.ai.MoveSigns;
                velocityAdjuster.Fields.Velocity = new Vector2( ms.x * moveSpeedX, ms.y * moveSpeedY);
                velocityAdjuster.ManualUpdate();

                Vector2 shootDirection = ConnectorElem.ai.ShootDirection;
                if (shootDirection != Vector2.zero)
                {
                    shootBullet.Use(ConnectorElem.bulletSpeed * shootDirection);
                }
            }
        }

        [System.Serializable]
        private class ElementSmashed : SmashedState
        {
            [SerializeField] ActorFunction.VelocityAdjusterFields.Method velocityAdjuster;

            AscEnemyElement connectorElem;
            AscEnemyElement ConnectorElem { get => connectorElem == null ? (connectorElem = Connector as AscEnemyElement) : connectorElem; }

            protected override void OnInitialize()
            {
                velocityAdjuster.enabled = false;
                base.OnInitialize();
                ConnectorElem.StartAttackInterval();

                ConnectorElem.elementAnimator.Play("Smashed");
            }

        }
    }
}
