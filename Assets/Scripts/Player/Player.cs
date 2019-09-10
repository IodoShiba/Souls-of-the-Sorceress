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

    [SerializeField] UmbrellaParameters umbrellaParameters;
    //[SerializeField] StateManager awakeningState;
    //[SerializeField] StateManager directionState;
    [SerializeField] ActionAwake actionAwake;
    [SerializeField] AttackInHitbox umbrellaUpward;
    [SerializeField] Collider2D guardColliderExtension;
    [SerializeField] ActorSarah.ActorStateConnectorSarah connectorSarah;
    [SerializeField] ActorFunction.Directionable directionable;
    [SerializeField] ActorFunction.GuardMethod guard;
    [SerializeField] UnityEngine.UI.Text _debugText;
    [SerializeField, DisabledField] bool damaged = false;

    bool dropAttacking = false;
    bool risingAttacking = false;
    int dirSign = 1;    //自機の横方向の向きを表す符号

    //StateMutable<float> guardDamageMultiplier = null;
    bool guardSucceed = false;
    //AwakeMutable<float> guardKnockBackMultiplier = null;
    //AwakeMutable<float> horizontalSpeed = null;
    //AwakeMutable<string> testStateMutable = null;

    PlayerStates.PlayerOnGround playerOnGround;
    PlayerStates.PlayerFlying playerFlying;
    
    public int DirSign { get => dirSign; }

    protected override void Awake()
    {
        base.Awake();

        
    }
    // Update is called once per frame
    void Update()
    {
        //Debug.Log(GetComponent<ActorSarah.ActorStateConnectorSarah>().Current.GetType().Name); 
        //現在、プレイヤーは被ダメージ状態から抜けた時点で体力が0以下だと消滅する
        if (health <= 0&&!damaged)
        {
            Destroy(gameObject);
        }
        

        if (_debugText != null)
        {
            _debugText.text =
                //$"A:Move Left, D:Move Right, Space:Jump, H:Attack, J:Magic Attack, K:Open Umbrella, L:Awake\n"+
                $"Health:{health}/{maxHealth}\n" +
                umbrellaParameters._DebugOutput() +
                actionAwake._DebugOutput();// +
                //$"guardDamageMultiplier=={guardDamageMultiplier.Content}";
        }
    }

    protected override void OnAttacked(GameObject attackObj,AttackData attack) //攻撃されたときにAttackから（間接的に）実行される関数
    {
        guardSucceed = false;
    }

    protected override void OnTriedAttack(Mortal attacker, AttackData dealt, in Vector2 relativePosition)
    {
        if (guard.Activated)
        {
            guard.TryGuard(dealt, relativePosition);
        }
    }

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


