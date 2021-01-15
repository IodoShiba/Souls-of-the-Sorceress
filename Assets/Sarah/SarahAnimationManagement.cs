using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SarahAnimationManagement : MonoBehaviour
{
    [SerializeField]
    List<SkinnedMeshRenderer> ordinaryMeshRenderers,awakenMeshRenderers,blueAwakenMeshRenderers;
    [SerializeField]
    List<GameObject> gameObject_ordinary, gameObject_awaken;
    [SerializeField]
    TrailRenderer trailRenderer;

    // Start is called before the first frame update
    void Start()
    {
        
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
