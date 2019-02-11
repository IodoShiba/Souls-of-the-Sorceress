using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Math;


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