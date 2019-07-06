using UnityEngine;
using static System.Math;

public class ElementAI : AI
{
    [SerializeField] float shootCycle;
    [SerializeField] float maxChaseGap;
    [SerializeField] float minChaseGap;
    [SerializeField] Player player;
    [SerializeField] ShootBullet _shootBullet;
    float t = 0;

    //HorizontalMove horizontalMove;
    //ShootBullet shootBullet;

    int moveSign = 0;
    Vector2 shoot = Vector2.zero;

    public int MoveSign { get => moveSign; }
    public Vector2 ShootDirection { get => shoot;  }

    private void Awake()
    {

        ResetOutputs();
    }

    public override void Decide()
    {
        Vector2 d = player.transform.position - transform.position;
        ResetOutputs();

        if (minChaseGap < Abs(d.x) && Abs(d.x) < maxChaseGap) 
        {
            moveSign = System.Math.Sign(d.x);
            if(t > shootCycle)
            {
                shoot = d.normalized;
                t -= shootCycle;
            }

            t += Time.deltaTime;
        }

    }

    void ResetOutputs()
    {
        moveSign = 0;
        shoot = Vector2.zero;
    }
}
