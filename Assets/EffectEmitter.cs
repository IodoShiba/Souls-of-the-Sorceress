using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectEmitter : MonoBehaviour
{
    private const float Z_SHIFT = 0.1f;

    [SerializeField] AudioClip audioClip;
    [SerializeField] AnimationClip animationClip;
    [SerializeField] Vector2 offset;
    [SerializeField] float positionerMulti;
    [SerializeField] GameObject origin;
    [SerializeField] AudioSource audioSource;

    public float PositionerMulti { get => positionerMulti; set => positionerMulti = value; }

    public void DoEffect(Mortal attackerMortal, AttackData _)
    {
        if(attackerMortal == null){ DoEffect(origin.transform.position); }
        DoEffect(attackerMortal.transform);
    }
    public void DoEffect(Mortal attackerMortal)
    {
        if(attackerMortal == null){ DoEffect(origin.transform.position); }
        DoEffect(attackerMortal.transform);
    }
    public void DoEffect(Transform endPoint)
    {
        if(endPoint == null){ DoEffect(origin.transform.position); }
        DoEffect(endPoint.position);
    }
    public void DoEffect(Vector2 endPoint)
    {
        AudioClip audC = audioClip;
        AnimationClip animC = animationClip;
        audioSource.PlayOneShot(audC);
        Vector2 diff = endPoint - (Vector2)origin.transform.position;
        EffectAnimationManager.Play(
            animC,
            origin.transform.position + (Vector3)offset + positionerMulti * (Vector3)diff - Z_SHIFT * Vector3.forward
            );
    }
}
