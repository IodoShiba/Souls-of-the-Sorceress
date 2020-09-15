using UnityEngine;

namespace CommonActorState
{
    [System.Serializable]
    public class Explosion : ActorState
    {
        [SerializeField] float timeSpan;
        [SerializeField] AttackInHitbox explosion;
        float t = 0;

        public bool Complete { get => t > timeSpan; }

        protected override bool ShouldCotinue() => true;

        protected override void OnInitialize()
        {
            t = 0;
            explosion.Activate();
        }

        protected override void OnActive()
        {
            t += Time.deltaTime;
        }

        public override bool IsResistibleTo(System.Type actorStateType)
        {
            return typeof(FightActorStateConector.SmashedState).IsAssignableFrom(actorStateType);
        }
    }
}