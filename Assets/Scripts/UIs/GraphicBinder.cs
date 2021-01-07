using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphicBinder : Graphic
{
    [SerializeField] Graphic[] graphics;

    public override void CrossFadeColor(Color targetColor, float duration, bool ignoreTimeScale, bool useAlpha, bool useRGB)
    {
        for(int i=0; i<graphics.Length; ++i){graphics[i].CrossFadeColor(targetColor, duration, ignoreTimeScale, useAlpha, useRGB);}
    }
}
