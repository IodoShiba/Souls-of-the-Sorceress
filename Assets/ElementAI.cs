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
    protected override void Brain()
    {
        float d = player.transform.position.x - transform.position.x;
        if (minChaseGap < Abs(d) && Abs(d) < maxChaseGap) 
        {
            _sign = System.Math.Sign(player.transform.position.x - transform.position.x);
            Decide<HorizontalMove>();
        }

        t += Time.deltaTime;
        if (t > shootCycle)
        {
            _shootBullet.direction = player.transform.position - transform.position;
            Decide<ShootBullet>();
            t -= shootCycle;
        }
    }
}
