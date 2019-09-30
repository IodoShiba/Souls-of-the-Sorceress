using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    static SoundManager instance;
    static SoundManager Instance { get => instance; }

    public void Initialize()
    {

    }

    public void PlayOneShot(AudioClip audioClip) => audioSource.PlayOneShot(audioClip);
    public void PlayOneShot(AudioClip audioClip, float volumeScale) => audioSource.PlayOneShot(audioClip, volumeScale);
    
    public class SoundManagerScriptable : ScriptableObject
    {
        [SerializeField] SoundManager prefab;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        void RuntimeInitializeOnLoad()
        {
            DontDestroyOnLoad(SoundManager.instance = Instantiate(prefab, Vector3.zero, Quaternion.identity));
            Instance.Initialize();
        }

        public void PlayOneShot(AudioClip audioClip) => Instance.PlayOneShot(audioClip);
        public void PlayOneShot(AudioClip audioClip, float volumeScale) => Instance.PlayOneShot(audioClip, volumeScale);

    }
}

