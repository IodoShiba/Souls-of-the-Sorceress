using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Math;

public abstract class AI : MonoBehaviour
{
    //_（アンダースコア）から始まるこれら2メンバーはAbilityにActorの向きの情報を与える上手い方法が現時点で無いがために作られた
    //AIにActorの向きの情報を持たせるのは論外な設計だから
    //これは修正して消す
    protected int _sign = 0;
    public int _Sign { get => _sign; }

    protected HashSet<System.Type> decisions;
    public void AskDecision(HashSet<System.Type> decisions) {
        this.decisions = decisions;
        Brain();
    }
    protected void Decide<AbilityType>() where AbilityType : Ability
    {
        this.decisions.Add(typeof(AbilityType));
    }
    protected abstract void Brain();
}

public class PlayerManualAI : AI
{

    protected override void Brain()
    {
        _sign = 0;
        if((_sign = Sign(Input.GetAxisRaw("Horizontal"))) != 0)
        {
            Decide<HorizontalMove>();
        }
        if (Input.GetButton("Jump"))
        {
            Decide<Jump>();
        }
        if (Input.GetButtonDown("Attack"))
        {
            Decide<VerticalSlash>();
            Decide<ReturnSlash>();
            Decide<SmashSlash>();
            Decide<AerialSlash>();
        }
    }
}
