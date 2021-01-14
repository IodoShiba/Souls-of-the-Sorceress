using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Async;
using DG.Tweening;

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

    float BgmVolume => instantiated.BgmVolume;

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

    public void StopBgm() {instantiated.StopBgm();}
    public void FadeOutBgm(float fadeDuration){instantiated.FadeOutBgm(fadeDuration);}

    public class Mono : MonoBehaviour
    {
        [SerializeField] SoundManagerScriptable scriptable;
        [SerializeField] AudioSource seAudioSource;
        [SerializeField] AudioSource bgmAudioSource;

        float cachedBgmVolume;

        public float BgmVolume 
        {
            get => bgmAudioSource.volume;
            set 
            {
                bgmAudioSource.volume = cachedBgmVolume = value;
            }
        }

        protected void SetInstantiated()
        {
            DontDestroyOnLoad(gameObject);
            scriptable.instantiated = this;
            Initialize();
        }
            
        void Initialize()
        {
            cachedBgmVolume = bgmAudioSource.volume;
        }

        public void PlaySE(AudioClip audioClip) => seAudioSource.PlayOneShot(audioClip);
        public void PlaySE(AudioClip audioClip, float volumeScale) => seAudioSource.PlayOneShot(audioClip, volumeScale);

        public void PlayBgm(AudioClip audioClip) => PlayBgm(audioClip, false);
        public void PlayBgm(AudioClip audioClip, bool forceReplay) 
        { 
            if(!forceReplay && bgmAudioSource.clip == audioClip) { return; }

            bgmAudioSource.clip = audioClip; 
            ResetBgmVolume();
            bgmAudioSource.Play();
        }

        public void StopBgm()
        {
            bgmAudioSource.clip = null;
        }

        public void FadeOutBgm(float fadeDuration) => FadeOutBgmAsync(fadeDuration).Forget();
        public async UniTaskVoid FadeOutBgmAsync(float fadeDuration = 0)
        {
            if(fadeDuration <= 0){StopBgm(); return;}

            float volume = bgmAudioSource.volume;
            for(float restTime = fadeDuration; restTime > 0; restTime -= Time.deltaTime)
            {
                bgmAudioSource.volume = volume * (restTime/fadeDuration);
                await UniTask.Yield();
            }

            StopBgm();
            ResetBgmVolume();
        }

        public void ResetBgmVolume()
        {
            bgmAudioSource.volume = cachedBgmVolume;
        }
    } 
}
