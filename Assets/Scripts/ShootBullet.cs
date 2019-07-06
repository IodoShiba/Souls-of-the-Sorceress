using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootBullet : MonoBehaviour
{
    [SerializeField] AttackInHitbox bulletPrefab;

    public void Use(Vector2 velocity)
    {
        var newRb2d = AttackInHitbox.InstantiateThis(bulletPrefab, transform.position, Quaternion.identity, GetComponent<Mortal>()).GetComponent<Rigidbody2D>();
        newRb2d.velocity = velocity;
    }
}
