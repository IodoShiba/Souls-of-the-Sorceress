using UnityEngine;
using SotS;
using UnityEngine.SceneManagement;

public static class CheckPointSetter
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void RuntimeInitialize()
    {
        SceneManager.sceneLoaded += 
            (scene, mode) => 
            {
                string sceneName = scene.name;
                if(sceneName == SceneName.gameOver || sceneName == SceneName.transitionAdd){ return; }

                ReviveController.SetTargetSceneName(sceneName);
            };
    }
}
