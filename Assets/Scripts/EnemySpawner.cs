using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    [SerializeField] EnemyManager _manager;
    [SerializeField] Enemy enemyPrefab;

	// Use this for initialization
	void Start () {
        _manager.AddEnemyDyingListener(Spawn);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void Spawn()
    {
        //Instantiate(enemyPrefab, transform.position, Quaternion.identity).GetComponent<Enemy>().manager = _manager;
        _manager.Summon(enemyPrefab, transform.position, Quaternion.identity);
    }
}
