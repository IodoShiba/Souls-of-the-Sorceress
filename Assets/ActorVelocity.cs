using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ActorVelocity : MonoBehaviour
{
    public abstract class VelocityShifter : MonoBehaviour
    {
        public abstract Vector2 GetVelocity();
    }
    [SerializeField] List<VelocityShifter> velocityShifters;
    [SerializeField] Rigidbody2D rigidbody2D;
    [SerializeField] float gravityScale;
    Vector2 rawVelocity;
    List<Vector2> forces = new List<Vector2>();

    public Vector2 RawVelocity { get => rawVelocity; set => rawVelocity = value; }

    private void Awake()
    {
        if (enabled)
        {
            rigidbody2D.gravityScale = 0;
        }
    }

    void FixedUpdate()
    {
        Vector2 velshift = Vector2.zero;
        foreach (var vs in velocityShifters) {
            velshift += vs.GetVelocity();
        }
        Vector2 sumForce = Vector2.zero;
        foreach(var f in forces)
        {
            sumForce += f;
        }
        sumForce += gravityScale * rigidbody2D.mass * Physics2D.gravity;
        rawVelocity += sumForce * Time.deltaTime / rigidbody2D.mass;
        rigidbody2D.velocity = rawVelocity + velshift;
    }

    public void AddForce(Vector2 force)
    {
        forces.Add(force);
    }
}
