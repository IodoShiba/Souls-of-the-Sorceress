using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageInfoView : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text stageTitle;
    [SerializeField] TMPro.TMP_Text stageDescription;
    [SerializeField] UnityEngine.UI.Image stageImage;
    [SerializeField] RecordViewer[] recordViewers;
    [SerializeField] StageMetaData stageMetaData;

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

        for(int i=0; i<3; ++i)
        {
            SetStageRecord(stageId, i);
        }
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

    void SetStageRecord(StageMetaData.Stage stageId, int recordNumber)
    {
        recordViewers[recordNumber].SetRecord(stageId, 77, 77*60*24, 2); //FIXME: 仮置きの値
    }
}
