using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SoundManager : SoundManagerScriptable.Mono
{
    const string addressableAddress = "Assets/Prefabs/Essentials/SoundManager.prefab";

    static SoundManager instance;

    public static SoundManager Instance { get => instance; }

    protected override void Awake()
    {
        base.Awake();
        instance = this;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void RuntimeInitializeOnLoad()
    {
        Addressables.InstantiateAsync(addressableAddress);
    }

}

