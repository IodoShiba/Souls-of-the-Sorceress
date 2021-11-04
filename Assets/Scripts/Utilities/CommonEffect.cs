using UnityEngine;
using UniRx.Async;

namespace CommonEffects
{
    public static class BoomCollapsingEffect
    {
        public static async UniTask Effect( 
            Rect range,
            int boomingCount, 
            float effectLength,
            AudioClip boomClip,
            AudioSource audioSource,
            AnimationClip boomEffect
            )
        {
            
            audioSource.PlayOneShot(boomClip);

            for(int j = 0; j < boomingCount; ++j)
            {
                EffectAnimationManager
                    .Play(
                    boomEffect,
                    new Vector3(
                        Random.Range(range.xMin, range.xMax), 
                        Random.Range(range.yMin, range.yMax), 
                        0));
                await UniTask.Delay(System.TimeSpan.FromSeconds(effectLength/boomingCount));
            }
        }
    }
}