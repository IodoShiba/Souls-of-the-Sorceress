using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "ScriptableObject/LootTable")]
public class LootTable : ScriptableObject
{
    [System.Serializable]
    private class LootTabelElement
    {
        public float dropTendencyWeight;
        public ItemBase item;
        [System.NonSerialized] public float _twSum;
    }

    [SerializeField] List<LootTabelElement> lootTabel;

    float tendencySum = 0;

    private void OnValidate()
    {
        tendencySum = 0;
        Prepare();
    }

    public ItemBase GetItem()
    {
        if(tendencySum == 0)
        {
            Prepare();
        }

        float randv = Random.Range(0, tendencySum);

        for (int i = 0; i < lootTabel.Count; ++i)
        {
            if(randv <= lootTabel[i]._twSum)
            {
                return lootTabel[i].item;
            }
        }
        return lootTabel.Count > 0 ? lootTabel[lootTabel.Count - 1].item : null;
    }

    public void Prepare()
    {
        if (lootTabel.Count == 0) { return; }

        lootTabel[0]._twSum = lootTabel[0].dropTendencyWeight;
        for(int i = 1; i < lootTabel.Count; ++i)
        {
            lootTabel[i]._twSum = lootTabel[i - 1]._twSum + lootTabel[i].dropTendencyWeight;
        }

        tendencySum = lootTabel[lootTabel.Count - 1]._twSum;
    }
}
