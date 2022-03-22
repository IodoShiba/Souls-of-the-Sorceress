using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

[DisallowMultipleComponent]
public class EnemyManager : MonoBehaviour {

    public const float ANIMATION_LENGTH = -1f;

    private List<Enemy> enemies = new List<Enemy>();
    private int aliveCount;
    static private EnemyManager instance;

    private List<System.Action> enemyDyingListeners = new List<System.Action>();
    private Subject<Enemy> subjectOnEnemyAdded = new Subject<Enemy>();
    private Subject<Enemy> subjectOnEnemyDead = new Subject<Enemy>();

    public static EnemyManager Instance { get => instance; }
    public IObservable<Enemy> observableOnEnemyAdded { get => subjectOnEnemyAdded; }
    public IObservable<Enemy> observableOnEnemyDead { get => subjectOnEnemyDead; }

    public EnemyManager() { if (instance == null) instance = this; }
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

    private void Awake()
    {
        if (instance != null && this != instance) 
        {
            Debug.LogError($"{this.GetType().Name} cannot exist double or more in one scene. GameObject '{name}' has been Deleted because it has second {this.GetType().Name}.");
            //Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        StartCoroutine( RemoveDead() );
    }

    public Enemy Summon(Enemy target,Vector3 position,Quaternion quaternion, AnimationClip summonEffect = null, float summonDelayTime = ANIMATION_LENGTH)
    {
        if (summonEffect == null)
        {
            var r = Enemy.InstantiateThis(target, position, quaternion);
            r.manager = this;
            AddNewEnemy(r);
            return r;
        }
        else
        {
            float realSummonDelayTime = summonDelayTime >= 0 ? summonDelayTime : summonEffect.length;
            var r = Enemy.InstantiateThis(target, position, quaternion);
            r.manager = this;
            r.gameObject.SetActive(false);
            EffectAnimationManager.Play(summonEffect, position);
            AddNewEnemy(r);
            UniRx.Observable
                .Timer(System.TimeSpan.FromSeconds(realSummonDelayTime)).Subscribe(_ => r.gameObject.SetActive(true));
            return r;
        }
    }

    public void AddNewEnemy(Enemy enemy)
    {
        if(enemy == null) { return; }

        aliveCount++;
        enemies.Add(enemy);

        subjectOnEnemyAdded.OnNext(enemy);
    }

    public void AddEnemyDyingListener(System.Action listener)
    {
        enemyDyingListeners.Add(listener);
    }

    public void EnemyDying(Enemy enemy)
    {
        subjectOnEnemyDead.OnNext(enemy);
        foreach(var f in enemyDyingListeners)
        {
            f();
        }
    }

    public void EliminateAllEnemies()
    {
        for(int i = enemies.Count - 1; i >= 0; --i)
        {
            enemies[i].Suicide();
        }
    }
    
}
