using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IodoShibaUtil.FlagUtility;

[DisallowMultipleComponent]
public class Enemy : Mortal {

    [Flags]
    public enum AttrFlags : int
    {
        Instantiated = 1 << 0, // Instantiateされた
    }

    public EnemyManager manager;//修正すべし
    private Rigidbody2D rb;
    private int attrFlags = 0;
    //private float _protectTime;

    bool GetAttrFlag(AttrFlags flag)
    {
        return attrFlags.GetFlagAny((int)flag);
    }

    private void SetAttrFlag(AttrFlags flag, bool value)
    {
        attrFlags.SetFlag((int)flag, value);
    }

    public bool isInstantiated 
    {
        get => GetAttrFlag(AttrFlags.Instantiated);
        private set => SetAttrFlag(AttrFlags.Instantiated, value);
    }

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
        manager.EnemyDying(this);
    }
    
    public static Enemy InstantiateThis(Enemy target, Vector3 position, Quaternion quaternion)
    {
        var ret = Instantiate<Enemy>(target, position, quaternion);

        ret.SetAttrFlag(AttrFlags.Instantiated, true);

        return ret;
    }
}
