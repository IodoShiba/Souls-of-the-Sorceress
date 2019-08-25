using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    //[SerializeField] EnemyManager _manager;
    //[SerializeField] Enemy enemyPrefab;
    [SerializeField] ActorFunction.Summon summon;
    bool use;

	// Use this for initialization
	void Start () {
        use = false;
	}
	
	// Update is called once per frame
	void Update () {
        summon.ManualUpdate(use);
        use = false;
	}

    public void Spawn()
    {
        use = true;
    }
}
