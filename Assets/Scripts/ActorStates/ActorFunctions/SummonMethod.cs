using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorFunction
{
    [System.Serializable]
    public class SummonFields : ActorFunctionFields
    {
        [System.Serializable]
        class SummonPositionParameter
        {
            public Vector2 position;
            public Vector2 randomRange;
        }

        [SerializeField] Enemy targetPrefab;
        [SerializeField] List<SummonPositionParameter> summonPositions;

        public class Method : ActorFunctionMethod<SummonFields>
        {
            [SerializeField] EnemyManager _manager;
            public override void ManualUpdate(in SummonFields fields) { }

            public void ManualUpdate(in SummonFields fields,bool use)
            {
                if (!use) { return; }

                foreach (var r in fields.summonPositions)
                {
                    _manager.Summon(fields.targetPrefab,
                        transform.position +
                            (Vector3)r.position +
                            new Vector3(Random.Range(-r.randomRange.x, r.randomRange.x),
                                Random.Range(-r.randomRange.y, r.randomRange.y), 0),
                        Quaternion.identity);
                }
            }
        }
    }

    public class SummonMethod : SummonFields.Method
    {
    }

    [System.Serializable]
    public class Summon : ActorFunction<SummonFields,SummonFields.Method>
    {
        public void ManualUpdate(bool use)
        {
            Method.ManualUpdate(Fields, use);
        }
    }
}
