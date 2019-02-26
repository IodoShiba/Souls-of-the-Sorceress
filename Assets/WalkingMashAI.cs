using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingMashAI : AI
{
    [SerializeField] Transform playerTransform;
    [SerializeField] float maxChaseRadius;
    [SerializeField] float minChaseRadius;

    protected override void Brain()
    {
        Vector2 vector = playerTransform.position - transform.position;
        if (minChaseRadius < vector.magnitude && vector.magnitude < maxChaseRadius)
        {
            _sign = System.Math.Sign(vector.x);
            Decide<HorizontalMove>();
        }
    }
}
