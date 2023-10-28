using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UIAudioConfig : MonoBehaviour
{
    [SerializeField] private AudioClip audioClipSeVolumeChanged;
    [SerializeField] private Sprite iconSpriteVocal;
    [SerializeField] private Sprite iconSpriteMute;
    
    [SerializeField] private Slider sliderVolumeMaster;
    [SerializeField] private Slider sliderVolumeBgm;
    [SerializeField] private Slider sliderVolumeSe;
    [SerializeField] private TMP_Text textValueVolumeMaster;
    [SerializeField] private TMP_Text textValueVolumeBgm;
    [SerializeField] private TMP_Text textValueVolumeSe;
    [SerializeField] private Image muteIconVolumeMaster;
    [SerializeField] private Image muteIconVolumeBgm;
    [SerializeField] private Image muteIconVolumeSe;
    [SerializeField] private Button buttonReset;
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private AudioSource _audioSource;
    
    // Start is called before the first frame update
    void Start()
    {
        sliderVolumeMaster.onValueChanged.AddListener((value) =>
        {
            var prop = value / sliderVolumeMaster.maxValue;
            SetExposedValue("VolumeMaster", ProportionToDecibel(prop));
            UpdateElements(prop, textValueVolumeMaster, muteIconVolumeMaster);
        });
        sliderVolumeBgm.onValueChanged.AddListener((value) =>
        {
            var prop = value / sliderVolumeBgm.maxValue;
            SetExposedValue("VolumeBgm", ProportionToDecibel(prop));
            UpdateElements(prop, textValueVolumeBgm, muteIconVolumeBgm);
        });
        sliderVolumeSe.onValueChanged.AddListener((value) =>
        {
            var prop = value / sliderVolumeSe.maxValue;
            SetExposedValue("VolumeSe", ProportionToDecibel(prop)); 
            _audioSource.PlayOneShot(audioClipSeVolumeChanged);
            UpdateElements(prop, textValueVolumeSe, muteIconVolumeSe);
        });
        
        buttonReset.onClick.AddListener(ResetVolumes);

        FetchVolumes();
    }

    void SetExposedValue(string key, float value)
    {
        _audioMixer.SetFloat(key, value);
    }

    void ResetVolumes()
    {
        SetExposedValue("VolumeMaster", 0);
        SetExposedValue("VolumeBgm", 0);
        SetExposedValue("VolumeSe", 0);

        FetchVolumes();
    }

    void FetchVolumes()
    {
        if (!_audioMixer.GetFloat("VolumeMaster", out float volumeMaster))
        {
            throw new InvalidOperationException();
        }
        
        if (!_audioMixer.GetFloat("VolumeBgm", out float volumeBgm))
        {
            throw new InvalidOperationException();
        }
        
        if (!_audioMixer.GetFloat("VolumeSe", out float volumeSe))
        {
            throw new InvalidOperationException();
        }

        UpdateElements(DecibelToProportion(volumeMaster), textValueVolumeMaster, muteIconVolumeMaster, sliderVolumeMaster);
        UpdateElements(DecibelToProportion(volumeBgm), textValueVolumeBgm, muteIconVolumeBgm, sliderVolumeBgm);
        UpdateElements(DecibelToProportion(volumeSe), textValueVolumeSe, muteIconVolumeSe, sliderVolumeSe);
    }

    void UpdateElements(float volume, TMP_Text textValue, Image muteIcon, Slider slider = null)
    {
        textValue.SetText("{0:0}", volume*100.0f);
        muteIcon.sprite = volume <= 0.0 ? iconSpriteMute : iconSpriteVocal;
        if (slider != null)
        {
            slider.SetValueWithoutNotify(volume*slider.maxValue);
        }
    }
    
    float ProportionToDecibel(float proportion)
    {
        if (proportion <= 0.0f)
        {
            return -80.0f;
        }
        
        return 20.0f*Mathf.Log10(Mathf.Lerp(0.0001f, 1.0f, proportion));
    }

    float Curve(float volumeProp) => Log10(volumeProp);

    float DecibelToProportion(float decibel)
    {
        if (Mathf.Approximately(decibel, -80.0f))
        {
            return 0.0f;
        }
        
        return Mathf.InverseLerp(0.0001f, 1.0f, Mathf.Pow(10, decibel / 20));
    }

    float CurveInv(float volumeProp) => Exp10(volumeProp);

    float Linear(float t)
    {
        return t;
    }

    float Exp10(float t)
    {
        return (Mathf.Pow(10.0f,t) - 1.0f) / 9f;
    }
    
    float Log10(float t)
    {
        return Mathf.Log(t * 9.0f + 1.0f);
    }

    public void SetMaster(float value)
    {
        SetExposedValue("VolumeMaster", ProportionToDecibel(value));
    }
    public void SetBgm(float value)
    {
        SetExposedValue("VolumeBgm", ProportionToDecibel(value));
    }
    public void SetSe(float value)
    {
        SetExposedValue("VolumeSe", ProportionToDecibel(value));
    }
}
