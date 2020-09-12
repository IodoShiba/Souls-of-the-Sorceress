using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cannon
{
    [SerializeField] Actor subject;
    [SerializeField] float launchSpeed;
    [SerializeField] float launchDegree;
    [SerializeField] Spawner2 spawner;

    public float LaunchSpeed { get => launchSpeed; set => launchSpeed = value; }
    public float LaunchDegree { get => launchDegree; set => launchDegree = value; }

    public Actor Launch(Actor subject, float speed, float degree)
    {
        float theta = Mathf.Deg2Rad * degree;
        var summoned = spawner.Summon(subject, spawner.transform.position);
        var rigidbody = summoned.GetComponent<Rigidbody2D>();
        rigidbody.velocity = new Vector2(Mathf.Cos(theta), Mathf.Sin(theta)) * speed;
        return summoned;
    }

    public Actor Launch(Actor subject) => Launch(subject, launchSpeed, launchDegree);
    public Actor Launch() => Launch(subject, LaunchSpeed, LaunchDegree);
}

public class AscCannon : FightActorStateConector
{
    [SerializeField] CannonDefaultState defaultState;
    [SerializeField] SmashedState smashed;

    public override ActorState DefaultState => defaultState;

    public override SmashedState Smashed => smashed;
    

    [System.Serializable]
    class CannonDefaultState : ActorState 
    {
        [SerializeField] float launchInterval;
        [SerializeField] Vector2 degreeRange;
        [SerializeField] Cannon cannon;
        [SerializeField] Actor launchedOriginal;

        float t = 0;

        protected override void OnInitialize()
        {
            t = launchInterval;
            base.OnInitialize();
        }

        protected override void OnActive()
        {
            t -= Time.deltaTime;
            if(t < 0)
            {
                cannon.LaunchDegree = Random.Range(degreeRange.x, degreeRange.y);
                cannon.Launch(launchedOriginal);
                t = launchInterval;
            }
        }
    }
}
