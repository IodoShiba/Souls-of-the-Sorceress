using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthViewer : Player.Viewer
{
    [SerializeField, Range(0, 1)] float health;

    //[SerializeField] UnityEngine.UI.Slider _healthSlider;
    [SerializeField] UnityEngine.UI.Image healthGaugeImage;

    // Update is called once per frame
    void Update()
    {
        health = Health / MaxHealth;
        //_healthSlider.value = health;
        healthGaugeImage.fillAmount = health;
    }

}
