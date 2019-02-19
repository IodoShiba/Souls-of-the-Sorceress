using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Math;

public class Jump : Ability
{
    [SerializeField] float jumpSpeed;
    [SerializeField] float maxPushForceMagnitude;
    [SerializeField] float maxJumpHeight;
    [SerializeField] Rigidbody2D targetRigidbody;
    float jumpBorder;
    Transform targetTransform;
    bool activated = false;
    float f;
    float t = 0;

    public bool Activated { get => activated;}
    
    // Use this for initialization
    private void Awake()
    {
        targetTransform = targetRigidbody.transform;
    }

    public override void Activate()
    {
        jumpBorder = targetTransform.position.y + maxJumpHeight;
        activated = true;
    }

    public override bool ContinueUnderBlocked => true;

    public override bool ContinueCheck(bool ordered)
    {
        return ordered && targetTransform.position.y < jumpBorder && !(t > 0.01 && targetRigidbody.velocity.y <= 0);
    }

    public override void OnActivated(bool ordered)
    {
        f = Min(targetRigidbody.mass * (jumpSpeed - targetRigidbody.velocity.y) / Time.deltaTime,
                        maxPushForceMagnitude);
        t += Time.deltaTime;
        Debug.Log("Jumping"+activated);
    }

    public override void OnEnd()
    {
        t = 0;
        activated = false;
    }

    private void FixedUpdate()
    {
        if (!activated) { return; }
        targetRigidbody.AddForce(f * Vector2.up);
    }
}
