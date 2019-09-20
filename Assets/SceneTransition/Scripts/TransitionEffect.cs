using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionEffect : MonoBehaviour
{
    [SerializeField] Material material;
    [SerializeField] UnityEngine.UI.Image image;

    bool fade = false;
    bool isOnEffect = false;

    const float IN_EFFECT_TIME = 0.2f;
    const float INTERVAL_TIME = 0.1f;
    const float OUT_EFFECT_TIME = 0.2f;

    static WipeEffet wipeEffet;
    public static WipeEffet WipeEffet { get => wipeEffet; set => wipeEffet = value; }

    public bool IsOnEffect { get => isOnEffect; }

    public void StartEffect(bool isTransIn)
    {
        if(isTransIn == fade || isOnEffect)
        {
            return;
        }
        fade = isTransIn;
        isOnEffect = true;
        material.SetTexture("_Map", TransitionEffect.wipeEffet.Texture);
        
        StartCoroutine(TransCo(isTransIn));
    }

    IEnumerator TransCo(bool isTransIn)
    {
        isOnEffect = true;
        yield return StartCoroutine(TransEffect(isTransIn));
        isOnEffect = false;
    }

    public virtual IEnumerator TransEffect(bool isTransIn)
    {
        if (material == null) { yield break; }

        float start = 0;
        float end = 1;

        float t = 0;
        float effectTime = isTransIn ? wipeEffet.InTime : wipeEffet.OutTime;

        AnimationCurve borderS = isTransIn ? wipeEffet.BorderSIn : wipeEffet.BorderSOut;
        AnimationCurve borderE = isTransIn ? wipeEffet.BorderEIn : wipeEffet.BorderEOut;

        while(t < effectTime)
        {
            material.SetFloat("_BorderS", borderS.Evaluate(t / effectTime));
            material.SetFloat("_BorderE", borderE.Evaluate(t / effectTime));
            t += Time.deltaTime;
            yield return null;
        }
        material.SetFloat("_BorderS", borderS.Evaluate(1));
        material.SetFloat("_BorderE", borderE.Evaluate(1));

        if (isTransIn)
        {
            yield return new WaitForSeconds(wipeEffet.IntervalTime);
        }
    }

    //private void OnRenderImage(RenderTexture source, RenderTexture destination)
    //{
    //    Graphics.Blit(source, destination, material);
    //}
}
