using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using static System.Math;

[RequireComponent(typeof(Rigidbody2D))]
public class HorizontalMove : Ability , ActorVelocity.VelocityShifter
{
    [SerializeField] float moveSpeed;
    [SerializeField, FormerlySerializedAs("MaxPushForceMagnitude")] float maxPushForceMagnitude;
    [SerializeField] float maxStopForceMagnitude;
    [SerializeField] Rigidbody2D targetRigidbody;

    int sign = 0;

    private void FixedUpdate()
    {
        float maxForceMagnitude = sign == 0 ? maxStopForceMagnitude : maxPushForceMagnitude;
        /*
        float accelaration = Max(-MaxAccelarationMagnitude, Min(sign * moveSpeed - targetRigidbody.velocity.x, MaxAccelarationMagnitude));
        Vector2 pushForce = (targetRigidbody.mass * accelaration * Vector2.right) / Time.deltaTime;*/
        float f = Max(-maxForceMagnitude, Min(
                        targetRigidbody.mass * (sign * moveSpeed - targetRigidbody.velocity.x) / Time.deltaTime,
                        maxForceMagnitude));
        targetRigidbody.AddForce(f*Vector2.right);
    }
    
    public override void Activate()
    {
        return;
    }

    public override bool ContinueCheck(bool ordered)
    {
        return ordered;
    }

    public override void OnActivated(bool ordered)
    {
        sign = Sign(Input.GetAxisRaw("Horizontal"));
    }
    

    public override void OnEnd()
    {
        sign = 0;
    }

    public Vector2 GetVelocity()
    {
        return sign * moveSpeed * Vector2.right;
    }
}

/*
[RequireComponent(typeof(Rigidbody2D))]
public class HorizontalMove : MonoBehaviour {
    public class VelocityShift
    {
        HorizontalMove target;
        public float velocity = 0;
        public float maxSpeed = 0;
        public float accelaration = 0;
        public float dragAccelaration;
        private float t=0;

        public VelocityShift(HorizontalMove target, float maxSpeed, float dragAccelaration)
        {
            this.target = target;
            this.maxSpeed = maxSpeed;
            this.dragAccelaration = dragAccelaration;
        }

        public void Update()
        {
            if (accelaration != 0) {
                if (float.IsPositiveInfinity(accelaration))
                {
                    velocity = maxSpeed;
                }
                else if (float.IsNegativeInfinity(accelaration))
                {
                    velocity = -maxSpeed;
                }
                else
                {
                    velocity = Max(-maxSpeed, Min(velocity + accelaration * Time.deltaTime, maxSpeed));
                }
            }
            else
            {
                if (float.IsInfinity(dragAccelaration))
                {
                    velocity = 0;
                }
                else
                {
                    float decreaseVelocityAmount = Sign(velocity) * dragAccelaration * Time.deltaTime;
                    if ((velocity - decreaseVelocityAmount) * velocity > 0)
                    {
                        velocity -= decreaseVelocityAmount;
                    }
                    else
                    {
                        velocity = 0;
                    }
                }
            }
            target.velocityShifts.Add(velocity);
            t += Time.deltaTime;
        }
    }

    [SerializeField] Rigidbody2D targetRigidbody2D;
    private float speed;
    private List<float> velocityShifts = new List<float>();

    public float Speed { get; }

    public void Update()
    {
        speed = 0;
        foreach(var s in velocityShifts)
        {
            speed += s;
        }
        targetRigidbody2D.velocity = Vector2.right * speed + Vector2.up * targetRigidbody2D.velocity.y;
        velocityShifts.Clear();
    }
    
    public VelocityShift CreateVelocityShift(float maxVelocity, float dragAccelaration = float.PositiveInfinity) { return new VelocityShift(this, maxVelocity,dragAccelaration); }
}
*/
