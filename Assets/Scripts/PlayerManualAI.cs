using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Math;

public abstract class AI : MonoBehaviour
{
    
    public abstract void AskDecision();
    
}

//public class PlayerManualAI : AI
//{
//    [SerializeField] InputA inputA;

//    HorizontalMove horizontalMove;
//    PassPlatform passPlatform;
//    Jump jump;
//    Guard guard;
//    VerticalSlash verticalSlash;
//    ReturnSlash returnSlash;
//    SmashSlash smashSlash;
//    AerialSlash aerialSlash;
//    Glide glide;
//    RisingAttack risingAttack;
//    DropAttack dropAttack;
//    Tackle tackle;

//    private void Awake()
//    {
//        horizontalMove = GetComponent<HorizontalMove>();
//        passPlatform = GetComponent<PassPlatform>();
//        jump = GetComponent<Jump>();
//        guard = GetComponent<Guard>();
//        verticalSlash = GetComponent<VerticalSlash>();
//        returnSlash = GetComponent<ReturnSlash>();
//        smashSlash = GetComponent<SmashSlash>();
//        aerialSlash = GetComponent<AerialSlash>();
//        glide = GetComponent<Glide>();
//        risingAttack = GetComponent<RisingAttack>();
//        dropAttack = GetComponent<DropAttack>();
//        tackle = GetComponent<Tackle>();
//    }
//    private void Start()
//    {
//        //ゲームパッドのジョイスティック入力をボタンとして解釈させる
//        //これをしないとゲームパッドで落下攻撃などが上手くできなかった(PlayerSettings.Inputの設定をうまくすればこの処理要らないかもしれないが)
//        //_InputAのインスタンス_.InterpretAsButton("登録ボタン名",ボタンと解釈させたい引数なし返り値bool関数f) でfをボタン扱いできる
//        inputA.InterpretAsButton("Up", () => Input.GetAxisRaw("Vertical") > 0);
//        inputA.InterpretAsButton("Down", () => Input.GetAxisRaw("Vertical") < 0);
//        inputA.InterpretAsButton("Right", () => Input.GetAxisRaw("Horizontal") > 0);
//        inputA.InterpretAsButton("Left", () => Input.GetAxisRaw("Horizontal") < 0);
//    }

//    public override void AskDecision()
//    {
//        int _sign = 0;
//        if((_sign = Sign(Input.GetAxisRaw("Horizontal"))) != 0)
//        {
//            horizontalMove.SetParams(_sign);
//            horizontalMove.SendSignal();
//        }
//        if (Input.GetAxisRaw("Vertical") < -0.3)
//        {
//            passPlatform.SendSignal();
//        }
//        if (Input.GetButton("Jump"))
//        {
//            jump.SendSignal();
//        }

//        if(Input.GetButton("Open Umbrella"))
//        {
//            glide.SendSignal();
//            guard.SendSignal();
//        }

//        if (Input.GetButtonDown("Attack"))
//        {
//            if (Input.GetButton("Open Umbrella"))
//            {
//                tackle.SendSignal();
//                risingAttack.SendSignal();
//            }
//            else
//            {
//                verticalSlash.SendSignal();
//                returnSlash.SendSignal();
//                smashSlash.SendSignal();
//                aerialSlash.SendSignal();
//            }
//        }

//        if (inputA.GetMultiButtonDown("Attack", "Down")) { dropAttack.SendSignal(); }
//    }
//}
