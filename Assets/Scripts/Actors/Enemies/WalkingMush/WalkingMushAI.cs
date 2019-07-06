using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingMushAI : AI
{
    [SerializeField] Transform playerTransform;
    [SerializeField] float maxChaseRadius;
    [SerializeField] float minChaseRadius;
    int moveSign;

    public int MoveSign { get => moveSign; }

    private void Awake()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
    }

    public override void Decide()
    {
        if (this==null) { return; }
        Vector2 vector = playerTransform.position - transform.position;
        if (minChaseRadius < vector.magnitude && vector.magnitude < maxChaseRadius)
        {
            moveSign = System.Math.Sign(vector.x);
        }
    }
}
//public class WalkingMashAI : AI
//{
//    [SerializeField] Transform playerTransform;
//    [SerializeField] float maxChaseRadius;
//    [SerializeField] float minChaseRadius;

//    HorizontalMove horizontalMove;

//    private void Awake()
//    {
//        horizontalMove = GetComponent<HorizontalMove>();
//        playerTransform = GameObject.FindWithTag("Player").transform;
//    }

//    public override void AskDecision()
//    {
//        Vector2 vector = playerTransform.position - transform.position;
//        if (minChaseRadius < vector.magnitude && vector.magnitude < maxChaseRadius)
//        {
//            horizontalMove.SetParams(System.Math.Sign(vector.x));
//            horizontalMove.SendSignal();
//        }
//    }
//}
