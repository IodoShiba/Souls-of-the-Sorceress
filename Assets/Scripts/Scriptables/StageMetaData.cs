using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "StageMetaData")]
public class StageMetaData : ScriptableObject
{
    public const string path = "Assets/Resources/SotSApplis/Stage Meta Data.asset";

    public enum Stage
    {
        StageEX = -1,
        Stage0 = 0,
        Stage1 = 1,
        Stage2 = 2,
        Stage3 = 3,
        Stage4 = 4,
    }

    [System.Serializable]
    public struct SceneEntry
    {
        public SceneAsset scene;
        [DisabledField] public int enemyCount;
    }

    [System.Serializable]
    public struct StageEntry
    {
        public Stage stage;
        public List<SceneEntry> sceneEntries;

        public StageEntry(Stage stage = default)
        {
            this.stage = stage;
            sceneEntries = new List<SceneEntry>() {default};
        }
    }

    // [SerializeField] List<SceneEntry> sceneEntry = new List<SceneEntry>();
    [DisabledField, SerializeField] List<StageEntry> stageEntries = new List<StageEntry>();




#if UNITY_EDITOR
    public class StageMetaDataWindowBase : EditorWindow
    {
        bool initialized = false;
        StageMetaData target;
        Vector2 scrpos = Vector2.zero;

        async void OnEnable()
        {
            if(initialized) {return;}

            var t = AssetDatabase.LoadAssetAtPath(path, typeof(StageMetaData));
            Debug.Assert(t != null);
            target = t as StageMetaData;

            initialized = true;
        }

        public void OnGUI()
        {   
            if(target == null){return;}

            using(var scrscope = new EditorGUILayout.ScrollViewScope(scrpos))
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Stage Entries");
                for (int i=0; i<target.stageEntries.Count; ++i)
                {
                    var stageEntry = target.stageEntries[i];
                    EditorGUILayout.Space();
                    
                    using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                    {
                        var doDeleteStage = GUILayout.Button("x", GUILayout.Width(24));
                        
                        EditorGUILayout.BeginVertical();

                        stageEntry.stage = (StageMetaData.Stage)EditorGUILayout.EnumPopup("Stage: ", target.stageEntries[i].stage);

                        if (stageEntry.sceneEntries == null)
                        {
                            stageEntry.sceneEntries = new List<SceneEntry>();
                        }

                        for(int j=0;j<stageEntry.sceneEntries.Count;++j)
                        {
                            var sceneEntry = stageEntry.sceneEntries[j];
                            
                            using(new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                            {
                                var doDeleteScene = GUILayout.Button("x", GUILayout.Width(24));
                                
                                EditorGUILayout.BeginVertical();
                                
                                sceneEntry.scene = EditorGUILayout.ObjectField("ScenePath", sceneEntry.scene, typeof(SceneAsset), false) as SceneAsset;
                                EditorGUILayout.LabelField("Enemy Count: ", sceneEntry.enemyCount.ToString());
                                if (GUILayout.Button("Update Meta Data"))
                                {
                                    var currentOpenScenePath = EditorSceneManager.GetActiveScene().path;
                                    UpdateSceneMetaData(ref sceneEntry);
                                    EditorSceneManager.OpenScene(currentOpenScenePath);
                                }

                                EditorGUILayout.EndVertical();

                                if(doDeleteScene)
                                {
                                    stageEntry.sceneEntries.RemoveAt(j);
                                    break;
                                }
                            }

                            stageEntry.sceneEntries[j] = sceneEntry;
                        }
                        EditorGUILayout.Space();
                        if(GUILayout.Button("Add New Scene Entry"))
                        {
                            stageEntry.sceneEntries.Add(default);
                        }
                        if(GUILayout.Button("Update Meta Data of All Scenes in This Stage"))
                        {
                            var currentOpenScenePath = EditorSceneManager.GetActiveScene().path;
                            for(int j=0;j<stageEntry.sceneEntries.Count;++j)
                            {
                                var sceneEntry = stageEntry.sceneEntries[j];
                                UpdateSceneMetaData(ref sceneEntry);
                                stageEntry.sceneEntries[j] = sceneEntry;
                            }
                            EditorSceneManager.OpenScene(currentOpenScenePath);
                        }
                        EditorGUILayout.EndVertical();

                        if(doDeleteStage)
                        {
                            target.stageEntries.RemoveAt(i);
                            break;
                        }

                    }
                    target.stageEntries[i] = stageEntry;

                    EditorGUILayout.Space();
                }
                EditorGUILayout.Space();
                if(GUILayout.Button("Add New Stage Entry"))
                {
                    if(target.stageEntries == null)
                    {
                        target.stageEntries = new List<StageEntry>();
                    }
                    target.stageEntries.Add(new StageEntry());
                }
                if(GUILayout.Button("Update Meta Data of All Scenes"))
                {
                    var currentOpenScenePath = EditorSceneManager.GetActiveScene().path;
                    for(int i=0;i<target.stageEntries.Count;++i)
                    {
                        var sceneEntries = target.stageEntries[i].sceneEntries;
                        for(int j=0; j<sceneEntries.Count; ++j)
                        {
                            var sceneEntry = sceneEntries[j];
                            UpdateSceneMetaData(ref sceneEntry);
                            sceneEntries[j] = sceneEntry;
                        }
                    }
                    EditorSceneManager.OpenScene(currentOpenScenePath);
                }

                scrpos = scrscope.scrollPosition;
            }
        }

        void UpdateSceneMetaData(ref SceneEntry sceneEntry)
        {
            var sceneName = sceneEntry.scene.name;

            if (sceneEntry.scene == null)
            {
                return;
            }

            string scenePath = string.Empty;
            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                EditorBuildSettingsScene ebsscene = EditorBuildSettings.scenes[i];
                if (ebsscene.path.IndexOf(sceneName) != -1)
                {
                    scenePath = ebsscene.path;

                    break;
                }
            }
            if (string.IsNullOrEmpty(scenePath)){ return; }
            
            Scene sceneStruct = EditorSceneManager.OpenScene(scenePath);
            GameObject[] roots = sceneStruct.GetRootGameObjects();
            int enemyCountValue = 0;
            for (int i=0; i<roots.Length; ++i)
            {
                enemyCountValue += roots[i].GetComponentsInChildren<Enemy>(true).Length;
            }
            sceneEntry.enemyCount = enemyCountValue;

        }
    }
#endif
}
