using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class GameResultEvaluator : MonoBehaviour
{
    public enum EvaluationRank : int {C, B, A, S, SS}

    static bool resultIsReady = false;
    static int enemyCountNativeDefeated;
    static float timeElapsed;

    static StageMetaData stageMetaData;

    public struct ResultEvaluation
    {
        public int enemyCountNativeDefeated {get; private set;}
        public float timeElapsed {get; private set;}
        public int continueCount {get; private set;}

        public EvaluationRank criteriaDefeatedCount {get; private set;}
        public EvaluationRank criteriaTimeElapsed {get; private set;}
        public EvaluationRank totalRank {get; private set;}

        public void Evaluate()
        {
            string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            Debug.Log($"scene: {currentSceneName}");
            int enemyCountNativeinThisStage = stageMetaData.GetOneStageEnemyCount(stageMetaData.SceneToStage(currentSceneName));
            Debug.Log($"cnt: {enemyCountNativeinThisStage}");
            int gapCount = enemyCountNativeDefeated - enemyCountNativeinThisStage;

            criteriaDefeatedCount =
                gapCount == 0       ?   EvaluationRank.SS :
                gapCount >= -10     ?   EvaluationRank.S :
                gapCount >= -20     ?   EvaluationRank.A :
                gapCount >= -30     ?   EvaluationRank.B :
                                        EvaluationRank.C;

            float gapTime = timeElapsed - 600;
            criteriaTimeElapsed =
                gapTime <= 0    ?   EvaluationRank.SS :
                gapTime <= 30   ?   EvaluationRank.S :
                gapTime <= 60   ?   EvaluationRank.A :
                gapTime <= 90   ?   EvaluationRank.B :
                                    EvaluationRank.C;

            totalRank = (EvaluationRank)System.Math.Max(((int)criteriaDefeatedCount + (int)criteriaTimeElapsed)/2 - continueCount, 0);
        }

        public ResultEvaluation(int enemyCountNativeDefeated, float timeElapsed, int continueCount)
        {
            this.enemyCountNativeDefeated = enemyCountNativeDefeated;
            this.timeElapsed = timeElapsed;
            this.continueCount = continueCount;
            this.criteriaDefeatedCount = EvaluationRank.C;
            this.criteriaTimeElapsed = EvaluationRank.C;
            this.totalRank = EvaluationRank.C;
        }
    }

    static ResultEvaluation resultEvaluation;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void RuntimeInitialize()
    {
        GameLifeCycle.observableGameClosed.Subscribe(_=>MakeEvaluation());
        stageMetaData = Resources.Load(StageMetaData.resourcePath) as StageMetaData;
    }

    static void MakeEvaluation()
    {
        resultEvaluation = new ResultEvaluation(
            EnemyCounter.countNativeDefeated, 
            TimeRecorder.timeElapsed, 
            SotS.ReviveController.ContinueCount
            );
        resultEvaluation.Evaluate();
        resultIsReady = true;
        
        Debug.Log(
            string.Format("Native Enemy defeated: {0}\nTime Elapsed: {1}\nContinue: {2}",
            resultEvaluation.enemyCountNativeDefeated,
            System.TimeSpan.FromSeconds((double)resultEvaluation.timeElapsed).ToString("c"),
            resultEvaluation.continueCount
            ));
        Debug.Log(
            string.Format("Rank\nDefeat: {0}\nTime: {1}\nContinue: {2}\n\nTotal: {3}",
            resultEvaluation.criteriaDefeatedCount,
            resultEvaluation.criteriaTimeElapsed,
            resultEvaluation.continueCount > 0 ? (new string('X', resultEvaluation.continueCount)) : "None",
            resultEvaluation.totalRank
            ));
    }

    public static bool TryGetEvaluation(out ResultEvaluation result)
    {
        if(!resultIsReady){result = default; return false;}

        result = resultEvaluation;
        return true;
    }

    public static void ClearEvaluation()
    {
        resultIsReady = false;
    }
}
