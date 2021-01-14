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
    [SerializeField] AudioClip clip;
    

    public void StartEffect()
    {
        StartCoroutine(EffectCo());
    }

    IEnumerator EffectCo()
    {
        SoundManager.Instance.FadeOutBgm(musicFadeOutSpan);

        yield return new WaitForSeconds(musicFadeOutSpan);

        SoundManager.Instance.PlaySE(clip);

        image.transform.localScale = new Vector3(initialScale, initialScale, 1);
        image.DOFade(1, wordFadeInSpan).SetEase(Ease.OutExpo);
        image.transform.DOScale(Vector3.one, wordFadeInSpan).SetEase(Ease.OutExpo);
    }
}
