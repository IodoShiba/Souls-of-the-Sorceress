using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Async;
using System.Threading;

public class StageInfoView : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text stageTitle;
    [SerializeField] TMPro.TMP_Text stageDescription;
    [SerializeField] UnityEngine.UI.Image stageImage;
    // [SerializeField] RecordViewer[] recordViewers;
    // [SerializeField] StageRecordReader stageRecordReader;
    [SerializeField] RecordRowView recordRowView;
    [SerializeField] StageRecordBuffer stageRecordBuffer;
    [SerializeField] StageMetaData stageMetaData;

    CancellationTokenSource cancellationTokenSource;

    // public void ReadStageInfo(string stageId) => ReadStageInfo((StageMetaData.Stage)System.Enum.Parse(typeof(StageMetaData.Stage), stageId));
    public void ReadStageInfo(string stageId) 
    {
        ReadStageInfo((StageMetaData.Stage)System.Enum.Parse(typeof(StageMetaData.Stage), stageId));
    }

    public void ReadStageInfo(int stageId) => ReadStageInfo((StageMetaData.Stage)stageId);

    public void ReadStageInfo(StageMetaData.Stage stageId)
    {

        SetStageTitle(StageIdToStageTitle(stageId));

        StageMisc misc;
        if(stageMetaData.GetStageMisc(stageId, out misc))
        {
            SetStageDescription(misc.StageDescription);
            SetStageImage(misc.StageImage);
        }
        else
        {
            SetStageDescription("<No Misc Data found>");
            SetStageImage(null);
            Debug.LogError("No Misc Data found.");
        }

        if(cancellationTokenSource != null)
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
        }
        cancellationTokenSource = new CancellationTokenSource();

        //SetStageRecord(stageId, cancellationTokenSource.Token);//.Forget();
        StageRecordAccessor accessor;
        stageMetaData.GetStageRecordAccessor(stageId, out accessor);
        stageRecordBuffer.ReadOrDefault(accessor);
    }

    string StageIdToStageTitle(StageMetaData.Stage stageId) => 
            stageId == StageMetaData.Stage.Stage0 ? "Tutorial":
            stageId == StageMetaData.Stage.Stage1 ? "Stage 1":
            stageId == StageMetaData.Stage.Stage2 ? "Stage 2":
            stageId == StageMetaData.Stage.Stage3 ? "Stage 3":
            stageId == StageMetaData.Stage.Stage4 ? "Stage 4":
            stageId == StageMetaData.Stage.StageEX_1 ? "Stage EX-1":
            stageId == StageMetaData.Stage.StageEX_2 ? "Stage EX-2":
            ((System.Enum)stageId).ToString();

    void SetStageTitle(string title)
    {
        stageTitle.text = title;
    }

    void SetStageDescription(string description)
    {
        stageDescription.text = description;
    }

    void SetStageImage(Sprite sprite)
    {
        stageImage.sprite = sprite;
    }

    //async UniTask<UniRx.Unit>
    // void SetStageRecord(StageMetaData.Stage stageId, CancellationToken cancellationToken)
    // {

    //     StageRecordAccessor recordAccessor;
    //     stageMetaData.GetStageRecordAccessor(stageId, out recordAccessor);
    //     if(recordAccessor == null)
    //     {
    //         for(int i=0;i<3;++i)
    //         {
    //             recordViewers[i].SetRecordDisabled();
    //         }
    //         return ;// UniRx.Unit.Default;
    //     }

    //     try
    //     {
    //         //await stageRecordReader.ReadAsync(recordAccessor, cancellationToken);
    //         stageRecordReader.Read(recordAccessor);
    //     }
    //     catch(System.IO.FileNotFoundException ex)
    //     {
    //         for(int i=0;i<3;++i)
    //         {
    //             recordViewers[i].SetRecordDisabled();
    //         }
    //         return ;//UniRx.Unit.Default;
    //     }
    //     catch(System.IO.DirectoryNotFoundException ex)
    //     {
    //         for(int i=0;i<3;++i)
    //         {
    //             recordViewers[i].SetRecordDisabled();
    //         }
    //         return ;//UniRx.Unit.Default;
    //     }

    //     for(int i=0;i<3;++i)
    //     {
    //         StageRecord.Single recordSingle = stageRecordReader.Get(i);
    //         if(recordSingle.isValid)
    //         {
    //             recordViewers[i].SetRecord(stageId, recordSingle.defeatedCount, recordSingle.time, recordSingle.continueCount);
    //         }
    //         else
    //         {
    //             recordViewers[i].SetRecordDisabled();
    //         }
    //     }
        
    //     return ;//UniRx.Unit.Default;
    // }
}
