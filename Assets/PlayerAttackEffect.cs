using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackEffect : MonoBehaviour
{
    private const float Z_SHIFT = 0.1f;

    [SerializeField] AudioClip audioClipOrdinary;
    [SerializeField] AudioClip audioClipAwaken;
    [SerializeField] AnimationClip animationClipOrdinary;
    [SerializeField] AnimationClip animationClipAwaken;
    [SerializeField] Vector2 offset;
    [SerializeField] float positionerMulti;
    [SerializeField] Mortal owner;
    [SerializeField] AudioSource audioSource;
    [SerializeField] ActionAwake actionAwake;

    public void DoEffect(Mortal subjectMortal)
    {
        AudioClip audC = null;
        AnimationClip animC = null;
        switch (actionAwake.AwakeLevel)
        {
            case ActionAwake.AwakeLevels.ordinary:
                audC = audioClipOrdinary;
                animC = animationClipOrdinary;
                break;
            case ActionAwake.AwakeLevels.awaken:
            case ActionAwake.AwakeLevels.blueAwaken:
                audC = audioClipAwaken;
                animC = animationClipAwaken;
                break;
        }
        audioSource.PlayOneShot(audC);
        Vector2 diff = subjectMortal.transform.position - owner.transform.position;
        EffectAnimationManager.Play(
            animC,
            owner.transform.position + (Vector3)offset + positionerMulti*(Vector3)diff - Z_SHIFT * Vector3.forward
            );
    }
}
