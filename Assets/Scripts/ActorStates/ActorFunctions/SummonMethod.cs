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

        [System.Serializable]
        class SubjectAndWeight
        {
            public Enemy subject;
            public float weight;
        }

        [SerializeField] List<SubjectAndWeight> targets;
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
                    _manager.Summon(SelectEnemy(fields),//fields.targetPrefab,
                        transform.position +
                            (Vector3)r.position +
                            new Vector3(Random.Range(-r.randomRange.x, r.randomRange.x)/2,
                                Random.Range(-r.randomRange.y, r.randomRange.y)/2, 0),
                        Quaternion.identity);
                }
            }

            Enemy SelectEnemy(SummonFields fields)
            {
                float sum = 0;
                for(int i = 0; i < fields.targets.Count; ++i)
                {
                    sum += fields.targets[i].weight;
                }

                float randf = Random.Range(0,sum);
                for (int i = 0; i < fields.targets.Count; ++i)
                {
                    randf -= fields.targets[i].weight;
                    if (randf <= 0)
                    {
                        return fields.targets[i].subject;
                    }
                }
                return null;
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
