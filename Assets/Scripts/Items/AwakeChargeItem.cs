using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IodoShibaUtil.Vector2DUtility;

public class AwakeChargeItem : ItemBase
{
    [SerializeField] float amount;
    [SerializeField] float approachingForce;
    [SerializeField] float drag;
    [SerializeField] float startApproachTime;
    [SerializeField] SpriteRenderer renderer;
    [SerializeField] Rigidbody2D rigidbody;

    Vector2 iv;
    float lifeTime = 0;

    public float Amount { get => amount; }

    private void FixedUpdate()
    {
        if (iv != Vector2.zero)
        {
            rigidbody.velocity = iv;
            iv = Vector2.zero;
        }
        if(lifeTime > startApproachTime && ActorManager.PlayerActor != null && approachingForce != 0)
        {
            rigidbody.AddForce(approachingForce * ((Vector2)(ActorManager.PlayerActor.transform.position - transform.position)).normalized - drag * rigidbody.velocity);
        }
        lifeTime += Time.fixedDeltaTime;
    }

    public void Activate(in Vector2 position,in Vector2 velocity, float chargeAmount)
    {
        transform.position = Vector2DUtilityClass.ModifiedXY(transform.position, position);
        float size = GetSize(chargeAmount);
        transform.localScale = new Vector3(size, size, 1);
        amount = chargeAmount;
        iv = velocity;
        lifeTime = 0;
        gameObject.SetActive(true);
    }

    float GetSize(float chargeAmount) => Mathf.Min(0.3f + 4 * (chargeAmount), 0.7f);

    public override void OnReceived()
    {
        Inactivate();
    }

    public void Inactivate()
    {
        gameObject.SetActive(false);
        lifeTime = 0;
    }
}
