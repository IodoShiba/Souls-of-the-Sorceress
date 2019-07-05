using UnityEngine;
using System.Collections;

public abstract class FightActorStateConector : ActorState.ActorStateConnector
{

    //public interface BearableState { }

    [System.Serializable]
    public class SmashedState : ActorState
    {
        [SerializeField] ActorFunction.HorizontalMoveMethod horizontalMove;
        IodoShiba.ManualUpdateClass.ManualClock clock = new IodoShiba.ManualUpdateClass.ManualClock();

        protected override bool ShouldCotinue() => clock.Clock < .5f;
        protected override void OnInitialize()
        {
            horizontalMove.enabled = false;
        }
        protected override void OnActive()
        {
            clock.Update();
        }
        protected override void OnTerminate(bool isNormal)
        {
            clock.Reset();
            horizontalMove.enabled = true;
        }
    }

    private bool bearAgainstAttack = false;
    protected override void Awake()
    {
        base.Awake();
        GetComponent<Actor>().OnAttacked.AddListener(() => { InterruptWith(Smashed); });
    }
    //[SerializeField] protected SmashedState smashed;

    public abstract SmashedState Smashed { get; }
    public bool BearAgainstAttack { get => bearAgainstAttack; set => bearAgainstAttack = value; }
}
