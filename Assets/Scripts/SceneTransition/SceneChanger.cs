using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] WipeEffet inWipeEffet;
    [SerializeField] WipeEffet outWipeEffet;
    [SerializeField] string defaultDestinationSceneName;

    public void ChangeSeneDefault()
    {
        ChangeSene(defaultDestinationSceneName);
    }

    public void ChangeSene(string destinationSceneName)
    {
        TransitionEffect.InWipeEffet = inWipeEffet;
        TransitionEffect.OutWipeEffet = outWipeEffet;
        SceneTransitionManager.TransScene(destinationSceneName, null);
    }
}
