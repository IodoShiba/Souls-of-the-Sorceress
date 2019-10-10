using UnityEngine;
using System.Collections;

public class EffectAnimationManagerScriptable : ScriptableObject
{
    [SerializeField] Vector3 position;
    public void SetPosition(Transform transform) { position = transform.position; }
    public void Play(AnimationClip animationClip)
    {
        EffectAnimationManager.Play(animationClip, position);
    }

}
