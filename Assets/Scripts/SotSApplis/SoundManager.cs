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

    public void OnBeforeSerialize()
    {
        throw new System.NotImplementedException();
    }

    public void OnAfterDeserialize()
    {
        throw new System.NotImplementedException();
    }

    public class Scriptable : ScriptableObject
    {
        [SerializeField] protected SoundManager prefab;
        static protected SoundManager sPrefab;

        [Space(16)]
        [SerializeField] AudioClip submit;
        [SerializeField] AudioClip cancel;
        [SerializeField] AudioClip moveCursor;
        [SerializeField] AudioClip attack;
        [SerializeField] AudioClip stageClear;
        [SerializeField] AudioClip stageReleased;

        
        [RuntimeInitializeOnLoadMethod]
        static void RuntimeInitializeOnLoad(SoundManager prefab)
        {
            DontDestroyOnLoad(SoundManager.instance = Instantiate(prefab, Vector3.zero, Quaternion.identity));
            Instance.Initialize();
        }

        public void PlayOneShot(AudioClip audioClip) => Instance.PlayOneShot(audioClip);
        public void PlayOneShot(AudioClip audioClip, float volumeScale) => Instance.PlayOneShot(audioClip, volumeScale);


    }
}

