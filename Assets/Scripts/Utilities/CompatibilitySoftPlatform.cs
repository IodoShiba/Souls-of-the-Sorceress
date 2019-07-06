using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CompatibilitySoftPlatform : MonoBehaviour
{
    Rigidbody2D ownerRb2d;
    int layerOrdinary;
    [SerializeField, LayerField] int layerInGoingUpward;
    private bool goThrough;
    float fdy = 0;

    public bool GoThrough { get => goThrough; set => goThrough = value; }

    private void Awake()
    {
        layerOrdinary = gameObject.layer;
        ownerRb2d = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        float dy = ownerRb2d.velocity.y;
        if ((ownerRb2d.velocity.y > 0.05f || goThrough) && gameObject.layer != layerInGoingUpward)
        {
            gameObject.layer = layerInGoingUpward;
        }
        else if (gameObject.layer != layerOrdinary)
        {
            gameObject.layer = layerOrdinary;
        }
        fdy = dy;
    }
}
