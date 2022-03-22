using UnityEngine;
using System.Collections.Generic;
using UniRx;

public class EnemyCounter : MonoBehaviour
{
    public int countNative {get; private set;} = 0;
    public int countInstantiated {get; private set;} = 0;
    public int countNativeDefeated {get; private set;} = 0;
    public int countInstantiatedDefeated {get; private set;} = 0;

    public void Awake()
    {
        var enemyManager = GetComponent<EnemyManager>();
        enemyManager.observableOnEnemyAdded.Subscribe(enemy => AddNewEnemy(enemy));
        enemyManager.observableOnEnemyDead.Subscribe(enemy => OneEnemyDefeated(enemy));
    }

    public void AddNewEnemy(Enemy enemy)
    {
        if(enemy.isInstantiated)
        {
            countInstantiated += 1;
        }
        else
        {
            countNative += 1;
        }
    }

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

    public int GetAllEnemyCount()
    {
        return countNative + countInstantiated;
    }
    
    public int GetAllDefeatedEnemyCount()
    {
        return countNativeDefeated + countInstantiatedDefeated;
    }
} 
