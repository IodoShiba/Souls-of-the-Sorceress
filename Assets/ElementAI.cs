using System.Collections;
using System.Collections.Generic;
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

    HorizontalMove horizontalMove;
    ShootBullet shootBullet;

    private void Awake()
    {
        horizontalMove = GetComponent<HorizontalMove>();
        shootBullet = GetComponent<ShootBullet>();
    }

    public override void AskDecision()
    {
        float d = player.transform.position.x - transform.position.x;
        if (minChaseGap < Abs(d) && Abs(d) < maxChaseGap) 
        {
            horizontalMove.SetParams(System.Math.Sign(player.transform.position.x - transform.position.x));
            horizontalMove.SendSignal();
        }

        t += Time.deltaTime;
        if (t > shootCycle)
        {
            _shootBullet.SetParams(player.transform.position - transform.position);
            _shootBullet.SendSignal();
            t -= shootCycle;
        }
    }
}
