using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshManagement : MonoBehaviour
{
    [SerializeField]
    List<SkinnedMeshRenderer> ordinaryMeshRenderers,awakenMeshRenderers,blueAwakenMeshRenderers;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ChangeMesh(ActionAwake.AwakeLevels mode)
    {
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
                break;
            case ActionAwake.AwakeLevels.awaken:
                foreach (SkinnedMeshRenderer r in awakenMeshRenderers)
                {
                    r.enabled = true;
                }
                break;
            case ActionAwake.AwakeLevels.blueAwaken:
                foreach (SkinnedMeshRenderer r in blueAwakenMeshRenderers)
                {
                    r.enabled = true;
                }
                break;
        }
    }
}
