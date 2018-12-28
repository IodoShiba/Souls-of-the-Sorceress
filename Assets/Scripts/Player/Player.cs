using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Mortal
{
    [System.Serializable]
    private class AttackObject
    {
        [SerializeField] GameObject current;
        [SerializeField] AwakeMutableObject prefabs;
        private bool valid = false;

        public GameObject Current
        {
            get
            {
                if (!valid)
                {
                    prefabs.SynchronizeWith(o => current = o);
                    valid = true;
                }
                return current;
            }
        }
    }

    /*
    メモ 

    傘耐久度<int> max = 8
    耐久度は傘開きボタンを押してない時0.5秒毎に1点回復

    耐久度減少量...
        1回ガード成功：1
        突進：2
        上昇攻撃；2
        滑空：1 毎秒

    傘破損状態
        継続時間：8秒
        使えない技:
            ガード,
            突進,
            滑空,
            上昇攻撃
        終了後：傘耐久値最大にする

    */
    [System.Serializable]
    private class UmbrellaParamaters
    {
        public int durability;
        public int maxDurability;
        public float recoverCycle;
        public float breakTime;
        public int costOfGuard;
        public int costOfTackle;
        public int costOfRisingAttack;
        public int costOfGlidingPerSecond;
        [System.NonSerialized] public float breakRestTime = 0;
        [System.NonSerialized] public float t = 0;
        [System.NonSerialized] public float cycle = 100000;
        [System.NonSerialized] public int amount = 0;

        public void Update()
        {
            if (breakRestTime <= 0) 
            {
                breakRestTime = 0;
                if (t > cycle)
                {
                    NudgeDurability(amount);
                    t -= cycle;
                }
                t += Time.deltaTime;
            }
            else //破損状態
            {
                breakRestTime -= Time.deltaTime;
                if(breakRestTime <= 0)
                {
                    durability = maxDurability;
                }
            }
        }

        public void ChangeDurabilityDifferential(float cycle,int amount)
        {
            this.cycle = cycle;
            this.amount = amount;
            t = 0;
        }
        public void _Recovering() { ChangeDurabilityDifferential(recoverCycle,1); }
        public void _Gliding() { ChangeDurabilityDifferential(1,-costOfGlidingPerSecond); }
        public void _Consuming() { ChangeDurabilityDifferential(100000,0); }

        public void NudgeDurability(int amount) //amountの文だけ傘耐久度を変更する
        {
            durability += amount;
            if (durability > maxDurability)
            {
                durability = maxDurability;
            }
            if (durability < 0)
            {
                breakRestTime = breakTime;
            }
        }
    }

    [SerializeField] UmbrellaParamaters umbrellaParamaters;
    [SerializeField] float awakeGauge;
    [SerializeField] float awakeGaugeDecreaseSpeed;
    [SerializeField] InputA inputA;
    [SerializeField] EnemyManager enemyManager;
    [SerializeField] StateManager behaviourState;
    [SerializeField] StateManager awakeningState;
    [SerializeField] StateManager directionState;
    //[SerializeField] float health;
    [SerializeField] AwakeMutableAttack guard;
    [SerializeField] AwakeMutableAttack umbrellaUpward;
    [SerializeField] Collider2D guardColliderExtension;
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
    bool risingAttacking = false;
    int dirSign = 1;

    StateMutable<float> guardDamageMultiplier = null;
    bool guardSucceed = false;
    StateMutable<float> guardKnockBackMultiplier = null;
    StateMutable<string> testStateMutable = null;
    

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
        inputA.InterpretAsButton("Up", () => Input.GetAxis("Vertical") > 0);
        inputA.InterpretAsButton("Down", () => Input.GetAxis("Vertical") < 0);
        inputA.InterpretAsButton("Right", () => Input.GetAxis("Horizontal") > 0);
        inputA.InterpretAsButton("Left", () => Input.GetAxis("Horizontal") < 0);

        awakeningState.Initialize();
        behaviourState.Initialize();
        directionState.Initialize();

        GetComponent<PlayerStates.Direction.Right>().RegisterInitialize(() => { dirSign = 1; });
        GetComponent<PlayerStates.Direction.Left>().RegisterInitialize(() => { dirSign = -1; });

        enemyManager.AddEnemyDyingListener(AddAwakeGauge);

        testStateMutable = new StateMutable<string>(gameObject, "StateMutable:Other");
        testStateMutable.Assign<PlayerStates.PlayerOnGround>("StateMutable:OnGround");
        testStateMutable.Assign<PlayerStates.PlayerFlying>("StateMutable:Flying");
        testStateMutable.Assign<PlayerStates.PlayerDamaged>("StateMutable:Damaged");
        testStateMutable.Assign<PlayerStates.PlayerGuard>("StateMutable:Guard");

        guardDamageMultiplier = new StateMutable<float>(gameObject, 0);
        guardDamageMultiplier.Assign<PlayerStates.Awakening.Ordinary>(0.5f);

        guardKnockBackMultiplier = new StateMutable<float>(gameObject, 0);

        GetComponent<PlayerStates.PlayerTackle>().RegisterTurnAction(
            () => {ConsumeUmbrellaDurability(umbrellaParamaters.costOfTackle); umbrellaParamaters._Consuming();},
            umbrellaParamaters._Recovering
            );
        GetComponent<PlayerStates.PlayerRisingAttack>().RegisterTurnAction(
            () => { ConsumeUmbrellaDurability(umbrellaParamaters.costOfRisingAttack); umbrellaParamaters._Consuming(); },
            umbrellaParamaters._Recovering
            );
        GetComponent<PlayerStates.PlayerGliding>().RegisterTurnAction(
            umbrellaParamaters._Gliding,
            umbrellaParamaters._Recovering
            );
        GetComponent<PlayerStates.PlayerGuard>().RegisterTurnAction(
            umbrellaParamaters._Consuming,
            umbrellaParamaters._Recovering
            );

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
        umbrellaParamaters.Update();

        _debugText.text = 
            //$"A:Move Left, D:Move Right, Space:Jump, H:Attack, J:Magic Attack, K:Open Umbrella, L:Awake\n"+
            $"Health:{health}/{maxHealth}\n" +
            $"Umbrella Durability:{(DoesUmbrellaWork()?$"{umbrellaParamaters.durability}/{umbrellaParamaters.maxDurability}":$"(Recover in {umbrellaParamaters.breakRestTime} seconds)")}\n" +
            $"Awake Gauge:{awakeGauge} (0.5 ≦ this value < 1 : Awake, this value = 1 : Blue Awake)\n" +
            $"testStateMutable=={testStateMutable.Content}\n"+
            $"guardDamageMultiplier=={guardDamageMultiplier.Content}";
    }

    public override void OnAttacked(GameObject attackObj, Attack attack)
    {
        Vector3 selfP = transform.position;
        Vector2 r = attackObj.transform.position - selfP;
        guardSucceed = false;
        
        if((GetComponent<PlayerStates.PlayerGuard>().IsCurrent && dirSign * r.x > 0)||
            (GetComponent<PlayerStates.PlayerGliding>().IsCurrent && r.y > 0))
        {
            guardSucceed = true;
            GetComponent<AwakeMutableCounterAttack>().Attack(attack.Owner);
            ConsumeUmbrellaDurability(umbrellaParamaters.costOfGuard);
        }
        else
        {
            Debug.Log("Sarah:Ouch.");
            var c = GetComponent<PlayerStates.PlayerDamaged>();
            if (!(behaviourState.CurrentState is PlayerStates.PlayerDamaged))
            {
                behaviourState.ManuallyChange(c);
            }
        }
    }

    public override bool IsInvulnerable()
    {
        return 
            damaged || 
            dropAttacking || 
            risingAttacking || 
            GetComponent<PlayerStates.PlayerTackle>().IsCurrent;
    }

    public override float ConvertDealtDamage(float given)
    {
        if ((GetComponent<PlayerStates.PlayerGuard>().IsCurrent || GetComponent<PlayerStates.PlayerGliding>().IsCurrent) && guardSucceed)
        {
            return given * guardDamageMultiplier.Content;
        }
        return given;
    }

    public override Vector2 ConvertDealtKnockBack(Vector2 given)
    {
        if ((GetComponent<PlayerStates.PlayerGuard>().IsCurrent || GetComponent<PlayerStates.PlayerGliding>().IsCurrent) && guardSucceed)
        {
            return Vector2.zero;
        }
        return given;
    }

    public override void ConvertDealingAttack(Attack.Parameters attackData)
    {
        if (!DoesUmbrellaWork())
        {
            attackData.damage *= 0.5f;
        }
    }

    public override void Dying()
    {
        Debug.Log("Player has dead.");
    }

    public void AddAwakeGauge()
    {
        if (GetComponent<PlayerStates.Awakening.Ordinary>().IsCurrent)
        {
            awakeGauge = System.Math.Min(awakeGauge + 0.1f, 1);
        }
    }

    public void ChangeDirection(int sign)
    {
        transform.localScale = new Vector3(sign, 1, 1);
    }

    public void Guard(bool toggle)
    {
        //guard.enabled=toggle;
        guardColliderExtension.enabled = toggle;
    }

    public void Gliding(bool toggle)
    {
        umbrellaUpward.enabled = toggle;
        
    }

    public void Damaged(bool toggle)
    {
        damaged = toggle;
    }
    public void DropAttack(bool toggle)
    {
        dropAttacking = toggle;
    }
    public void RisingAttack(bool toggle)
    {
        risingAttacking = toggle;
    }

    public void ConsumeUmbrellaDurability(int amount) //amountの文だけ傘耐久度を消費する 転送
    {
        umbrellaParamaters.NudgeDurability(-amount);
    }
    public void RecoverUmbrellaDurability(int amount) //amountの文だけ傘耐久度を回復する 転送
    {
        umbrellaParamaters.NudgeDurability(amount);
    }
    

    //傘が機能する(=使える=「破損」状態でない)かを返す "=>"以降の式がこの条件そのもの（同値）と考える
    //Note: _ReturnType _Function() => _statement; で1文だけの関数を定義できる これは _ReturnType _Function() {return _statement;} に等しい
    public bool DoesUmbrellaWork() => umbrellaParamaters.breakRestTime <= 0;
    
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
