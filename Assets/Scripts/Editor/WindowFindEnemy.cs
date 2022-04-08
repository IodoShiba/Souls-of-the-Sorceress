using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class WindowFindEnemy : EditorWindow
{
    List<GameObject> enemyHolders = new List<GameObject>();
    Vector2 scrpos;
    UnityEngine.Events.UnityAction<Scene, Scene> activeSceneChangedAction;

    [MenuItem("Window/Window Find Enemy")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        WindowFindEnemy window = EditorWindow.GetWindow<WindowFindEnemy>();
        window.Show();
    }

    void Awake()
    {
        activeSceneChangedAction = (sfrom, sto) => {enemyHolders.Clear();};
        EditorSceneManager.activeSceneChanged += activeSceneChangedAction;
    }

    void OnDestroy()
    {
        EditorSceneManager.activeSceneChanged -= activeSceneChangedAction;
    }

    void OnGUI()
    {
        if(GUILayout.Button("Find Enemies"))
        {
            Find();
        }

        EditorGUILayout.LabelField("Enemy Holder Count: ", enemyHolders.Count.ToString());

        using (var scrscope = new EditorGUILayout.ScrollViewScope(scrpos))
        {
            for(int i=0;i<enemyHolders.Count; ++i)
            {
                if (GUILayout.Button(enemyHolders[i].name))
                {
                    Selection.activeGameObject = enemyHolders[i];
                }
            }
            scrpos = scrscope.scrollPosition;
        }
    }

    void Find()
    {
        enemyHolders.Clear();
        Scene sceneStruct = EditorSceneManager.GetActiveScene();;
        GameObject[] roots = sceneStruct.GetRootGameObjects();
        for (int i=0; i<roots.Length; ++i)
        {
            enemyHolders.AddRange(roots[i].GetComponentsInChildren<Enemy>(true).Select(e=>e.gameObject));
        }
    }
}