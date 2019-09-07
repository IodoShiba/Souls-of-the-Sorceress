using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Enemy : Mortal {
    public EnemyManager manager;//修正すべし
    private Rigidbody2D rb;
    //private float _protectTime;

	// Use this for initialization
	void Start () {
        if (manager == null) { manager = EnemyManager.Instance; }
        manager.AddNewEnemy(this);
        rb = GetComponent<Rigidbody2D>();
	}
	

    protected override void OnAttacked(GameObject attackObj, AttackData attack)
    {
        Debug.Log("Enemy:Ahh!");
        //_protectTime = 0.3f;
    }
    

    public override void OnDying(DealtAttackInfo causeOfDeath)
    {
        UnityEngine.EventSystems.ExecuteEvents.Execute<IDyingCallbackReceiver>(
           gameObject,
           null,
           (dyingCallbackReceiver, disposedEventData) => { dyingCallbackReceiver.OnSelfDying(causeOfDeath); }
        );
        Debug.Log("Enemy has dead.");
        manager.EnemyDying();
    }
    

}
