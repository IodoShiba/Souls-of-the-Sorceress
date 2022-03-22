using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEditor;
public static class GameRecord
{
    static bool recording = false;

    static int enemyCountAllNative;
    static int enemyCountAllInstantiate;
    static int enemyCountAllNativeDefeated;
    static int enemyCountAllInstantiateDefeated;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        GameLifeCycle.observableOnGameOpen.Subscribe(_=>{Reset(); StartRecord();});
        GameLifeCycle.observableOnGameClose.Subscribe(_=>StopRecord());
    }

    static void StartRecord()
    {
        recording = true;
    }

    static void StopRecord()
    {
        recording = false;
    }

    public static Dictionary<string, double> Dump()
    {
        Dictionary<string, double> dic = new Dictionary<string, double>();

        dic["enemyCountAllNative"]                  = enemyCountAllNative;
        dic["enemyCountAllInstantiate"]             = enemyCountAllInstantiate;
        dic["enemyCountAllNativeDefeated"]          = enemyCountAllNativeDefeated;
        dic["enemyCountAllInstantiateDefeated"]     = enemyCountAllInstantiateDefeated;

        return dic;
    }

    public static string DumpJson()
    {
        Dictionary<string, double> dic = Dump();

        return JsonUtility.ToJson(dic);
    }

    static void Reset()
    {
        enemyCountAllNative = 0;
        enemyCountAllInstantiate = 0;
        enemyCountAllNativeDefeated = 0;
        enemyCountAllInstantiateDefeated = 0;
    }
}