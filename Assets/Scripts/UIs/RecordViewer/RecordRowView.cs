using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class RecordRowView : MonoBehaviour
{
    [SerializeField] RecordViewer[] recordViewers;
    [SerializeField] StageMetaData stageMetaData;

    //[SerializeField] StageRecordBuffer stageRecordBuffer;

    // StageMetaData.Stage currectStageId = StageMetaData.Stage.None;

    public void SetRecordSingle(int index, StageRecord.Single single)
    {
        SetRecordSingle(index, single, stageMetaData.GetCurrentStage());
    }

    public void SetRecordSingle(int index, StageRecord.Single single, StageMetaData.Stage stage)
    {
        recordViewers[index].SetRecord(stage, single);
    }
    // public void SetStageRecord(StageMetaData.Stage stageId)
    // {
    //     if(stageId != currectStageId)
    //     {
    //         StageRecordAccessor recordAccessor;
    //         stageMetaData.GetStageRecordAccessor(stageId, out recordAccessor);
    //         if(recordAccessor == null)
    //         {
    //             for(int i=0;i<3;++i)
    //             {
    //                 recordViewers[i].SetRecordDisabled();
    //             }
    //             return ;// UniRx.Unit.Default;
    //         }

    //         try
    //         {
    //             //await stageRecordReader.ReadAsync(recordAccessor, cancellationToken);
    //             stageRecordBuffer.Read(recordAccessor);
    //         }
    //         catch(System.IO.FileNotFoundException ex)
    //         {
    //             for(int i=0;i<3;++i)
    //             {
    //                 recordViewers[i].SetRecordDisabled();
    //             }
    //             return ;//UniRx.Unit.Default;
    //         }
    //         catch(System.IO.DirectoryNotFoundException ex)
    //         {
    //             for(int i=0;i<3;++i)
    //             {
    //                 recordViewers[i].SetRecordDisabled();
    //             }
    //             return ;//UniRx.Unit.Default;
    //         }
    //     }
    //     currectStageId = stageId;

    //     UpdateStageRecord();
        
    //     return ;//UniRx.Unit.Default;
    // }

    // public void UpdateStageRecord()
    // {
    //     for(int i=0;i<3;++i)
    //     {
    //         StageRecord.Single recordSingle = stageRecordBuffer.Get(i);
    //         if(recordSingle.isValid)
    //         {
    //             recordViewers[i].SetRecord(currectStageId, recordSingle.defeatedCount, recordSingle.time, recordSingle.continueCount);
    //         }
    //         else
    //         {
    //             recordViewers[i].SetRecordDisabled();
    //         }
    //     }
    // }
}
