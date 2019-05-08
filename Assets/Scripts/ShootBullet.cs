﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootBullet : ArtsAbility,ActorBehaviour.IParamableWith<Vector2>
{
    [SerializeField] Rigidbody2D bulletPrefab;
    public float speed;
    private Vector2 direction;

    protected override bool ShouldContinue(bool ordered)
    {
        return false;
    }
    protected override void OnInitialize()
    {
        var newRb2d = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        newRb2d.velocity = direction.normalized * speed;
    }
    public void SetParams(Vector2 value)
    {
        direction = value;
    }
}