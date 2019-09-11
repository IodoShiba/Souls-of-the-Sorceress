using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAwakeGaugeViewer : ActionAwake.Viewer
{
    [SerializeField] float maxFillAmount;
    [SerializeField] UnityEngine.UI.Image _awakeGaugeImage;

    // Update is called once per frame
    private void Update()
    {
        _awakeGaugeImage.fillAmount = Mathf.Clamp(AwakeGauge * 2 * maxFillAmount, 0, maxFillAmount);
    }
}
