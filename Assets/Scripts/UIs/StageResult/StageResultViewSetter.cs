using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageResultViewSetter : MonoBehaviour
{
    [SerializeField] StageMetaData stageMetaData;
    [SerializeField] RecordViewer targetRecordViewer;

    // Start is called before the first frame update
    void Start()
    {
        GameResultEvaluator.ResultEvaluation resultEvaluation;
        if (GameResultEvaluator.TryGetEvaluation(out resultEvaluation))
        {
            targetRecordViewer.SetRecord(resultEvaluation);
        }
    }
}
