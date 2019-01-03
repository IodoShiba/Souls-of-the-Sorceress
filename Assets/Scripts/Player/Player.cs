using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Mortal
{

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
    private class UmbrellaParameters
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

    /*[System.Serializable]
    class SelfParameters {
        public float horizontalSpeed;
    }*/
    
    [SerializeField] UmbrellaParameters umbrellaParameters;
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
    int dirSign = 1;    //自機の横方向の向きを表す符号

    StateMutable<float> guardDamageMultiplier = null;
    bool guardSucceed = false;
    StateMutable<float> guardKnockBackMultiplier = null;
    StateMutable<float> horizontalSpeed = null;
    StateMutable<string> testStateMutable = null;

    PlayerStates.PlayerOnGround playerOnGround;
    PlayerStates.PlayerFlying playerFlying;

    public float AwakeGauge
    {
        get
        {
            return awakeGauge;
        }
    }

    private void Awake()
    {
        //状態遷移管理用のコンポーネントの初期化
        //これを行って初めて状態遷移が動き始める
        awakeningState.Initialize();
        behaviourState.Initialize();
        directionState.Initialize();

        //namespace PlayerStates 内のクラスはプレイヤーの状態を表すStateクラスを継承したクラス
        //_Stateのインスタンスs_.RegisterInitialize(_引数なし返り値なし関数f_) でプレイヤーの状態がsになった時に実行される関数を登録する
        GetComponent<PlayerStates.Direction.Right>().RegisterInitialize(() => { dirSign = 1; });
        GetComponent<PlayerStates.Direction.Left>().RegisterInitialize(() => { dirSign = -1; });

        enemyManager.AddEnemyDyingListener(AddAwakeGauge); //敵が死んだときに覚醒ゲージが1増えるようにする 引数の AddAwakeGauge はそのための関数

        testStateMutable = new StateMutable<string>(gameObject, "StateMutable:Other");
        testStateMutable.Assign<PlayerStates.PlayerOnGround>("StateMutable:OnGround");
        testStateMutable.Assign<PlayerStates.PlayerFlying>("StateMutable:Flying");
        testStateMutable.Assign<PlayerStates.PlayerDamaged>("StateMutable:Damaged");
        testStateMutable.Assign<PlayerStates.PlayerGuard>("StateMutable:Guard");

        //ガード成功時のダメージ倍率はプレイヤーの覚醒状態に依存する
        //クラス StateMutable<T> は「状態に応じて中身が勝手に入れ替わる変数」のイメージ
        guardDamageMultiplier = new StateMutable<float>(gameObject, 0); //デフォルト値を設定する　覚醒状態が「通常」でない（<=>覚醒状態は「覚醒」か「蒼覚醒」）ならばダメージ0倍（<=>無効化）
        guardDamageMultiplier.Assign<PlayerStates.Awakening.Ordinary>(0.5f);    //覚醒状態が「通常」の時だけダメージ0.5倍

        horizontalSpeed = new StateMutable<float>(gameObject, 0);

        //guardKnockBackMultiplier = new StateMutable<float>(gameObject, 0);

        //傘の耐久度周りの設定
        GetComponent<PlayerStates.PlayerTackle>().RegisterTurnAction(
            () => { ConsumeUmbrellaDurability(umbrellaParameters.costOfTackle); umbrellaParameters._Consuming(); },
            umbrellaParameters._Recovering
            );
        GetComponent<PlayerStates.PlayerRisingAttack>().RegisterTurnAction(
            () => { ConsumeUmbrellaDurability(umbrellaParameters.costOfRisingAttack); umbrellaParameters._Consuming(); },
            umbrellaParameters._Recovering
            );
        GetComponent<PlayerStates.PlayerGliding>().RegisterTurnAction(
            umbrellaParameters._Gliding,
            umbrellaParameters._Recovering
            );
        GetComponent<PlayerStates.PlayerGuard>().RegisterTurnAction(
            umbrellaParameters._Consuming,
            umbrellaParameters._Recovering
            );
    }
    // Use this for initialization
    void Start()
    {
        //ゲームパッドのジョイスティック入力をボタンとして解釈させる
        //これをしないとゲームパッドで落下攻撃などが上手くできなかった(PlayerSettings.Inputの設定をうまくすればこの処理要らないかもしれないが)
        //_InputAのインスタンス_.InterpretAsButton("登録ボタン名",ボタンと解釈させたい引数なし返り値bool関数f) でfをボタン扱いできる
        inputA.InterpretAsButton("Up", () => Input.GetAxis("Vertical") > 0);
        inputA.InterpretAsButton("Down", () => Input.GetAxis("Vertical") < 0);
        inputA.InterpretAsButton("Right", () => Input.GetAxis("Horizontal") > 0);
        inputA.InterpretAsButton("Left", () => Input.GetAxis("Horizontal") < 0);


    }


    // Update is called once per frame
    void Update()
    {
        //現在、プレイヤーは被ダメージ状態から抜けた時点で体力が0以下だと消滅する
        if (health <= 0&&!damaged)
        {
            Destroy(gameObject);
        }

        //状態遷移管理用のコンポーネントは内部にUpdate()を持たないため、外部から毎フレームExecute()を呼ぶ必要がある
        awakeningState.Execute();
        directionState.Execute();

        //覚醒時に覚醒ゲージを減らす処理
        if (!(awakeningState.CurrentState is PlayerStates.Awakening.Ordinary)) 
        {
            awakeGauge -= awakeGaugeDecreaseSpeed * Time.deltaTime;
            awakeGauge = System.Math.Max(awakeGauge, 0);
        }

        behaviourState.Execute();

        umbrellaParameters.Update();

        _debugText.text = 
            //$"A:Move Left, D:Move Right, Space:Jump, H:Attack, J:Magic Attack, K:Open Umbrella, L:Awake\n"+
            $"Health:{health}/{maxHealth}\n" +
            $"Umbrella Durability:{(DoesUmbrellaWork()?$"{umbrellaParameters.durability}/{umbrellaParameters.maxDurability}":$"(Recover in {umbrellaParameters.breakRestTime} seconds)")}\n" +
            $"Awake Gauge:{awakeGauge} (0.5 ≦ this value < 1 : Awake, this value = 1 : Blue Awake)\n" +
            $"testStateMutable=={testStateMutable.Content}\n"+
            $"guardDamageMultiplier=={guardDamageMultiplier.Content}";
    }

    public override void OnAttacked(GameObject attackObj, Attack attack) //攻撃されたときにAttackから（間接的に）実行される関数
    {
        Vector3 selfP = transform.position;
        Vector2 r = attackObj.transform.position - selfP;
        guardSucceed = false;
        
        if((GetComponent<PlayerStates.PlayerGuard>().IsCurrent && dirSign * r.x > 0)|| //地上ガード時のガード成否判定
            (GetComponent<PlayerStates.PlayerGliding>().IsCurrent && r.y > 0)) //滑空時のガード成否判定
        {
            guardSucceed = true;
            GetComponent<AwakeMutableCounterAttack>().Attack(attack.Owner); //ガード成功時の反撃　AwakeMutableCounterAttackはプレイヤーの覚醒状態によって反撃内容が変わる反撃用のコンポーネント
            ConsumeUmbrellaDurability(umbrellaParameters.costOfGuard);
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

    public override bool IsInvulnerable() //無敵判定用の関数 これがtrueを返す間は被ダメージ処理自体が行われない
    {
        return 
            damaged || 
            dropAttacking || 
            risingAttacking || 
            GetComponent<PlayerStates.PlayerTackle>().IsCurrent;
    }

    public override float ConvertDealtDamage(float given) //受けたダメージの変換関数
    {
        if ((GetComponent<PlayerStates.PlayerGuard>().IsCurrent || GetComponent<PlayerStates.PlayerGliding>().IsCurrent) && guardSucceed)
        {
            return given * guardDamageMultiplier.Content;
        }
        return given;
    }

    public override Vector2 ConvertDealtKnockBack(Vector2 given) //受けたノックバックの変換関数
    {
        if ((GetComponent<PlayerStates.PlayerGuard>().IsCurrent || GetComponent<PlayerStates.PlayerGliding>().IsCurrent) && guardSucceed)
        {
            return Vector2.zero;
        }
        return given;
    }

    public override void ConvertDealingAttack(ref Attack.Parameters attackData) //プレイヤーが与える攻撃の変換用関数 傘破損時に自機の攻撃力を半減させる処理などはここに書く
    {
        if (!DoesUmbrellaWork())
        {
            attackData.damage *= 0.5f;
        }
    }

    public override void Dying() //体力がなくなると呼ばれる関数　今はガバ実装
    {
        Debug.Log("Player has dead.");
    }

    public void AddAwakeGauge() //外部から覚醒ゲージを1増やす関数
    {
        if (GetComponent<PlayerStates.Awakening.Ordinary>().IsCurrent)
        {
            awakeGauge = System.Math.Min(awakeGauge + 0.1f, 1);
        }
    }

    public void ChangeDirection(int sign) //方向転換用の関数　dirSignもこの中で変更してもよいかもしれない
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
        //umbrellaUpward.enabled = toggle;
        if (toggle) { umbrellaUpward.Activate(); } else { umbrellaUpward.Inactivate(); }
    }

    //StateクラスにIsCurrentプロパティがなかった時代の名残
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
        umbrellaParameters.NudgeDurability(-amount);
    }
    public void RecoverUmbrellaDurability(int amount) //amountの文だけ傘耐久度を回復する 転送
    {
        umbrellaParameters.NudgeDurability(amount);
    }
    

    //傘が機能する(=使える=「破損」状態でない)かを返す "=>"以降の式がこの条件そのもの（同値）と考える
    //Note: _ReturnType _Function() => _statement; で1文だけの関数を定義できる これは _ReturnType _Function() {return _statement;} に等しい
    public bool DoesUmbrellaWork() => umbrellaParameters.breakRestTime <= 0;
    
}


