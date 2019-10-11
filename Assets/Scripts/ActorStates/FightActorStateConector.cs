using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Mortal))]
public abstract class FightActorStateConector : ActorState.ActorStateConnector
{
    private bool bearAgainstAttack = false;
    protected override void Awake()
    {
        base.Awake();
        GetComponent<Mortal>().OnAttackedCallbacks.AddListener(() => { InterruptWith(Smashed); });
    }

    public abstract SmashedState Smashed { get; }
    public virtual DeadState Dead { get => null; }
    public bool BearAgainstAttack { get => bearAgainstAttack; set => bearAgainstAttack = value; }
    public bool IsDead { get => Current is DeadState; }

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

        Mortal selfMortal = null;
        Mortal SelfMortal { get => selfMortal == null ? (selfMortal = GameObject.GetComponent<Mortal>()) : selfMortal; }

        protected override bool ShouldCotinue() => clock.Clock < stateSpan;
        protected override void OnInitialize()
        {
            if (!disallowCross)
            {
                if (useInvincibleTime)
                {
                    SelfMortal.OrderInvincible(invincibleTime);
                }
                else
                {
                    originalLayer = GameObject.layer;
                    GameObject.layer = LayerMask.NameToLayer(LayerName.uncrossActor);
                }
            }
            if (horizontalMove != null) { horizontalMove.enabled = false; }
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
            if (horizontalMove != null) { horizontalMove.enabled = true; }
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
