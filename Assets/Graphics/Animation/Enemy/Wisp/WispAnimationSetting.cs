using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WispAnimationSetting : MonoBehaviour
{
    [SerializeField] WispColor color;
    Animator animator;

    public void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void OnEnable()
    {
        PlayAnimation(WispState.Waiting);
    }

    public void PlayAnimation(WispState state)
    {
        string stateName = "wisp_";
        if(color == WispColor.Purple) { stateName += "p_"; }
        else if (color == WispColor.Green) { stateName += "g_"; }
        switch (state)
        {
            case WispState.Waiting:
                stateName += "waiting";
                break;
            case WispState.BeforeLaunch:
                stateName += "before_launch";
                break;
            case WispState.Attack:
                stateName += "attack";
                break;
            case WispState.Damaged:
                stateName += "damaged";
                break;
        }
        //Debug.Log(state+":"+stateName);
        animator.Play(stateName);
    }

    enum WispColor { Purple, Green }
    public enum WispState {Waiting,BeforeLaunch,Attack,Damaged}
}
