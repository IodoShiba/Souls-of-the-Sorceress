using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardEffect : MonoBehaviour
{
    private const float Z_SHIFT = 0.1f;

    [SerializeField] AudioClip audioClip;
    [SerializeField] AnimationClip animationClip;
    [SerializeField] Rect barrierRange;
    [SerializeField] AudioSource audioSource;

    public void DoEffect(Mortal subjectMortal, AttackData attackData) => DoEffect(subjectMortal);
    public void DoEffect(Mortal subjectMortal)
    {
        Debug.Log("Do guard effect");
        AudioClip audC = audioClip;
        AnimationClip animC = animationClip;
        audioSource.PlayOneShot(audC);
        Vector2 center = (Vector2)transform.position + barrierRange.position;
        Vector2 diff = (Vector2)subjectMortal.transform.position - center;
        Vector2 centerToEffectPos = diff*Mathf.Min(.5f*barrierRange.width/Mathf.Abs(diff.x), .5f*barrierRange.height/Mathf.Abs(diff.y));
        EffectAnimationManager.Play(
            animC,
            (Vector3)(center + centerToEffectPos) + new Vector3(0, 0, transform.position.z - Z_SHIFT)
            );
    }
}
