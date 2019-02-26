using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootBullet : ArtsAbility
{
    [SerializeField] Rigidbody2D bulletPrefab;
    public float speed;
    public Vector2 direction;

    public override bool CanContinue(bool ordered)
    {
        return false;
    }
    public override void ActivateImple()
    {
        var newRb2d = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        newRb2d.velocity = direction.normalized * speed;
    }
}
