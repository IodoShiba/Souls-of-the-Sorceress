﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorEnemyElement {
    public class AscEnemyElement : FightActorStateConector
    {
        [SerializeField] float bulletSpeed;
        [SerializeField] ElementAI ai;

        [SerializeField] ElementDefault elementDefault;
        [SerializeField] SmashedState smashed;

        public override ActorState DefaultState => elementDefault;

        public override SmashedState Smashed => smashed;

        protected override void BeforeStateUpdate()
        {
            ai.Decide();
        }

        [System.Serializable]
        private class ElementDefault : DefaultState
        {
            [SerializeField] float moveSpeedX;
            [SerializeField] ActorFunction.VelocityAdjuster velocityAdjuster;
            [SerializeField] ShootBullet shootBullet;

            AscEnemyElement connectorElem;
            AscEnemyElement ConnectorElem { get => connectorElem == null ? (connectorElem = Connector as AscEnemyElement) : connectorElem; }

            protected override void OnInitialize()
            {
                base.OnInitialize();
            }
            protected override void OnActive()
            {
                velocityAdjuster.Fields.Velocity = ConnectorElem.ai.MoveSign * moveSpeedX * Vector2.right;
                velocityAdjuster.ManualUpdate();

                Vector2 shootDirection = ConnectorElem.ai.ShootDirection;
                if (shootDirection != Vector2.zero)
                {
                    shootBullet.Use(ConnectorElem.bulletSpeed * shootDirection);
                }
            }
        }

    }
}