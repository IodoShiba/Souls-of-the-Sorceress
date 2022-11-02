using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UniRx.Async;
using UniRx;

public class RecordUpdateModel : MonoBehaviour
{
    [System.Serializable]
    public class UnityEvent_int : UnityEvent<int>{}

    public UnityEvent_int onRecordUpdated;

    StageRecord.Single recordYours;

    [SerializeField] StageMetaData stageMetaData;
    [SerializeField] RecordViewer recordYoursView;
    [SerializeField] RecordRowView recordRowView;
    [SerializeField] StageRecordBuffer stageRecordBuffer;
    [SerializeField] SotS.UI.Dialog.DialogCambasScene dialogCambasScene;

    StageRecordAccessor stageRecordAccessor;
    // Start is called before the first frame update
    void Start()
    {
        dialogCambasScene.OnModalClose.Subscribe(_ => OnModalClose());
        stageMetaData.GetStageRecordAccessor(stageMetaData.GetCurrentStage(), out stageRecordAccessor);
        stageRecordBuffer.ReadOrDefault(stageRecordAccessor);

        // onInitialize.Invoke();

        GameResultEvaluator.ResultEvaluation resultEvaluation = default;
        Debug.Assert(GameResultEvaluator.TryGetEvaluation(out resultEvaluation));
        recordYours = new StageRecord.Single();
        recordYours.Set(resultEvaluation.enemyCountNativeDefeated, resultEvaluation.timeElapsed, resultEvaluation.continueCount);

        _Fes_AutoSelectUpdate(); // TODO:学祭終了後に取り除く
    }

    void _Fes_AutoSelectUpdate()
    {
        var record0 = stageRecordBuffer.Get(0);
        if(
            !record0.isValid || 
            record0.defeatedCount < recordYours.defeatedCount || 
            (record0.defeatedCount == recordYours.defeatedCount && record0.time > recordYours.time)
            )
        {
            UpdateRecordByYours(0);
        }

        var record1 = stageRecordBuffer.Get(1);
        if(!record1.isValid || record1.time > recordYours.time)
        {
            UpdateRecordByYours(1);
        }

    }

    void _Fes_RecentUpdate()
    {
        var record2 = stageRecordBuffer.Get(2);
        var evaluation2 = new GameResultEvaluator.ResultEvaluation(stageMetaData.GetCurrentStage(), record2.defeatedCount, record2.time, record2.continueCount);
        var evaluationYours = new GameResultEvaluator.ResultEvaluation(stageMetaData.GetCurrentStage(), recordYours.defeatedCount, recordYours.time, recordYours.continueCount);
        evaluation2.Evaluate();
        evaluationYours.Evaluate();
        
        if(!record2.isValid || ((int)evaluationYours.totalRank >= (int)GameResultEvaluator.EvaluationRank.S && (int)evaluationYours.totalRank >= (int)evaluation2.totalRank))
        {
            UpdateRecordByYours(2);
        }
    }

    public void UpdateRecordByYours(int index)
    {
        stageRecordBuffer.Set(index, recordYours.defeatedCount, recordYours.time, recordYours.continueCount);
        onRecordUpdated.Invoke(index);
    }

    public void OnModalClose()
    {
        Debug.Log($"Write StageRecord {stageRecordAccessor.Stage}");

        _Fes_RecentUpdate();

        stageRecordBuffer.Write(stageRecordAccessor);
    }
}
