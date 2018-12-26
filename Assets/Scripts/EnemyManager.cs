﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {

    private List<Enemy> enemies = new List<Enemy>();
    private int aliveCount;

    private List<System.Action> enemyDyingListeners = new List<System.Action>();

    IEnumerator RemoveDead()
    {
        Enemy it;
        while (true)
        {
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                if ((it = enemies[i]) == null)
                {
                    enemies.Remove(it);
                }
            }
            yield return new WaitForSeconds(10);
        }
    }

    private void Start()
    {
        StartCoroutine( RemoveDead() );
    }

    public void AddNewEnemy(Enemy enemy)
    {
        if(enemy == null) { return; }

        aliveCount++;
        enemies.Add(enemy);
    }

    public void AddEnemyDyingListener(System.Action listener)
    {
        enemyDyingListeners.Add(listener);
    }

    public void EnemyDying()
    {
        foreach(var f in enemyDyingListeners)
        {
            f();
        }
    }
}
