using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Mortal {
    [SerializeField] Vector2 knockBackImpact;
    private Rigidbody2D rb;
    private float _protectTime;
    private float _randShift;

	// Use this for initialization
	void Start () {
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
                Destroy(gameObject);
            }
            transform.position += new Vector3((float)(5 * System.Math.Sin(Time.fixedTime+_randShift)), rb.velocity.y, 0) * Time.deltaTime;
        }
	}

    public override void OnAttacked(GameObject attackObj, AttackData attack)
    {
        Debug.Log("Enemy:Ahh!");
        _protectTime = 0.3f;
    }

    public override Vector2 ConvertKnockDown(Vector2 given)
    {
        return 10 * given;
    }

    public override bool IsInvulnerable()
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
