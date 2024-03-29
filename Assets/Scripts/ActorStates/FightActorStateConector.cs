﻿using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Mortal))]
[RequireComponent(typeof(ActorFunction.Hitstop))]
public abstract class FightActorStateConector : ActorState.ActorStateConnector
{
    private bool bearAgainstAttack = false;
    protected override void Awake()
    {
        base.Awake();
        Mortal selfMortal = GetComponent<Mortal>();
        //selfMortal.OnAttackedCallbacks.AddListener((_) => { InterruptWith(Smashed); });
        selfMortal.OnHitstopGiven.AddListener(Smashed.SetHitstopTime);
    }

    public abstract SmashedState Smashed { get; }
    public virtual DeadState Dead { get => null; }
    public bool BearAgainstAttack { get => bearAgainstAttack; set => bearAgainstAttack = value; }
    public bool IsDead { get => Current is DeadState; }

    public void GoDead() 
    {
        InterruptWith(Dead);
    }

    [System.Serializable]
    public class SmashedState : ActorState
    {
        [SerializeField, UnityEngine.Serialization.FormerlySerializedAs("stunTime")] float stateSpan;
        [SerializeField] bool useInvincibleTime;
        [SerializeField] float invincibleTime;
        [SerializeField] ActorFunction.HorizontalMoveMethod horizontalMove;
        [SerializeField] bool disallowCross;

        IodoShibaUtil.ManualUpdateClass.ManualClock clock = new IodoShibaUtil.ManualUpdateClass.ManualClock();
        int originalLayer;
        float hitstopSpan = 0;

        Mortal selfMortal = null;
        Mortal SelfMortal { get => selfMortal == null ? (selfMortal = GameObject.GetComponent<Mortal>()) : selfMortal; }

        protected override bool ShouldCotinue() => clock.Clock < stateSpan + hitstopSpan;
        protected override void OnInitialize()
        {
            //Debug.Log("OnInitialize");
            if (!disallowCross)
            {
                //Debug.Log("!disallowCross");
                if (useInvincibleTime)
                {
                    //Debug.Log("useInvincibleTime");
                    SelfMortal.OrderInvincible(invincibleTime);
                }
                else
                {
                    originalLayer = GameObject.layer;
                    GameObject.layer = LayerMask.NameToLayer(LayerName.uncrossActor);
                }
            }
            if (horizontalMove != null) { horizontalMove.Use = false; }
        }
        protected override void OnActive()
        {
            clock.Update();
        }
        protected override void OnTerminate(bool isNormal)
        {
            if (!disallowCross && !useInvincibleTime)
            {
                GameObject.layer = originalLayer;
            }
            clock.Reset();
            if (horizontalMove != null) { horizontalMove.Use = true; }
        }
        public void SetHitstopTime(float time)
        {
            hitstopSpan = time;
        }
    }

    [System.Serializable]
    public class DeadState : ActorState
    {
        protected override bool ShouldCotinue() => true;
        protected override void OnInitialize()
        {
            Destroy(GameObject);
        }

        public override bool IsResistibleTo(Type actorStateType)
        {
            return true;
        }
    }
}
