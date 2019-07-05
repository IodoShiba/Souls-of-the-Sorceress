using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using static System.Math;

[RequireComponent(typeof(Rigidbody2D))]
public class HorizontalMove : BasicAbility , ActorVelocity.VelocityShifter , ActorBehaviour.IParamableWith<int>
{
    [SerializeField] public float moveSpeed;
    [SerializeField] float maxPushForceMagnitude;
    [SerializeField] float maxStopForceMagnitude;
    [SerializeField] Rigidbody2D targetRigidbody;
    Mortal selfMortal;

    float defaultMoveSpeed;
    int sign = 0;

    public int Sign
    { get
        {
            if (!Activated) { sign = 0; }
            return sign;
        }
    }

    private void Start()
    {
        defaultMoveSpeed = moveSpeed;
        selfMortal = GetComponent<Mortal>();
    }

    private void FixedUpdate()
    {
        if (!Activated) sign = 0;
        float maxForceMagnitude = (sign == 0 ? maxStopForceMagnitude : maxPushForceMagnitude) * (false ? 0 : 1);
        /*
        float accelaration = Max(-MaxAccelarationMagnitude, Min(sign * moveSpeed - targetRigidbody.velocity.x, MaxAccelarationMagnitude));
        Vector2 pushForce = (targetRigidbody.mass * accelaration * Vector2.right) / Time.deltaTime;*/
        float f = Max(-maxForceMagnitude, Min(
                        targetRigidbody.mass * (sign * moveSpeed - targetRigidbody.velocity.x) / Time.deltaTime,
                        maxForceMagnitude));
        targetRigidbody.AddForce(f*Vector2.right);
    }
    
    protected override void OnInitialize()
    {
        return;
    }

    protected override bool ShouldContinue(bool ordered)
    {
        return ordered;
    }

    protected override void OnActive(bool ordered)
    {
    }
    

    protected override void OnTerminate()
    {
        sign = 0;
    }

    public Vector2 GetVelocity()
    {
        return sign * moveSpeed * Vector2.right;
    }

    public void SetParams(int value)
    {
        sign = value;
    }

    public void ResetSpeed() { moveSpeed = defaultMoveSpeed; }
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
