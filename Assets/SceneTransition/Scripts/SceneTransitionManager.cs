using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UniRx.Async;

public class SceneTransitionManager : MonoBehaviour
{
    [System.Serializable] public class UnityEventLoadingScene : UnityEvent<bool> {}

    static SceneTransitionManager instance = null;

    [SerializeField] TransitionEffect transitionEffect;
    [SerializeField] GameObject sceneMemberRoot;

    public UnityEventLoadingScene eventIsLoadingScene;

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

        TransCo().Forget();
        //StartCoroutine(TransCo());
    }

    public async UniTask TransCo()
    {
        transitionEffect.StartEffect(true);
        //while (transitionEffect.IsOnEffect) { yield return null; }
        await UniTask.WaitWhile(()=>transitionEffect.IsOnEffect);

        originScene = SceneManager.GetActiveScene().name;
        await SceneManager.UnloadSceneAsync(originScene);

        targetSceneLoaded = false;
        Debug.Log($"Load Start {targetScene}");
        eventIsLoadingScene.Invoke(true);
        // await UniTask.Delay(System.TimeSpan.FromSeconds(5));
        await SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive);
        eventIsLoadingScene.Invoke(false);
        Debug.Log($"Load End {targetScene}");

        //while (!targetSceneLoaded) { yield return null; }
        //await UniTask.WaitUntil(()=>targetSceneLoaded);

        var targetSceneObj = SceneManager.GetSceneByName(targetScene);
        SceneManager.SetActiveScene(targetSceneObj);

        if (sceneInitializer != null)
        {
            sceneInitializer(targetSceneObj);
        }

        transitionEffect.StartEffect(false);
        //while (transitionEffect.IsOnEffect) { yield return null; }
        await UniTask.WaitWhile(()=>transitionEffect.IsOnEffect);

        SceneTransitionManager.targetScene = null;
        SceneTransitionManager.sceneInitializer = null;
        SceneTransitionManager.originScene = null;

        GameObject[] roots = SceneManager.GetSceneByName(transitionSceneName).GetRootGameObjects();
        for(int i = 0; i < roots.Length; ++i)
        {
            if(roots[i] == sceneMemberRoot) { continue; }
            SceneManager.MoveGameObjectToScene(roots[i], targetSceneObj);
        }

        await SceneManager.UnloadSceneAsync(transitionSceneName);
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
