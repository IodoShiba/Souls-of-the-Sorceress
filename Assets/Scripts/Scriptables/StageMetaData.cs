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
    public const string resourcePath = "SotSApplis/Stage Meta Data";

    public enum Stage
    {
        None = -10000,
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
        public string scene;
        [DisabledField] public int enemyCount;
    }

    [System.Serializable]
    public struct StageEntry
    {
        public Stage stage;
        public StageMisc misc;
        public GameResultEvaluator.CriteriaDefeatedCount criteriaDefeatedCount;
        public GameResultEvaluator.CriteriaTimeElapsed criteriaTimeElapsed;
        public List<SceneEntry> sceneEntries;

        public StageEntry(Stage stage = default)
        {
            this.stage = stage;
            misc = null;
            criteriaDefeatedCount = default;
            criteriaTimeElapsed = default;
            sceneEntries = new List<SceneEntry>() {default};
        }
    }

    // [SerializeField] List<SceneEntry> sceneEntry = new List<SceneEntry>();
    [DisabledField, SerializeField] List<StageEntry> stageEntries = new List<StageEntry>();

    public Stage SceneToStage(string scene)
    {
        int idx = stageEntries.FindIndex(stageEntry=>stageEntry.sceneEntries.Any(sceneEntry=>sceneEntry.scene == scene));
        if(idx<0){return Stage.None;}

        return stageEntries[idx].stage;
    }

    public int GetOneStageEnemyCount(Stage stage)
    {
        int idx = stageEntries.FindIndex(se=>se.stage == stage);
        if(idx < 0){ return -1; }

        return stageEntries[idx].sceneEntries.Select(se=>se.enemyCount).Sum();
    }

    public bool GetCriteriaDefeatedCount(Stage stage, out GameResultEvaluator.CriteriaDefeatedCount result)
    {
        int idx = stageEntries.FindIndex(stageEntry=>stageEntry.stage == stage);
        if(idx < 0){result = default; return false;}

        result = stageEntries[idx].criteriaDefeatedCount;
        return true;
    }

    public bool GetCriteriaTimeElapsed(Stage stage, out GameResultEvaluator.CriteriaTimeElapsed result)
    {
        int idx = stageEntries.FindIndex(stageEntry=>stageEntry.stage == stage);
        if(idx < 0){result = default; return false;}

        result = stageEntries[idx].criteriaTimeElapsed;
        return true;
        
    }

    public bool GetStageMisc(Stage stage, out StageMisc result)
    {
        int idx = stageEntries.FindIndex(stageEntry=>stageEntry.stage == stage);
        if(idx < 0){result = default; return false;}

        result = stageEntries[idx].misc;
        return true;
    }

#if UNITY_EDITOR
    public class StageMetaDataWindowBase : EditorWindow
    {
        bool initialized = false;
        StageMetaData target;
        Vector2 scrpos = Vector2.zero;
        List<SceneAsset> sceneAssets = new List<SceneAsset>();

        async void OnEnable()
        {
            if(initialized) {return;}

            var t = AssetDatabase.LoadAssetAtPath(path, typeof(StageMetaData));
            Debug.Assert(t != null);
            target = t as StageMetaData;

            initialized = true;

            sceneAssets = AssetDatabase.FindAssets("t:SceneAsset")
                .Select(guid=>AssetDatabase.GUIDToAssetPath(guid))
                .Select(path=>AssetDatabase.LoadAssetAtPath<SceneAsset>(path))
                .Where(scene=>scene != null)
                .ToList();
        }

        SceneAsset GetSceneAsset(string name)
        {
            if (string.IsNullOrEmpty(name)){return null;}
            int idx = sceneAssets.FindIndex(sc=>sc.name == name);
            return idx >= 0 ? sceneAssets[idx] : null;
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

                        var newEnumStage = (StageMetaData.Stage)EditorGUILayout.EnumPopup("Stage: ", target.stageEntries[i].stage);
                        if(newEnumStage != stageEntry.stage)
                        {
                            stageEntry.stage = newEnumStage;
                            EditorUtility.SetDirty(target);
                        }

                        var newMisc = (StageMisc)EditorGUILayout.ObjectField("Stage Misc", stageEntry.misc, typeof(StageMisc), false);
                        if(newMisc != stageEntry.misc)
                        {
                            stageEntry.misc = newMisc;
                            EditorUtility.SetDirty(target);
                        }

                        ShowCriteriaDefeatedCount(ref stageEntry.criteriaDefeatedCount);
                        EditorGUILayout.Separator();
                        ShowCriteriaTimeElapsed(ref stageEntry.criteriaTimeElapsed);

                        if (stageEntry.sceneEntries == null)
                        {
                            stageEntry.sceneEntries = new List<SceneEntry>();
                        }

                        EditorGUILayout.Space();
                        EditorGUILayout.LabelField("Total Native Enemy", stageEntry.sceneEntries.Select(se=>se.enemyCount).Sum().ToString());

                        EditorGUILayout.Space();
                        EditorGUILayout.LabelField("Scene Belongs");
                        for(int j=0;j<stageEntry.sceneEntries.Count;++j)
                        {
                            var sceneEntry = stageEntry.sceneEntries[j];
                            
                            using(new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                            {
                                var doDeleteScene = GUILayout.Button("x", GUILayout.Width(24));
                                
                                EditorGUILayout.BeginVertical();
                                
                                var sceneAsset = EditorGUILayout.ObjectField("ScenePath", GetSceneAsset(sceneEntry.scene), typeof(SceneAsset), false) as SceneAsset;
                                if(sceneAsset == null)
                                {
                                    if(!string.IsNullOrEmpty(sceneEntry.scene))
                                    {
                                        sceneEntry.scene = string.Empty;
                                        EditorUtility.SetDirty(target);
                                    }
                                }
                                else if(sceneAsset.name != sceneEntry.scene)
                                {
                                    sceneEntry.scene = sceneAsset.name;
                                    EditorUtility.SetDirty(target);
                                }
                                EditorGUILayout.LabelField("Enemy Count: ", sceneEntry.enemyCount.ToString());
                                if (GUILayout.Button("Update Meta Data"))
                                {
                                    var currentOpenScenePath = EditorSceneManager.GetActiveScene().path;
                                    UpdateSceneMetaData(ref sceneEntry);
                                    EditorSceneManager.OpenScene(currentOpenScenePath);
                                    EditorUtility.SetDirty(target);
                                }

                                EditorGUILayout.EndVertical();

                                if(doDeleteScene)
                                {
                                    stageEntry.sceneEntries.RemoveAt(j);
                                    EditorUtility.SetDirty(target);
                                    break;
                                }
                            }

                            stageEntry.sceneEntries[j] = sceneEntry;
                        }
                        EditorGUILayout.Space();
                        if(GUILayout.Button("Add New Scene Entry"))
                        {
                            stageEntry.sceneEntries.Add(default);
                            EditorUtility.SetDirty(target);
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
                            EditorUtility.SetDirty(target);
                        }
                        EditorGUILayout.EndVertical();

                        if(doDeleteStage)
                        {
                            target.stageEntries.RemoveAt(i);
                            EditorUtility.SetDirty(target);
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
                    EditorUtility.SetDirty(target);
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
                    EditorUtility.SetDirty(target);
                }

                scrpos = scrscope.scrollPosition;
            }
        }

        void ShowCriteriaDefeatedCount(ref GameResultEvaluator.CriteriaDefeatedCount criteria)
        {
            EditorGUILayout.LabelField("Criteria (Enemy Defeated Count)");

            IntField("Threshold SS", ref criteria.thresholdSS);
            IntField("Threshold S", ref criteria.thresholdS);
            IntField("Threshold A", ref criteria.thresholdA);
            IntField("Threshold B", ref criteria.thresholdB);

            void IntField(string label, ref int value)
            {
                int newValue = EditorGUILayout.IntField(label, value);
                if (newValue != value)
                {
                    value = newValue;
                    EditorUtility.SetDirty(target);
                }
            }
        }

        void ShowCriteriaTimeElapsed(ref GameResultEvaluator.CriteriaTimeElapsed criteria)
        {
            EditorGUILayout.LabelField("Criteria (Time Elapsed)");

            FloatField("Threshold SS", ref criteria.thresholdSS);
            FloatField("Threshold S", ref criteria.thresholdS);
            FloatField("Threshold A", ref criteria.thresholdA);
            FloatField("Threshold B", ref criteria.thresholdB);

            void FloatField(string label, ref float value)
            {
                float newValue = EditorGUILayout.FloatField(label, value);
                if (newValue != value)
                {
                    value = newValue;
                    EditorUtility.SetDirty(target);
                }
            }
        }

        void UpdateSceneMetaData(ref SceneEntry sceneEntry)
        {
            var sceneName = sceneEntry.scene;

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
