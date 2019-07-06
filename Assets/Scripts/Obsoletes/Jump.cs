using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Math;

//原因不明の動作不良の疑いあり
//動作不良の内容…たまに十分な高さまでジャンプしなかったり、高くジャンプしすぎたりする
//再現ができず、原因の検証ができていない
public class Jump : BasicAbility
{
    [SerializeField] float jumpSpeed;
    [SerializeField] float maxPushForceMagnitude;
    [SerializeField] float maxJumpHeight;
    [SerializeField] Rigidbody2D targetRigidbody;
    //private CompatibilitySoftPlatform softPlatform;
    float jumpBorder;
    Transform targetTransform;
    float f;
    float t = 0;
    bool isAvailable = true;
    public override bool IsAvailable()
    {
        return isAvailable;
    }
    // Use this for initialization
    private void Awake()
    {
        targetTransform = targetRigidbody.transform;
        //softPlatform = GetComponent<CompatibilitySoftPlatform>();
    }

    protected override void OnInitialize()
    {
        jumpBorder = targetTransform.position.y + maxJumpHeight;
        //if (softPlatform != null) softPlatform.GoThrough = true;
    }

    public override bool ContinueUnderBlocked => true;


    protected override bool ShouldContinue(bool ordered)
    {
        return ordered && targetTransform.position.y < jumpBorder && !(t > 0.01 && targetRigidbody.velocity.y <= 0);
    }

    protected override void OnActive(bool ordered)
    {
        f = Min(targetRigidbody.mass * (jumpSpeed - targetRigidbody.velocity.y) / Time.deltaTime,
                        maxPushForceMagnitude);
        t += Time.deltaTime;
        Debug.Log("Jumping"+Activated);
    }

    protected override void OnTerminate()
    {
        isAvailable = false;
        t = 0;
        //if (softPlatform != null) softPlatform.GoThrough = false;
    }

    private void FixedUpdate()
    {
        if (Activated)
        {
            targetRigidbody.AddForce(f * Vector2.up);
        }
        else if (!isAvailable && targetRigidbody.velocity.y <= 0) 
        {
            isAvailable = true;
        }
    }
}
