using UnityEngine;
using static System.Math;

public class ElementAI : AI
{
    [SerializeField] float shootCycle;
    [SerializeField] Vector2 maxChaseGap;
    [SerializeField] Vector2 minChaseGap;
    [SerializeField] Player player;
    [SerializeField] ShootBullet _shootBullet;
    float t = 0;

    //HorizontalMove horizontalMove;
    //ShootBullet shootBullet;

    int moveSign = 0;
    Vector2Int moveSigns;
    Vector2 shoot = Vector2.zero;

    public Vector2Int MoveSigns { get => moveSigns; }
    public Vector2 ShootDirection { get => shoot;  }

    private void Awake()
    {
        ResetOutputs();
    }

    public override void Decide()
    {
        Vector2 d = player.transform.position - transform.position;
        ResetOutputs();

        if (minChaseGap.x < Abs(d.x) && Abs(d.x) < maxChaseGap.x && minChaseGap.y < Abs(d.y) && Abs(d.y) < maxChaseGap.y) 
        {
            moveSign = System.Math.Sign(d.x);
            moveSigns = new Vector2Int(System.Math.Sign(d.x), System.Math.Sign(d.y));

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
        moveSigns = Vector2Int.zero;
        shoot = Vector2.zero;
    }
}
