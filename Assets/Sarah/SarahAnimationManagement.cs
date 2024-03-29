﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SarahAnimationManagement : MonoBehaviour
{
    [SerializeField]
    List<SkinnedMeshRenderer> ordinaryMeshRenderers,awakenMeshRenderers,blueAwakenMeshRenderers;
    [SerializeField]
    List<GameObject> gameObject_ordinary, gameObject_awaken;
    [SerializeField] GameObject fire,particle;
    [SerializeField]
    TrailRenderer trailRenderer;
    ActionAwake actionAwake;

    private void Awake()
    {
        actionAwake = GetComponentInParent<ActionAwake>();
    }

    public void FireON()
    {
        if (actionAwake.IsActive)
        {
            fire.SetActive(true);
            particle.SetActive(true);
        }
    }

    public void FireOFF()
    {
        fire.SetActive(false);
        particle.SetActive(false);
    }

    public void ChangeApperance(ActionAwake.AwakeLevels mode)
    {
        foreach (GameObject gameObject in gameObject_ordinary)
        {
            gameObject.SetActive(false);
        }
        foreach (GameObject gameObject in gameObject_awaken)
        {
            gameObject.SetActive(false);
        }
        foreach (SkinnedMeshRenderer r in ordinaryMeshRenderers)
        {
            r.enabled = false;
        }
        foreach (SkinnedMeshRenderer r in awakenMeshRenderers)
        {
            r.enabled = false;
        }
        foreach (SkinnedMeshRenderer r in blueAwakenMeshRenderers)
        {
            r.enabled = false;
        }
        switch (mode)
        {
            case ActionAwake.AwakeLevels.ordinary:
                foreach (GameObject gameObject in gameObject_ordinary)
                {
                    gameObject.SetActive(true);
                }
                foreach (SkinnedMeshRenderer r in ordinaryMeshRenderers)
                {
                    r.enabled = true;
                }
                break;
            case ActionAwake.AwakeLevels.awaken:
                foreach (GameObject gameObject in gameObject_awaken)
                {
                    gameObject.SetActive(true);
                }
                foreach (SkinnedMeshRenderer r in awakenMeshRenderers)
                {
                    r.enabled = true;
                }
                break;
            case ActionAwake.AwakeLevels.blueAwaken:
                foreach (GameObject gameObject in gameObject_awaken)
                {
                    gameObject.SetActive(true);
                }
                foreach (SkinnedMeshRenderer r in blueAwakenMeshRenderers)
                {
                    r.enabled = true;
                }
                
                break;
        }
    }
}
