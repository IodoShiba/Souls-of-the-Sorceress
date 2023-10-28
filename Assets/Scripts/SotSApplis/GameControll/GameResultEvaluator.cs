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

    [System.Serializable]
    public struct CriteriaDefeatedCount
    {
        public int thresholdSS;
        public int thresholdS;
        public int thresholdA;
        public int thresholdB;

        public EvaluationRank GetRank(int defeatedCount)
        {
            return
                defeatedCount >= thresholdSS    ?   EvaluationRank.SS :
                defeatedCount >= thresholdS     ?   EvaluationRank.S :
                defeatedCount >= thresholdA     ?   EvaluationRank.A :
                defeatedCount >= thresholdB     ?   EvaluationRank.B :
                                                    EvaluationRank.C;
        }
    }    

    [System.Serializable]
    public struct CriteriaTimeElapsed
    {
        public float thresholdSS;
        public float thresholdS;
        public float thresholdA;
        public float thresholdB;

        public EvaluationRank GetRank(float time)
        {
            return
                time <= thresholdSS    ?    EvaluationRank.SS :
                time <= thresholdS     ?    EvaluationRank.S :
                time <= thresholdA     ?    EvaluationRank.A :
                time <= thresholdB     ?    EvaluationRank.B :
                                            EvaluationRank.C;
        }
    }

    [System.Serializable]
    public struct ResultEvaluation
    {
        public bool evaluated {get; private set;}
        public StageMetaData.Stage targetStage {get; private set;}
        public int enemyCountNativeDefeated {get; private set;}
        public float timeElapsed {get; private set;}
        public int continueCount {get; private set;}

        public EvaluationRank rankDefeatedCount {get; private set;}
        public EvaluationRank rankTimeElapsed {get; private set;}
        public EvaluationRank totalRank {get; private set;}

        public EvaluationRank GetTotalRank(EvaluationRank rankDefeatedCount, EvaluationRank rankTimeElapsed, int continueCount)
        {
            int sum = (int)rankDefeatedCount + (int)rankTimeElapsed;
            EvaluationRank totalRankBase =
                sum >= 6    ?   EvaluationRank.SS :
                sum >= 5    ?   EvaluationRank.S :
                sum >= 4    ?   EvaluationRank.A :
                sum >= 2    ?   EvaluationRank.B :
                                EvaluationRank.C;

            return (EvaluationRank)System.Math.Max((int)totalRankBase - continueCount, 0);
            // return (EvaluationRank)System.Math.Max(((int)rankDefeatedCount + (int)rankTimeElapsed)/2 - continueCount, 0);
        }

        public void Evaluate()
        {
            // int enemyCountNativeinThisStage = stageMetaData.GetOneStageEnemyCount(targetStage);

            CriteriaDefeatedCount criteriaDefeatedCount;
            stageMetaData.GetCriteriaDefeatedCount(targetStage, out criteriaDefeatedCount);
            rankDefeatedCount = criteriaDefeatedCount.GetRank(enemyCountNativeDefeated);

            CriteriaTimeElapsed criteriaTimeElapsed;
            stageMetaData.GetCriteriaTimeElapsed(targetStage, out criteriaTimeElapsed);
            rankTimeElapsed = criteriaTimeElapsed.GetRank(timeElapsed);

            totalRank = GetTotalRank(rankDefeatedCount, rankTimeElapsed, continueCount);

            evaluated = true;
        }

        public ResultEvaluation(StageMetaData.Stage targetStage, int enemyCountNativeDefeated, float timeElapsed, int continueCount)
        {
            this.evaluated = false;
            this.targetStage = targetStage;
            this.enemyCountNativeDefeated = enemyCountNativeDefeated;
            this.timeElapsed = timeElapsed;
            this.continueCount = continueCount;
            this.rankDefeatedCount = EvaluationRank.C;
            this.rankTimeElapsed = EvaluationRank.C;
            this.totalRank = EvaluationRank.C;
        }
    }

    static ResultEvaluation resultEvaluation;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void RuntimeInitialize()
    {
        GameLifeCycle.observableGameClosed.Subscribe(cause =>
        {
            if (cause == GameLifeCycle.CloseCause.PlayerVictory)
            {
                MakeEvaluation();
            }
        });
        stageMetaData = Resources.Load(StageMetaData.resourcePath) as StageMetaData;
    }

    static void MakeEvaluation()
    {
        resultEvaluation = new ResultEvaluation(
            stageMetaData.GetCurrentStage(),
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
            resultEvaluation.rankDefeatedCount,
            resultEvaluation.rankTimeElapsed,
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
