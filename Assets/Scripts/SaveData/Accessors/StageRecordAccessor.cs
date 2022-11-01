using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SaveData/StageRecordAccessor")]
public class StageRecordAccessor : SaveDataFileAccessor<StageRecord>
{
    [SerializeField] StageMetaData.Stage stage;

    public StageMetaData.Stage Stage {get => stage;}
}

[System.Serializable]
public class StageRecord
{
    public const int MAX_RECORD_COUNT = 3;
    public static readonly StageRecord Default = new StageRecord();
    public Single[] records;

    public StageRecord()
    {
        records = new Single[MAX_RECORD_COUNT];
        for(int i=0;i<records.Length;++i)
        {
            records[i] = new Single();
        }
    }

    [System.Serializable]
    public class Single
    {
        public bool isValid = false;
        public int defeatedCount;
        public float time; 
        public int continueCount;

        public void Set(int defeatedCount, float time, int continueCount)
        {
            this.defeatedCount = defeatedCount;
            this.time = time;
            this.continueCount = continueCount;
            this.isValid = true;
        }
    }
}
