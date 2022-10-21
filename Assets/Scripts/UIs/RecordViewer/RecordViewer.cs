using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordViewer : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text recordDefeated;
    [SerializeField] UnityEngine.UI.Image rankMarkDefeated;

    [SerializeField] TMPro.TMP_Text recordTime;
    [SerializeField] UnityEngine.UI.Image rankMarkTime;
    
    [SerializeField] UnityEngine.UI.Image rankMarkSummary;
    [SerializeField] TMPro.TMP_Text recordContinue;

    [SerializeField] StageMetaData stageMetaData;
    [SerializeField] RankMarks rankMarks;

    readonly string time_format = @"hh\:mm\:ss\.fff";

    public void SetRecord(StageMetaData.Stage stageId, int defeatedCount, float time, int continueCount)
    {
        var ev = new GameResultEvaluator.ResultEvaluation(stageId, defeatedCount, time, continueCount);
        ev.Evaluate();

        SetRecord(ev);
    }

    public void SetRecord(in GameResultEvaluator.ResultEvaluation ev)
    {
        if(!ev.evaluated)
        {
            return;
        }

        recordDefeated.SetText("{0}", (float)ev.enemyCountNativeDefeated);
        recordTime.text = System.TimeSpan.FromSeconds((double)ev.timeElapsed).ToString(time_format);
        if(recordContinue != null)
        {
            recordContinue.SetText("{0}", (float)ev.continueCount);
        }
        
        SetRankMark(rankMarkDefeated, ev.rankDefeatedCount);
        SetRankMark(rankMarkTime, ev.rankTimeElapsed);
        SetRankMark(rankMarkSummary, ev.totalRank);
    }
    
    void SetRankMark(UnityEngine.UI.Image image, GameResultEvaluator.EvaluationRank rank)
    {
        var mark = rankMarks.GetMark(rank);
        if(mark == null)
        {
            image.sprite = null;
            image.color = Color.clear;
        }
        else
        {
            image.sprite = mark;
            image.color = Color.white;
        }
    }
}
