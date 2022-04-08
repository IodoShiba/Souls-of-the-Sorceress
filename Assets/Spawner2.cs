using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner2 : MonoBehaviour
{
    [System.Serializable] class UnityEvent_Actor : UnityEngine.Events.UnityEvent<Actor> {}

    [SerializeField] Actor subject;
    [SerializeField] Vector3 offset;
    [SerializeField] UnityEvent_Actor initializer;

    public Enemy SummonEnemy(Enemy enemy, Vector2 position)
    {
        var summoned = EnemyManager.Instance.Summon(enemy, position, Quaternion.identity);
        return summoned;
    }

    public Actor Summon(Actor actor, Vector3 position)
    {
        var summoned = Instantiate(actor, position, Quaternion.identity);

        return summoned;
    }

    public Actor SummonRange(Actor actor, Vector3 centerPos, Vector2 randRange)
        => Summon(
            actor, 
            centerPos + new Vector3(
                Random.Range(-randRange.x, randRange.x), 
                Random.Range(-randRange.y, randRange.y), 
                0
            )
        );

    public void SummonAction(Actor actor, Vector3 position) => initializer.Invoke(Summon(actor, position));
    public void SummonAction(Actor actor) => SummonAction(actor, transform.position + offset);
    public void SummonAction(Vector3 position) => SummonAction(subject, position);
    public void SummonAction() => SummonAction(subject, transform.position + offset);
}
