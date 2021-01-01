using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObject/SoundManager")]
public class SoundManagerScriptable : ScriptableObject
{
    Mono instantiated;

    [Space(16)]
    [SerializeField] AudioClip submit;
    [SerializeField] AudioClip cancel;
    [SerializeField] AudioClip moveCursor;
    [SerializeField] AudioClip attack;
    [SerializeField] AudioClip stageClear;
    [SerializeField] AudioClip stageReleased;

    public void PlayOneShot(AudioClip audioClip) => instantiated.PlayOneShot(audioClip);
    public void PlayOneShot(AudioClip audioClip, float volumeScale) => instantiated.PlayOneShot(audioClip, volumeScale);

    public class Mono : MonoBehaviour
    {
        [SerializeField] SoundManagerScriptable scriptable;
        [SerializeField] AudioSource audioSource;

        protected virtual void Awake()
        {
            DontDestroyOnLoad(gameObject);
            scriptable.instantiated = this;
        }
            
        public void PlayOneShot(AudioClip audioClip) => audioSource.PlayOneShot(audioClip);
        public void PlayOneShot(AudioClip audioClip, float volumeScale) => audioSource.PlayOneShot(audioClip, volumeScale);
    }
}
