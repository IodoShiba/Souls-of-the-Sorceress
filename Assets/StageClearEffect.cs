using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StageClearEffect : MonoBehaviour
{
    [SerializeField] float musicFadeOutSpan;
    [SerializeField] float wordFadeInSpan;
    [SerializeField] float initialScale;

    [SerializeField] UnityEngine.UI.Image image;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip clip;
    

    public void StartEffect()
    {
        StartCoroutine(EffectCo());
    }

    IEnumerator EffectCo()
    {
        float initVolume = audioSource.volume;
        audioSource.DOFade(0, musicFadeOutSpan);

        yield return new WaitForSeconds(musicFadeOutSpan);

        audioSource.clip = null;
        audioSource.volume = initVolume;
        audioSource.PlayOneShot(clip);

        image.transform.localScale = new Vector3(initialScale, initialScale, 1);
        image.DOFade(1, wordFadeInSpan).SetEase(Ease.OutExpo);
        image.transform.DOScale(Vector3.one, wordFadeInSpan).SetEase(Ease.OutExpo);
    }
}
