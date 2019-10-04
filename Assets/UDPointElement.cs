using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UDPointElement : MonoBehaviour
{
    [SerializeField] bool usable;
    [SerializeField] UnityEngine.UI.Image usableImage;
    public bool Usable {
        get => usable;
        set {
            usableImage.enabled = usable = value;
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
}
