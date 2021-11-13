using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

public class EffectAnimation : MonoBehaviour
{
    [SerializeField] AnimationClip clip;

    PlayableGraph playableGraph;
    AnimationClipPlayable currentPlayable;
    AnimationPlayableOutput aPlayableOut;

    const string outputName = "output";

    public AnimationClip Clip { get => clip;
        private set
        {
            playableGraph.Stop();

            if (currentPlayable.IsValid()) { currentPlayable.Destroy(); }
            currentPlayable = AnimationClipPlayable.Create(playableGraph, value);
            currentPlayable.SetDuration(clip.length);
            aPlayableOut.SetSourcePlayable(currentPlayable);
            playableGraph.Play();

            clip = value;
        }
    }

    private void Awake()
    {
        playableGraph = PlayableGraph.Create();

        currentPlayable = AnimationClipPlayable.Create(playableGraph, clip);
        currentPlayable.SetDuration(clip.length);

        aPlayableOut = AnimationPlayableOutput.Create(playableGraph, outputName, GetComponent<Animator>());

        aPlayableOut.SetSourcePlayable(currentPlayable);

        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!currentPlayable.IsValid() || currentPlayable.IsDone())
        {
            transform.position = Vector3.zero;
            gameObject.SetActive(false);
        }
    }

    public void Play(AnimationClip animationClip,Vector3 position,Vector3 localscale)
    {
        if (animationClip == null)
        {
            throw new System.NullReferenceException("Argument 'animationClip' cannot be null.");
        }
        gameObject.SetActive(true);
        transform.position = position;
        transform.localScale = localscale;
        Clip = animationClip;
    }

    public void Play(AnimationClip animationClip,Vector3 position)
    {
        Play(animationClip,position,Vector3.one);
    }

    private void OnDestroy()
    {
        playableGraph.Destroy();
    }


}
