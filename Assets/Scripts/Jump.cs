using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Math;

public class Jump : BasicAbility
{
    [SerializeField] float jumpSpeed;
    [SerializeField] float maxPushForceMagnitude;
    [SerializeField] float maxJumpHeight;
    [SerializeField] Rigidbody2D targetRigidbody;
    float jumpBorder;
    Transform targetTransform;
    float f;
    float t = 0;
    
    // Use this for initialization
    private void Awake()
    {
        targetTransform = targetRigidbody.transform;
    }

    protected override void ActivateImple()
    {
        jumpBorder = targetTransform.position.y + maxJumpHeight;
    }

    public override bool ContinueUnderBlocked => true;

    protected override bool CanContinue(bool ordered)
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

    protected override void OnEndImple()
    {
        t = 0;
    }

    private void FixedUpdate()
    {
        if (!Activated) { return; }
        targetRigidbody.AddForce(f * Vector2.up);
    }
}
