using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SotSGame/RankMarks")]
public class RankMarks : ScriptableObject
{
    [SerializeField] Sprite markC;
    [SerializeField] Sprite markB;
    [SerializeField] Sprite markA;
    [SerializeField] Sprite markS;
    [SerializeField] Sprite markSS;

    public Sprite GetMark(GameResultEvaluator.EvaluationRank rank) 
    {
        return 
            rank == GameResultEvaluator.EvaluationRank.C ? markC:
            rank == GameResultEvaluator.EvaluationRank.B ? markB:
            rank == GameResultEvaluator.EvaluationRank.A ? markA:
            rank == GameResultEvaluator.EvaluationRank.S ? markS:
            rank == GameResultEvaluator.EvaluationRank.SS ? markSS:
            null;

    }
}
