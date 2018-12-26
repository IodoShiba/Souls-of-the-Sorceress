using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerStates.Awakening;

public class AwakeMutableAttack : Attack
{
    /*
    [System.Serializable]
    class Imple {
        //public Sprite sprite;
        public float damage;
        public Vector2 knockBackImpact;
        public Collider2D attackCollider;
        public bool throughable;
    }*/
    [Space(16)]
    [SerializeField] Parameters ordinaryAttack;
    [SerializeField] Parameters awakenAttack;
    [SerializeField] Parameters blueAwakenAttack;
    
    Ordinary ordinary;
    Awaken awaken;
    BlueAwaken blueAwaken;
    StateMutable<Parameters> awakeMutableAttack;
    bool ready = false;

    // Awake()内では外部のGameObject,コンポーネントの参照が安全に行えるが、
    // それらのフィールドの参照は(メソッド、プロパティ経由も含めて)安全でない
    // 自クラス内で完結する(外部のメソッドやらを参照しない)初期化処理はここで済ませたい
    new void Awake ()
    {
        GameObject player = transform.parent.gameObject;
        awakeMutableAttack = new StateMutable<Parameters>(player, null);
        ordinary = player.GetComponent<Ordinary>();
        awaken = player.GetComponent<Awaken>();
        blueAwaken = player.GetComponent<BlueAwaken>();
        //base.Awake();
    }

    // OnEnable()はAwake()の直後に呼ばれるため、
    // 起動直後の最初のフレームでは外部のGameObject,コンポーネントのフィールドは参照できない
    protected override void OnEnable() 
    {
        if (ready)
        {

            if (ordinaryAttack.attackCollider!=null&& ordinary.IsCurrent) ordinaryAttack.attackCollider.enabled = true;
            if (awakenAttack.attackCollider != null&& awaken.IsCurrent) awakenAttack.attackCollider.enabled = true;
            if (blueAwakenAttack.attackCollider != null && blueAwaken.IsCurrent) blueAwakenAttack.attackCollider.enabled = true;
            SpriteRenderer sr;
            if ((sr = GetComponent<SpriteRenderer>()) != null)
            {
                sr.enabled = false;
            }

            paramsRaw = new Parameters(awakeMutableAttack.Content);
            if (owner)
            {
                paramsConvertedByOwner = new Parameters(paramsRaw);
                owner.ConvertDealingAttack(paramsConvertedByOwner);
            }

            //Parameters attack = awakeMutableAttack.Content;
            //base.sprite = attack.sprite;
            base.damage = paramsConvertedByOwner.damage;
            base.knockBackImpact = paramsConvertedByOwner.knockBackImpact;
            base.attackCollider = paramsConvertedByOwner.attackCollider;
        }
    }

    // ここでようやく他のコンポーネントの関数が呼べる
    // Player.Start() と AwakeMutableAttack.Start() のどちらが先に実行されても
    // Update() が実行される時点で awakeMutableAttack.Content != null のはずである
    // (StateManager.Initialize()内で、awakeMutableAttackがAwakening.Stateに設定したコールバックが実行されるからである)
    private void Start()
    {
        awakeMutableAttack.Assign<Ordinary>(ordinaryAttack);
        awakeMutableAttack.Assign<Awaken>(awakenAttack);
        awakeMutableAttack.Assign<BlueAwaken>(blueAwakenAttack);
    }

    //awakeMutableAttack.Content != null のはず... Assert()とはそのような意味である
    private void Update()
    {
        if (!ready)
        {
            ready = true;
            paramsRaw = new Parameters(awakeMutableAttack.Content);
            paramsConvertedByOwner = new Parameters(paramsRaw);
            OnEnable();//うんｔ
        }
        Debug.Assert(awakeMutableAttack.Content != null);
    }

    protected override void OnDisable()
    {
        if (ordinaryAttack.attackCollider != null) ordinaryAttack.attackCollider.enabled = false;
        if (awakenAttack.attackCollider != null) awakenAttack.attackCollider.enabled = false;
        if (blueAwakenAttack.attackCollider != null) blueAwakenAttack.attackCollider.enabled = false;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.enabled = false;
        }
    }
}
