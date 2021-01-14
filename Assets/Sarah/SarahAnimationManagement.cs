using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SarahAnimationManagement : MonoBehaviour
{
    [SerializeField]
    List<SkinnedMeshRenderer> ordinaryMeshRenderers,awakenMeshRenderers,blueAwakenMeshRenderers;
    public GameObject particle,fire, trail, trail_awaken;
    [SerializeField]
    TrailRenderer trailRenderer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ChangeMesh(ActionAwake.AwakeLevels mode)
    {
        trail.SetActive(false);
        trail_awaken.SetActive(false);
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
                foreach (SkinnedMeshRenderer r in ordinaryMeshRenderers)
                {
                    r.enabled = true;
                }
                trail.SetActive(true);
                fire.SetActive(false);
                break;
            case ActionAwake.AwakeLevels.awaken:
                foreach (SkinnedMeshRenderer r in awakenMeshRenderers)
                {
                    r.enabled = true;
                }
                trail_awaken.SetActive(true);
                fire.SetActive(true);
                break;
            case ActionAwake.AwakeLevels.blueAwaken:
                foreach (SkinnedMeshRenderer r in blueAwakenMeshRenderers)
                {
                    r.enabled = true;
                }
                fire.SetActive(true);
                break;
        }
    }
}
