using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Mortal))]
public abstract class FightActorStateConector : ActorState.ActorStateConnector
{

    //public interface BearableState { }

    [System.Serializable]
    public class SmashedState : ActorState
    {
        [SerializeField] float stunTime;
        [SerializeField] ActorFunction.HorizontalMoveMethod horizontalMove;
        [SerializeField] bool disallowCross;

        IodoShibaUtil.ManualUpdateClass.ManualClock clock = new IodoShibaUtil.ManualUpdateClass.ManualClock();
        int originalLayer;

        protected override bool ShouldCotinue() => clock.Clock < stunTime;
        protected override void OnInitialize()
        {
            if (!disallowCross)
            {
                originalLayer = GameObject.layer;
                GameObject.layer = LayerMask.NameToLayer("Smashed Actor");
            }
            if (horizontalMove != null) { horizontalMove.enabled = false; }
        }
        protected override void OnActive()
        {
            clock.Update();
        }
        protected override void OnTerminate(bool isNormal)
        {
            clock.Reset();
            if (!disallowCross)
            {
                GameObject.layer = originalLayer;
            }
            if (horizontalMove != null) { horizontalMove.enabled = true; }
        }
    }

    private bool bearAgainstAttack = false;
    protected override void Awake()
    {
        base.Awake();
        GetComponent<Mortal>().OnAttackedCallbacks.AddListener(() => { InterruptWith(Smashed); });
    }
    //[SerializeField] protected SmashedState smashed;

    public abstract SmashedState Smashed { get; }
    public bool BearAgainstAttack { get => bearAgainstAttack; set => bearAgainstAttack = value; }
}
