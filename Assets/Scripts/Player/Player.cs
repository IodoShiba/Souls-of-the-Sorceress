using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//このクラスの役割は
//・体力を管理し、無くなれば死ぬ
//・攻撃を与える
//・攻撃を受ける
//・状態遷移を構築する
[DisallowMultipleComponent]
public class Player : Mortal
{
    enum BehaviourStates : int
    {
        endOfAction=0,
        onGround,
        flying
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
    class StateOption {
        protected Player player;
        public StateOption(Player player) { this.player = player; }
    }
    [SerializeField] UmbrellaParameters umbrellaParameters;
    [SerializeField] InputA inputA;
    [SerializeField] EnemyManager enemyManager;
    [SerializeField] StateManager awakeningState;
    [SerializeField] StateManager directionState;
    [SerializeField] ActionAwake actionAwake;
    [SerializeField] AttackInHitbox umbrellaUpward;
    [SerializeField] Collider2D guardColliderExtension;
    [SerializeField] GroundSensor groundSensor;
    [SerializeField] Jump jumpAbility;
    [SerializeField] UnityEngine.UI.Text _debugText;
    

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
    
    public int DirSign { get => dirSign; }

    private void Awake()
    {
        //状態遷移管理用のコンポーネントの初期化
        //これを行って初めて状態遷移が動き始める
        awakeningState.Initialize();
        //behaviourState.Initialize();
        directionState.Initialize();

        //namespace PlayerStates 内のクラスはプレイヤーの状態を表すStateクラスを継承したクラス
        //_Stateのインスタンスs_.RegisterInitialize(_引数なし返り値なし関数f_) でプレイヤーの状態がsになった時に実行される関数を登録する
        GetComponent<PlayerStates.Direction.Right>().RegisterInitialize(() => { dirSign = 1; });
        GetComponent<PlayerStates.Direction.Left>().RegisterInitialize(() => { dirSign = -1; });
        
        //ガード成功時のダメージ倍率はプレイヤーの覚醒状態に依存する
        //クラス StateMutable<T> は「状態に応じて中身が勝手に入れ替わる変数」のイメージ
        guardDamageMultiplier = new StateMutable<float>(gameObject, 0); //デフォルト値を設定する　覚醒状態が「通常」でない（<=>覚醒状態は「覚醒」か「蒼覚醒」）ならばダメージ0倍（<=>無効化）
        guardDamageMultiplier.Assign<PlayerStates.Awakening.Ordinary>(0.5f);    //覚醒状態が「通常」の時だけダメージ0.5倍

        horizontalSpeed = new StateMutable<float>(gameObject, 0);

        //guardKnockBackMultiplier = new StateMutable<float>(gameObject, 0);
        
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
        
        if (Input.GetButtonDown("Awake"))
        {
            actionAwake.Action();
        }
        
        _debugText.text =
            //$"A:Move Left, D:Move Right, Space:Jump, H:Attack, J:Magic Attack, K:Open Umbrella, L:Awake\n"+
            $"Health:{health}/{maxHealth}\n" +
            umbrellaParameters._DebugOutput() +
            actionAwake._DebugOutput() +
            $"guardDamageMultiplier=={guardDamageMultiplier.Content}";
    }

    protected override void OnAttacked(GameObject attackObj,AttackData attack) //攻撃されたときにAttackから（間接的に）実行される関数
    {
        Vector3 selfP = transform.position;
        Vector2 r = attackObj.transform.position - selfP;
        guardSucceed = false;
    }
    //protected override void ConvertDealtAttack(AttackData dealt)
    //{
    //    base.ConvertDealtAttack(dealt);
    //}

    protected override bool IsInvulnerable() //無敵判定用の関数 これがtrueを返す間は被ダメージ処理自体が行われない
    {
        return
            damaged ||
            dropAttacking ||
            risingAttacking;
    }

    //public override void ConvertDealingAttack(AttackData attackData) //プレイヤーが与える攻撃の変換用関数 傘破損時に自機の攻撃力を半減させる処理などはここに書く
    //{
    //    if (!DoesUmbrellaWork())
    //    {
    //        attackData.damage *= 0.5f;
    //    }
    //}

    public override void Dying() //体力がなくなると呼ばれる関数　今はガバ実装
    {
        Debug.Log("Player has dead.");
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

    //傘が機能する(=使える=「破損」状態でない)かを返す "=>"以降の式がこの条件そのもの（同値）と考える
    //Note: _ReturnType _Function() => _statement; で1文だけの関数を定義できる これは _ReturnType _Function() {return _statement;} に等しい
    public bool DoesUmbrellaWork() => umbrellaParameters.DoesUmbrellaWork();
    
}


