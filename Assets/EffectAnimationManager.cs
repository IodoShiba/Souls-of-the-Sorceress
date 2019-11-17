using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectAnimationManager : MonoBehaviour
{
    [SerializeField] int poolSize;
    [SerializeField] EffectAnimation prefab;
    EffectAnimation[] pool;
    
    static EffectAnimationManager instance = null;
    int nextIndex;

    public static EffectAnimationManager Instance { get => instance; }

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null) { instance = this; }
        else if (this != instance)
        {
            Debug.LogError($"Two or more {this.GetType().Name} cannot exist in one scene. GameObject '{name}' has been Deleted because it has second {this.GetType().Name}.");
            Destroy(gameObject);
            return;
        }

        pool = new EffectAnimation[poolSize];
        for(int i = 0; i < pool.Length; ++i)
        {
            pool[i] = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            pool[i].transform.SetParent(transform);
        }
        nextIndex = 0;

    }

    void _Play(AnimationClip animationClip,in Vector3 position)
    {
        for (int i = 0; i < pool.Length; ++i)
        {
            if (pool[nextIndex] != null && !pool[nextIndex].gameObject.activeSelf)
            {
                break;
            }
            nextIndex = (nextIndex + 1) % pool.Length;
        }

        if(pool[nextIndex] == null) { return; }
        pool[nextIndex].Play(animationClip, position);

        nextIndex = (nextIndex + 1) % pool.Length;
    }

    static public void Play(AnimationClip animationClip,in Vector3 position)
    {
        Instance._Play(animationClip,position);
    }

}
