using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//多分後々 abstractな基底クラス になる
[DisallowMultipleComponent]
public class Enemy : Mortal {
    public EnemyManager manager;//修正すべし
    private Rigidbody2D rb;
    private float _protectTime;

	// Use this for initialization
	void Start () {
        if (manager == null) manager = EnemyManager.Instance;
        manager.AddNewEnemy(this);
        rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        if (_protectTime > 0)
        {
            _protectTime -= Time.deltaTime;
        }
        else if (_protectTime < 0)
        {
            rb.velocity = new Vector2(0, 0);
            _protectTime = 0;
        }
        else
        {
            if (health <= 0)
            {
                manager.EnemyDying();
                Destroy(gameObject);
            }
            
        }
	}

    protected override void OnAttacked(GameObject attackObj, AttackData attack)
    {
        Debug.Log("Enemy:Ahh!");
        _protectTime = 0.3f;
    }
    

    protected override bool _IsInvulnerable()
    {
        return false;//_protectTime > 0;
    }

    public override void Dying()
    {
        Debug.Log("Enemy has dead.");
        
    }
    
    /*private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_protectTime <= 0 && collision.transform.tag == "Attack")
        {
            int sign = System.Math.Sign(transform.position.x - collision.transform.position.x);
            rb.AddForce(new Vector2(sign * knockBackImpact.x, knockBackImpact.y));
            _protectTime = 0.3f;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_protectTime <= 0 && collision.transform.tag == "Attack")
        {
            Debug.Log("Enemy:Ahh!");
            int sign = System.Math.Sign(transform.position.x - collision.transform.position.x);
            rb.AddForce(new Vector2(sign * knockBackImpact.x, knockBackImpact.y));
            _protectTime = 0.3f;
        }
    }*/
}
