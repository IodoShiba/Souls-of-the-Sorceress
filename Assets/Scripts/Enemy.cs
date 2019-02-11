using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//多分後々 abstractな基底クラス になる
public class Enemy : Mortal {
    public EnemyManager manager;//修正すべし
    [SerializeField] Vector2 knockBackImpact;
    private Rigidbody2D rb;
    private float _protectTime;
    private float _randShift;

	// Use this for initialization
	void Start () {
        manager.AddNewEnemy(this);
        rb = GetComponent<Rigidbody2D>();
        _randShift = Random.Range(0,(float)(2.0*System.Math.PI));
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
            transform.position += new Vector3((float)(5 * System.Math.Sin(Time.fixedTime+_randShift)), rb.velocity.y, 0) * Time.deltaTime;
        }
	}

    protected override void OnAttacked(GameObject attackObj, Attack attack)
    {
        Debug.Log("Enemy:Ahh!");
        _protectTime = 0.3f;
    }

    protected override Vector2 ConvertDealtKnockBack(Vector2 given)
    {
        return 10 * given;
    }

    protected override bool IsInvulnerable()
    {
        return _protectTime > 0;
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
