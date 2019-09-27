using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] WipeEffet inWipeEffet;
    [SerializeField] WipeEffet outWipeEffet;
    [SerializeField] string defaultDestinationSceneName;

    public void ChangeSceneDefault()
    {
        ChangeScene(defaultDestinationSceneName);
    }

    public void ChangeScene(string destinationSceneName)
    {
        ChangeScene(destinationSceneName, null);
    }

    public void ChangeScene(string destinationSceneName, System.Action<UnityEngine.SceneManagement.Scene> sceneInitializer)
    {
        TransitionEffect.InWipeEffet = inWipeEffet;
        TransitionEffect.OutWipeEffet = outWipeEffet;
        SceneTransitionManager.TransScene(destinationSceneName, sceneInitializer);
    }
    public void ChngeSceneTimed(float time)
    {
        StartCoroutine(WaitCo(defaultDestinationSceneName, time));
    }

    IEnumerator WaitCo(string dest,float time)
    {
        yield return new WaitForSeconds(time);
        ChangeScene(dest);
    }
}
