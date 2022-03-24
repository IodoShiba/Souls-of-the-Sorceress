using UnityEngine;
using System.Collections.Generic;
using UniRx;

public class EnemyCounter : MonoBehaviour
{
    // public static int countNative {get; private set;} = 0;
    // public static int countInstantiated {get; private set;} = 0;
    public static int countNativeDefeated {get; private set;} = 0;
    public static int countInstantiatedDefeated {get; private set;} = 0;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void RuntimeInitialize()
    {
        GameLifeCycle.observableOnGameOpen.Subscribe(_=>ResetCount());
    }

    public void Start()
    {
        var enemyManager = GetComponent<EnemyManager>();
        // enemyManager.observableOnEnemyAdded.Subscribe(enemy => AddNewEnemy(enemy));
        enemyManager.observableOnEnemyDead.Subscribe(enemy => OneEnemyDefeated(enemy)).AddTo(gameObject);
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
        }
        else
        {
            countNativeDefeated += 1;
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
    }

    // for debug purpose
    [SerializeField] int _countNativeDefeated;
    [SerializeField] int _countInstantiatedDefeated;
    void Update()
    {
        _countInstantiatedDefeated = countInstantiatedDefeated;
        _countNativeDefeated = countNativeDefeated;
    }
    
} 
