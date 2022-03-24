using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx.Async;

public class SceneTransitionManager : MonoBehaviour
{
    static SceneTransitionManager instance = null;

    [SerializeField] TransitionEffect transitionEffect;
    [SerializeField] GameObject sceneMemberRoot;

    static string targetScene = null;
    static string originScene = null;
    static System.Action<Scene> sceneInitializer = null;
    static bool targetSceneLoaded;

    public const string transitionSceneName = "TransitionAddScene";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void RuntimeInitializeOnLoad()
    {
        Initialize();
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            if (scene.name == targetScene)
            {
                targetSceneLoaded = true;
            }
        };
    }

    public static void Initialize()
    {
        targetScene = null;
        originScene = null;
        sceneInitializer = null;
    }
    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this);
            return;
        }
        instance = this;

        if (string.IsNullOrEmpty(targetScene)) { return; }

        StartCoroutine(TransCo());
    }

    public IEnumerator TransCo()
    {
        transitionEffect.StartEffect(true);
        while (transitionEffect.IsOnEffect) { yield return null; }

        SceneManager.UnloadSceneAsync(originScene);

        targetSceneLoaded = false;
        SceneManager.LoadScene(targetScene, LoadSceneMode.Additive);

        while (!targetSceneLoaded) { yield return null; }

        var targetSceneObj = SceneManager.GetSceneByName(targetScene);
        SceneManager.SetActiveScene(targetSceneObj);

        if (sceneInitializer != null)
        {
            sceneInitializer(targetSceneObj);
        }

        transitionEffect.StartEffect(false);
        while (transitionEffect.IsOnEffect) { yield return null; }

        SceneTransitionManager.targetScene = null;
        SceneTransitionManager.sceneInitializer = null;
        SceneTransitionManager.originScene = null;

        GameObject[] roots = SceneManager.GetSceneByName(transitionSceneName).GetRootGameObjects();
        for(int i = 0; i < roots.Length; ++i)
        {
            if(roots[i] == sceneMemberRoot) { continue; }
            SceneManager.MoveGameObjectToScene(roots[i], targetSceneObj);
        }

        SceneManager.UnloadSceneAsync(transitionSceneName);
    }

    public static void TransScene(string targetScene, System.Action<Scene> sceneInitializer)
    {
        if (!string.IsNullOrEmpty(SceneTransitionManager.targetScene)) { return; }

        SceneTransitionManager.targetScene = targetScene;
        SceneTransitionManager.sceneInitializer = sceneInitializer;
        SceneTransitionManager.originScene = SceneManager.GetActiveScene().name;

        SceneManager.LoadScene(transitionSceneName, LoadSceneMode.Additive);
        //SceneManager.SetActiveScene(SceneManager.GetSceneByName(transitionSceneName));
    }
}
