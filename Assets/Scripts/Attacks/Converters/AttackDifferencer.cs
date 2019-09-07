using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackDifferencer : AttackConverter
{
    [SerializeField] List<AttackData> attackDatas;
    [SerializeField] int useIndex;

    public int UseIndex
    {
        get => useIndex;
        set
        {
            if(useIndex < 0 || attackDatas.Count <= useIndex)
            {
                throw new System.IndexOutOfRangeException();
            }
            useIndex = value;
        }
    }

    public override bool Convert(AttackData value)
    {
        AttackData.Copy(value, attackDatas[useIndex]);
        return true;
    }

}
