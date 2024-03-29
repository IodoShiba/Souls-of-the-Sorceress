﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAwakeGaugeViewer : ActionAwake.Viewer
{
    [SerializeField] float maxFillAmount;
    [SerializeField] UnityEngine.UI.Image _awakeGaugeImage;
    [SerializeField] Animator flameAniamtor;
    [SerializeField] UnityEngine.UI.Image gaugeBeatImage;
    [SerializeField] ParticleSystem waveParticleSys;

    static readonly int beatHash = Animator.StringToHash("Beat");
    static readonly int idleHash = Animator.StringToHash("Idle");

    bool ableToAwake;
    bool AbleToAwake
    {
        get => ableToAwake;
        set
        {
            if(!ableToAwake && value)
            {
                flameAniamtor.Play(beatHash);
                gaugeBeatImage.enabled = true;
            }
            if(ableToAwake && !value)
            {
                flameAniamtor.Play(idleHash);
                gaugeBeatImage.enabled = false;
            }
            ableToAwake = value;
        }
    }

    private void Start()
    {
        flameAniamtor.Play(idleHash);
        gaugeBeatImage.enabled = false;
    }
    // Update is called once per frame
    private void Update()
    {
        _awakeGaugeImage.fillAmount = Mathf.Clamp(AwakeGauge * 2 * maxFillAmount, 0, maxFillAmount);
        AbleToAwake = target.IsAbleToAwake;

        // if (AbleToAwake && waveParticleSys.isStopped)
        // {
        //     waveParticleSys.Play();
        // }
        // else if(!AbleToAwake && waveParticleSys.isPlaying)
        // {
        //     waveParticleSys.Stop();
        // }
    }
}
