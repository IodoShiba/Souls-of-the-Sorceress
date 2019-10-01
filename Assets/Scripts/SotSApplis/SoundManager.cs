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
    


    public class Scriptable : ScriptableObject
    {
        [SerializeField] SoundManager prefab;

        [Space(16)]
        [SerializeField] AudioClip submit;
        [SerializeField] AudioClip cancel;
        [SerializeField] AudioClip moveCursor;
        [SerializeField] AudioClip attack;
        [SerializeField] AudioClip stageClear;
        [SerializeField] AudioClip stageReleased;


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

