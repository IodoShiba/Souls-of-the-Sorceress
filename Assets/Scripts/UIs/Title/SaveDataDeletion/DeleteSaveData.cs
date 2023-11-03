using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteSaveData : MonoBehaviour
{
    [SerializeField] private StageRecordAccessor[] stageRecordAccessors;

    public void Delete()
    {
        for (int i = 0; i < stageRecordAccessors.Length; ++i)
        {
            stageRecordAccessors[i].Delete();
        }
    }
}
