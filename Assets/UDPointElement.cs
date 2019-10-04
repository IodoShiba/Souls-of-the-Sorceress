using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UDPointElement : MonoBehaviour
{
    [SerializeField] bool usable;
    [SerializeField] UnityEngine.UI.Image usableImage;
    [SerializeField] UnityEngine.UI.Image bgImage;
    Sequence blinkSeq;
    float blinkCycle;
    
    public bool Usable {
        get => usable;
        set {
            usableImage.enabled = usable = value;
        }
    }

    bool umbrellaWork = true;
    bool UmbrellaWorks
    {
        set
        {
            if (!umbrellaWork && value)
            {
                blinkSeq.Restart();
                blinkSeq.Pause();
                bgImage.color = Color.black;
            }
            if (umbrellaWork && !value)
            {
                blinkSeq.Play();
            }
            umbrellaWork = value;
        }
    }


    RectTransform selfRTrans;
    RectTransform childRTrans;
    RectTransform SelfRTrans { get => selfRTrans == null ? (selfRTrans = GetComponent<RectTransform>()) : selfRTrans; }
    RectTransform ChildRTrans { get => childRTrans == null ? (childRTrans = usableImage.GetComponent<RectTransform>()) : childRTrans; }

    Vector3 initPos;

    public void SetSize(in Vector2 size)
    {
        ChildRTrans.sizeDelta = SelfRTrans.sizeDelta = size;
        ChildRTrans.localScale = SelfRTrans.localScale = Vector3.one;
    }
    public void SetBlinkCycle(float time)
    {
        blinkCycle = time;
        blinkSeq =
            DOTween.Sequence()
            .OnStart(() => bgImage.color = new Color(0.5f, 0, 0))
            .Append(bgImage.DOColor(Color.black, blinkCycle))
            .SetLoops(-1,LoopType.Restart);
        blinkSeq.Pause();
    }
    public void SetUmbrellaWork(bool does)
    {
        UmbrellaWorks = does;
    }
}
