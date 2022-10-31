using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UniRx.Async;

public class StageRecordBuffer : MonoBehaviour
{
    [System.Serializable]
    public class UnityEvent_int_StageRecordSingle_StageMetaDataStage : UnityEvent<int, StageRecord.Single, StageMetaData.Stage>{}

    public UnityEvent_int_StageRecordSingle_StageMetaDataStage onRecordUpdated;

    StageMetaData.Stage stage = StageMetaData.Stage.None;
    StageRecord record;

    public bool Available {get => record != null;}

    public StageRecord.Single Get(int index)
    {
        if (!Available) {return null;}

        if(index < 0 || StageRecord.MAX_RECORD_COUNT < index)
        {
            throw new System.IndexOutOfRangeException();
        }

        return record.records[index];
    }

    public void Set(int index, int defeatedCount, float time, int continueCount)
    {
        if (!Available) {return;}

        if(index < 0 || StageRecord.MAX_RECORD_COUNT < index)
        {
            throw new System.IndexOutOfRangeException();
        }

        record.records[index].Set(defeatedCount, time, continueCount);

        onRecordUpdated.Invoke(index, record.records[index], stage);
    }

    public async UniTask<UniRx.Unit> ReadAsync(StageRecordAccessor accessor, CancellationToken cancellationToken) 
    {
        record = await accessor.LoadAsync(cancellationToken);
        return UniRx.Unit.Default;
    }

    public void ReadOrDefault(StageRecordAccessor accessor)
    {
        record = accessor.Load();
        stage = accessor.Stage;

        for(int i=0;i<3;++i)
        {
            onRecordUpdated.Invoke(i, record.records[i], stage);
        }
    }

    public void Write(StageRecordAccessor accessor)
    {
        accessor.Save(record);
    }
}
