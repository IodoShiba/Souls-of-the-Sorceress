using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfoViewer : Player.Viewer
{
    [SerializeField, Range(0, 1)] float health;
    [SerializeField, Range(0, 1)] float awake;
    [SerializeField, Range(0, 8)] int umbrellaDurability;

    [SerializeField] UnityEngine.UI.Slider _healthSlider;
    [SerializeField] UnityEngine.UI.Image _awakeGaugeImage;
    [SerializeField] UnityEngine.UI.Toggle[] _UDurabilityToggles;

    // Update is called once per frame
    void Update()
    {
        UpdateHealth();
        UpdateAwake();
        UpdateUmbrellaDurability();
    }

    private void UpdateUmbrellaDurability()
    {
        for (int i=0; i< _UDurabilityToggles.Length; ++i)
        {
            _UDurabilityToggles[i].isOn = i < umbrellaDurability;
        }
    }

    private void UpdateAwake()
    {
        _awakeGaugeImage.fillAmount = awake * 0.6f;
    }

    private void UpdateHealth()
    {
        health = Health / MaxHealth;
        _healthSlider.value = health;
    }
}
