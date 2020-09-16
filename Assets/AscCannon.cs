using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cannon
{
    [SerializeField] Actor subject;
    [SerializeField] float launchSpeed;
    [SerializeField] float offsetRadius;
    [SerializeField] float launchDegree;
    [SerializeField] Spawner2 spawner;

    public float LaunchSpeed { get => launchSpeed; set => launchSpeed = value; }
    public float LaunchDegree { get => launchDegree; set => launchDegree = value; }

    public Actor Launch(Actor subject, float speed, float degree)
    {
        float theta = Mathf.Deg2Rad * degree;
        Vector2 dir = new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));
        var summoned = spawner.Summon(subject, (Vector2)spawner.transform.position + dir * offsetRadius);
        var rigidbody = summoned.GetComponent<Rigidbody2D>();
        rigidbody.velocity = dir * speed;
        return summoned;
    }

    public Actor Launch(Actor subject) => Launch(subject, launchSpeed, GetDegree());
    public Actor Launch() => Launch(subject, LaunchSpeed, GetDegree());

    float GetDegree() => launchDegree;
}

public class AscCannon : FightActorStateConector
{
    [SerializeField] CannonDefaultState cannonDefault;
    [SerializeField] SmashedState smashed;

    public override ActorState DefaultState => cannonDefault;

    public override SmashedState Smashed => smashed;

    protected override void BeforeStateUpdate()
    {
        base.BeforeStateUpdate();
    }

    [System.Serializable]
    class CannonDefaultState : DefaultState 
    {
        [SerializeField] float launchInterval;
        [SerializeField] Vector2 degreeRange;
        [SerializeField] Cannon cannon;

        float t = 0;

        protected override void OnInitialize()
        {
            t = launchInterval;
            base.OnInitialize();
        }

        protected override void OnActive()
        {
            base.OnActive();

            Debug.Log("Cannon Running");
            t -= Time.deltaTime;
            if(t < 0)
            {
                cannon.LaunchDegree = Random.Range(degreeRange.x, degreeRange.y);
                cannon.Launch();
                t = launchInterval;
            }
        }

    }
}
