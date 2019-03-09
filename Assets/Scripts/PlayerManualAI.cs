using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Math;

public abstract class AI : MonoBehaviour
{
    
    public abstract void AskDecision();
    
}

public class PlayerManualAI : AI
{
    HorizontalMove horizontalMove;
    Jump jump;
    VerticalSlash verticalSlash;
    ReturnSlash returnSlash;
    SmashSlash smashSlash;
    AerialSlash aerialSlash;

    private void Awake()
    {
        horizontalMove = GetComponent<HorizontalMove>();
        jump = GetComponent<Jump>();
        verticalSlash = GetComponent<VerticalSlash>();
        returnSlash = GetComponent<ReturnSlash>();
        smashSlash = GetComponent<SmashSlash>();
        aerialSlash = GetComponent<AerialSlash>();
    }

    public override void AskDecision()
    {
        int _sign = 0;
        if((_sign = Sign(Input.GetAxisRaw("Horizontal"))) != 0)
        {
            //Decide<HorizontalMove>();
            horizontalMove.SetParams(_sign).SendSignal();
        }
        if (Input.GetButton("Jump"))
        {
            //Decide<Jump>();
            jump.SendSignal();
        }
        if (Input.GetButtonDown("Attack"))
        {
            //Decide<VerticalSlash>();
            //Decide<ReturnSlash>();
            //Decide<SmashSlash>();
            //Decide<AerialSlash>();
            verticalSlash.SendSignal();
            returnSlash.SendSignal();
            smashSlash.SendSignal();
            aerialSlash.SendSignal();
        }
    }
}
