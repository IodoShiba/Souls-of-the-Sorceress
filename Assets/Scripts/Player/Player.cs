using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Mortal
{
    [SerializeField] InputA inputA;
    [SerializeField] StateManager behaviourState;
    [SerializeField] StateManager awakeningState;
    [SerializeField] StateManager directionState;
    //[SerializeField] float health;
    [SerializeField] float umbrellaDurability;
    [SerializeField] float awakeGauge;
    [SerializeField] float awakeGaugeDecreaseSpeed;
    [SerializeField] GameObject umbrellaForward;
    [SerializeField] GameObject umbrellaUpward;
    [SerializeField] UnityEngine.UI.Text _debugText;

    [System.Serializable]
    private class Command
    {
        [SerializeField] public string buttonName;
        //[SerializeField] public Ability ability;
        [SerializeField] public bool momential;
        [SerializeField] public UnityEngine.Events.UnityEvent func = new UnityEngine.Events.UnityEvent();
    }
    [SerializeField] List<Command> commands;

    [SerializeField]bool damaged=false;
    bool dropAttacking = false;

    public float AwakeGauge
    {
        get
        {
            return awakeGauge;
        }
    }


    // Use this for initialization
    void Start()
    {
        awakeningState.Initialize();
        behaviourState.Initialize();
        directionState.Initialize();
    }


    // Update is called once per frame
    void Update()
    {
        if (health <= 0&&!damaged)
        {
            Destroy(gameObject);
        }

        awakeningState.Execute();
        directionState.Execute();
        var v = transform.localScale;

        if (!(awakeningState.CurrentState is PlayerStates.Awakening.Ordinary))
        {
            awakeGauge -= awakeGaugeDecreaseSpeed * Time.deltaTime;
            awakeGauge = System.Math.Max(awakeGauge, 0);
            Debug.Log(awakeGauge);
        }
        behaviourState.Execute();

        _debugText.text = $"Health:{health}\nUmbrella Durability:{umbrellaDurability}\nAwake Gauge:{awakeGauge}";
    }

    public override void OnAttacked(GameObject attackObj, AttackData attack)
    {
        Vector2 selfVel = GetComponent<Rigidbody2D>().velocity;
        float rv = attackObj.GetComponent<Rigidbody2D>().velocity.x - selfVel.x;
        if (!umbrellaForward.activeInHierarchy || selfVel.x * rv >= 0)
        {
            Debug.Log("Sarah:Ouch.");
            //GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 300));
            var c = GetComponent<PlayerStates.PlayerDamaged>();
            //c.TeachCollider(attackObj.GetComponent<Collider2D>());
            if (!(behaviourState.CurrentState is PlayerStates.PlayerDamaged))
            {
                behaviourState.ManuallyChange(c);
            }
        }
    }

    public override bool IsInvulnerable()
    {
        return damaged||dropAttacking;
    }

    public override void Dying()
    {
        Debug.Log("Player has dead.");
    }

    public void ChangeDirection(int sign)
    {
        transform.localScale = new Vector3(sign, 1, 1);
    }

    public void Guard(bool toggle)
    {
        umbrellaForward.SetActive(toggle);
    }

    public void Gliding(bool toggle)
    {
        umbrellaUpward.SetActive(toggle);
    }

    public void Damaged(bool toggle)
    {
        damaged = toggle;
    }
    public void DropAttack(bool toggle)
    {
        dropAttacking = toggle;
    }
}

/*public class Player : MonoBehaviour {
    [SerializeField] InputA inputA;
    [SerializeField] StateManager behaviourState;
    [SerializeField] StateManager awakeningState;
    [SerializeField] StateManager directionState;
    [SerializeField] float health;
    [SerializeField] float umbrellaDurability;
    [SerializeField] float awakeGauge;
    [SerializeField] float awakeGaugeDecreaseSpeed;
    [SerializeField] GameObject umbrellaForward;
    [SerializeField] GameObject umbrellaUpward;
    [SerializeField] UnityEngine.UI.Text _debugText;

    [System.Serializable]
    private class Command
    {
        [SerializeField] public string buttonName;
        //[SerializeField] public Ability ability;
        [SerializeField] public bool momential;
        [SerializeField] public UnityEngine.Events.UnityEvent func = new UnityEngine.Events.UnityEvent();
    }
    [SerializeField] List<Command> commands;

    public float AwakeGauge
    {
        get
        {
            return awakeGauge;
        }
    }


    // Use this for initialization
    void Start () {
        awakeningState.Initialize();
        behaviourState.Initialize();
        directionState.Initialize();
	}

	
	// Update is called once per frame
	void Update () {
        awakeningState.Execute();
        directionState.Execute();
        var v = transform.localScale;
        
        if(!(awakeningState.CurrentState is PlayerStates.Awakening.Ordinary) )
        {
            awakeGauge -= awakeGaugeDecreaseSpeed * Time.deltaTime;
            awakeGauge = System.Math.Max(awakeGauge, 0);
            Debug.Log(awakeGauge);
        }
        behaviourState.Execute();

        _debugText.text = $"Health:{health}\nUmbrella Durability:{umbrellaDurability}\nAwake Gauge:{awakeGauge}";
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Enemy Attack")
        {
            Vector2 selfVel = GetComponent<Rigidbody2D>().velocity;
            float rv = collider.gameObject.GetComponent<Rigidbody2D>().velocity.x - selfVel.x;
            health -= 10;
            if (!umbrellaForward.activeInHierarchy || selfVel.x * rv > 0)
            {
                //GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 300));
                var c = GetComponent<PlayerStates.PlayerDamaged>();
                c.TeachCollision(collider);
                if (!(behaviourState.CurrentState is PlayerStates.PlayerDamaged))
                {
                    behaviourState.ManuallyChange(c);
                }
            }
        }
    }

    public void ChangeDirection(int sign)
    {
        transform.localScale = new Vector3(sign, 1, 1);
    }

    public void Guard(bool toggle)
    {
        umbrellaForward.SetActive(toggle);
    }

    public void Gliding(bool toggle)
    {
        umbrellaUpward.SetActive(toggle);
    }
}*/
