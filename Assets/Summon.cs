using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Summon : ArtsAbility
{
    [System.Serializable]
    class SummonPositionParameter
    {
        public Vector2 position;
        public Vector2 randomRange;
    }
    
    [SerializeField] Enemy targetPrefab;

    [SerializeField] EnemyManager _manager;
    [SerializeField] List<SummonPositionParameter> summonPositions;

    protected override bool CanContinue(bool ordered)
    {
        return false;
    }

    protected override void OnInitialize()
    {
        foreach(var r in summonPositions)
        {
            _manager.Summon(targetPrefab,
                transform.position +
                    (Vector3)r.position +
                    new Vector3(Random.Range(-r.randomRange.x, r.randomRange.x),
                        Random.Range(-r.randomRange.y, r.randomRange.y), 0),
                Quaternion.identity);
        }
    }

}
