using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformContactorVelocityFrexiblizer : MonoBehaviour
{
    [SerializeField] float speedMinThreshold;
    [SerializeField] float ordinaryHeight;
    [SerializeField] float hardDroppingHeight;
    [SerializeField] Rigidbody2D targetRigidbody;
    bool hardDropping = false;
    BoxCollider2D platformContactorCollider;
    private void Awake()
    {
        platformContactorCollider = GetComponent<BoxCollider2D>();
        hardDropping = false;
    }
    private void FixedUpdate()
    {
        bool ishd = targetRigidbody.velocity.y < speedMinThreshold;
        if (ishd != hardDropping)
        {
            float resHeight = ishd ? hardDroppingHeight : ordinaryHeight;
            platformContactorCollider.size = new Vector2(platformContactorCollider.size.x, resHeight);
            platformContactorCollider.offset = new Vector2(platformContactorCollider.offset.x, resHeight / 2);
        }
        hardDropping = ishd;
    }
}
