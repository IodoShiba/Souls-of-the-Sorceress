using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Async;

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

    public void PlaySE(AudioClip audioClip) => instantiated.PlaySE(audioClip) ;
    public void PlaySE(AudioClip audioClip, float volumeScale) => instantiated.PlaySE(audioClip, volumeScale);
    public async void PlayBgm(AudioClip audioClip) 
    { 
        if(instantiated == null){ await UniTask.WaitUntil(()=>instantiated!=null); }

        instantiated.PlayBgm(audioClip); 
    
    }
    public async void PlayBgm(AudioClip audioClip, bool forceReplay) 
    { 
        if(instantiated == null){ await UniTask.WaitUntil(()=>instantiated!=null); }
        
        instantiated.PlayBgm(audioClip, forceReplay);
    }

    public class Mono : MonoBehaviour
    {
        [SerializeField] SoundManagerScriptable scriptable;
        [SerializeField] AudioSource seAudioSource;
        [SerializeField] AudioSource bgmAudioSource;

        protected void SetInstantiated()
        {
            DontDestroyOnLoad(gameObject);
            scriptable.instantiated = this;
        }
            
        public void PlaySE(AudioClip audioClip) => seAudioSource.PlayOneShot(audioClip);
        public void PlaySE(AudioClip audioClip, float volumeScale) => seAudioSource.PlayOneShot(audioClip, volumeScale);

        public void PlayBgm(AudioClip audioClip) => PlayBgm(audioClip, false);
        public void PlayBgm(AudioClip audioClip, bool forceReplay) 
        { 
            if(!forceReplay && bgmAudioSource.clip == audioClip) { return; }

            bgmAudioSource.clip = audioClip; 
            bgmAudioSource.Play();
        }

        public void SetBgmVolume(float amount) => bgmAudioSource.volume = amount;
    }
}
