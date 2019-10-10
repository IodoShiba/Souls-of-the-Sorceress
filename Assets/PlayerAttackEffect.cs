using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackEffect : MonoBehaviour
{
    [SerializeField] AudioClip audioClipOrdinary;
    [SerializeField] AudioClip audioClipAwaken;
    [SerializeField] AnimationClip animationClipOrdinary;
    [SerializeField] AnimationClip animationClipAwaken;
    [SerializeField] AudioSource audioSource;
    [SerializeField] ActionAwake actionAwake;

    public void DoEffect()
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
        EffectAnimationManager.Play(animC, transform.position - 0.1f * Vector3.forward);
    }
}
