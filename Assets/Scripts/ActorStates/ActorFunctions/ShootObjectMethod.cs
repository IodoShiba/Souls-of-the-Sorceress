using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootObjectMethod : MonoBehaviour
{
    [SerializeField] Vector2 initialVelocity;
    [SerializeField] Vector2 relativePosition;
    [SerializeField] Rigidbody2D target;

    public Vector2 InitialVelocity { get => initialVelocity; set => initialVelocity = value; }
    public Vector2 RelativePosition { get => relativePosition; set => relativePosition = value; }

    // Start is called before the first frame update

    public void Use()
    {
        Instantiate(target, gameObject.transform.position + (Vector3)relativePosition, Quaternion.identity).velocity = initialVelocity;
    }
}
