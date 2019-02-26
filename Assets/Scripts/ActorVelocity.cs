using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ActorVelocity : MonoBehaviour
{
    public interface VelocityShifter
    {
         Vector2 GetVelocity();
    }
    List<VelocityShifter> velocityShifters = new List<VelocityShifter>();
    [SerializeField] Rigidbody2D rigidbody2D;
    [SerializeField] float gravityScale;
    Vector2 rawVelocity;
    List<Vector2> forces = new List<Vector2>();
    Vector2 velshift = Vector2.zero;

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
        rawVelocity = rigidbody2D.velocity - velshift;

        velshift = Vector2.zero;
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
        forces.Clear();
    }

    public void AddForce(Vector2 force)
    {
        forces.Add(force);
    }

    public void AddVelocityShifter(VelocityShifter item)
    {
        velocityShifters.Add(item);
    }

    public void RemoveVelocityShifter(VelocityShifter item)
    {
        velocityShifters.Remove(item);
    }
}
