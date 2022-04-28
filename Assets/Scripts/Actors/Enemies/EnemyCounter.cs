using UnityEngine;
using System.Collections.Generic;
using UniRx;

public class EnemyCounter : MonoBehaviour
{
    // public static int countNative {get; private set;} = 0;
    // public static int countInstantiated {get; private set;} = 0;
    public static int countNativeDefeated {get; private set;} = 0;
    public static int countInstantiatedDefeated {get; private set;} = 0;
    public static int countNativeDefeatedLastSection {get; private set;} = 0;
    public static int countInstantiatedDefeatedLastSection {get; private set;} = 0;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void RuntimeInitialize()
    {
        GameLifeCycle.observableOnGameOpen.Subscribe(_=>ResetCount());
        GameOverScene.observableGameOver.Subscribe(_=>RewindLastSection());
        ResetCount();
    }

    public void Start()
    {
        var enemyManager = GetComponent<EnemyManager>();
        // enemyManager.observableOnEnemyAdded.Subscribe(enemy => AddNewEnemy(enemy));
        enemyManager.observableOnEnemyDead.Subscribe(enemy => OneEnemyDefeated(enemy)).AddTo(gameObject);

        Debug.Log($"count native defeated last section: {countNativeDefeatedLastSection}");

        countNativeDefeatedLastSection = 0;
        countInstantiatedDefeatedLastSection = 0;
    }

    // public void AddNewEnemy(Enemy enemy)
    // {
    //     if(enemy.isInstantiated)
    //     {
    //         countInstantiated += 1;
    //     }
    //     else
    //     {
    //         countNative += 1;
    //     }
    // }

    public void OneEnemyDefeated(Enemy enemy)
    {
        if(enemy.isInstantiated)
        {
            countInstantiatedDefeated += 1;
            countInstantiatedDefeatedLastSection += 1;
        }
        else
        {
            countNativeDefeated += 1;
            countNativeDefeatedLastSection += 1;

            // Debug.Log(enemy.name);
        }
    }

    // public int GetAllEnemyCount()
    // {
    //     return countNative + countInstantiated;
    // }
    
    public static int GetAllDefeatedEnemyCount()
    {
        return countNativeDefeated + countInstantiatedDefeated;
    }

    static void ResetCount()
    {
        countNativeDefeated = 0;
        countInstantiatedDefeated = 0;
        countNativeDefeatedLastSection = 0;
        countInstantiatedDefeatedLastSection = 0;
    }

    public static void RewindLastSection()
    {
        countNativeDefeated -= countNativeDefeatedLastSection;
        countInstantiatedDefeated -= countInstantiatedDefeatedLastSection = 0;
;
        countNativeDefeatedLastSection = 0;
        countInstantiatedDefeatedLastSection = 0;
    }

    // for debug purpose
    private void OnGUI()
    {
        GUI.Label(new Rect(0, 48, Screen.width, Screen.height), $"Count Native Defeated: {countNativeDefeated}\nCount Instantiated Defeated: {countInstantiatedDefeated}");
    }
    
} 
