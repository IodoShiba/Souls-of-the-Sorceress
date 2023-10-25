using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UIAudioConfig : MonoBehaviour
{
    [SerializeField] private Slider sliderVolumeMaster;
    [SerializeField] private Slider sliderVolumeBgm;
    [SerializeField] private Slider sliderVolumeSe;
    [SerializeField] private Button buttonReset;
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip audioClipSeVolumeChanged;
    
    // Start is called before the first frame update
    void Start()
    {
        sliderVolumeMaster.onValueChanged.AddListener((value) => { SetExposedValue("VolumeMaster", ProportionToDecibel(value));});
        sliderVolumeBgm.onValueChanged.AddListener((value) => { SetExposedValue("VolumeBgm", ProportionToDecibel(value));});
        sliderVolumeSe.onValueChanged.AddListener((value) => { SetExposedValue("VolumeSe", ProportionToDecibel(value)); _audioSource.PlayOneShot(audioClipSeVolumeChanged);});
        buttonReset.onClick.AddListener(ResetVolumes);
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
    }

    float ProportionToDecibel(float proportion)
    {
        // return Mathf.Lerp(-80.0f,0.0f, Mathf.Exp(proportion)/(Mathf.Exp(1.0f)-1.0f));
        return Mathf.Lerp(-80.0f,0.0f, proportion);
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
