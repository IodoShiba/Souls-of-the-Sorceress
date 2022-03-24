using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class GameResultEvaluator : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void RuntimeInitialize()
    {
        GameLifeCycle.observableGameClosed.Subscribe(_=>Evaluate());
    }

    static void Evaluate()
    {
        int enemyCountNativeDefeated = EnemyCounter.countNativeDefeated;
        float timeElapsed = TimeRecorder.timeElapsed;
        
        Debug.Log($"Native Enemy defeated: {enemyCountNativeDefeated}\nTime Elapsed: {System.TimeSpan.FromSeconds((double)timeElapsed).ToString("c")}");
    }
}
