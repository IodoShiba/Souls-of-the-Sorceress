﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAwakeGaugeViewer : ActionAwake.Viewer
{

    [SerializeField] UnityEngine.UI.Image _awakeGaugeImage;

    // Update is called once per frame
    private void Update()
    {
        _awakeGaugeImage.fillAmount = AwakeGauge * 0.6f;
    }
}