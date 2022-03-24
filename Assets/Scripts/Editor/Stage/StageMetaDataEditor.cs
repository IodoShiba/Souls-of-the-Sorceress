using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditor;


[CustomEditor(typeof(StageMetaData))]
public class StageMetaDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if(GUILayout.Button("Open Editor Window"))
        {
            EditorWindow.GetWindow<StageMetaDataWindow>("Stage Meta Data Window");
        }
        EditorGUILayout.HelpBox("Due to containing process across scenes,\nediting must be done in special window \nwhich is scene independent workspace.", MessageType.Info);

        EditorGUILayout.Space();
        
        EditorGUI.BeginDisabledGroup(true);
        base.OnInspectorGUI();
        EditorGUI.BeginDisabledGroup(false);
    }
}
// [CustomEditor(typeof(StageMetaData))]
// public class StageMetaDataEditor : Editor
// {
//     ScriptableObject stageMetaData;

//     public override void OnInspectorGUI()
//     {   
//         var currentScenePath = SceneManager.GetActiveScene().path;

//         // ShowCurrentSceneStatus();

//         EditorGUILayout.LabelField("wahh");

//         EditorGUILayout.Separator();

//         stageMetaData = EditorGUILayout.ObjectField(stageMetaData, typeof(StageMetaData), false) as StageMetaData;

//         EditorGUILayout.LabelField("Stage Entries");
//         var stageEntries = serializedObject.FindProperty("stageEntries");
//         var stageEntriesCount = stageEntries.arraySize;
//         for (int i=0; i<stageEntriesCount; ++i)
//         {
//             var stageElem = stageEntries.GetArrayElementAtIndex(i);
            
//             using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
//             {
//                 var doDeleteStage = GUILayout.Button("x", GUILayout.Width(24));
                
//                 EditorGUILayout.BeginVertical();
//                 var stage = stageElem.FindPropertyRelative("stage");

//                 StageMetaData.Stage e = (StageMetaData.Stage)EditorGUILayout.EnumPopup((StageMetaData.Stage)stage.intValue);
//                 stage.intValue = (int)e;
                

//                 var sceneEntries = stageElem.FindPropertyRelative("sceneEntries");
//                 var sceneEntriesCount = sceneEntries.arraySize;
//                 for(int j=0;j<sceneEntriesCount;++j)
//                 {
//                     var sceneElem = sceneEntries.GetArrayElementAtIndex(j);
                    
//                     using(new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
//                     {
//                         var doDeleteScene = GUILayout.Button("x", GUILayout.Width(24));
                        
//                         EditorGUILayout.BeginVertical();
//                         EditorGUILayout.PropertyField(sceneElem, true);
//                         if (GUILayout.Button("Update Meta Data"))
//                         {
//                             var currentOpenScenePath = EditorSceneManager.GetActiveScene().path;
//                             UpdateSceneMetaData(sceneElem);
//                             EditorSceneManager.OpenScene(currentOpenScenePath);

//                         }

//                         EditorGUILayout.EndVertical();

//                         if(doDeleteScene)
//                         {
//                             sceneEntries.DeleteArrayElementAtIndex(j);
//                             break;
//                         }
//                     }
//                 }
//                 if(GUILayout.Button("Add New Scene Entry"))
//                 {
//                     sceneEntries.InsertArrayElementAtIndex(sceneEntriesCount);
//                 }
//                 if(GUILayout.Button("Update Meta Data of All Scenes in This Stage"))
//                 {
//                     var currentOpenScenePath = EditorSceneManager.GetActiveScene().path;
//                     for(int j=0;j<sceneEntriesCount;++j)
//                     {
//                         var sceneElem = sceneEntries.GetArrayElementAtIndex(j);
//                         UpdateSceneMetaData(sceneElem);
//                     }
//                     EditorSceneManager.OpenScene(currentOpenScenePath);
//                 }
//                 EditorGUILayout.EndVertical();

//                 if(doDeleteStage)
//                 {
//                     stageEntries.DeleteArrayElementAtIndex(i);
//                     break;
//                 }
//             }
//         }
//         if(GUILayout.Button("Add New Stage Entry"))
//         {
//             stageEntries.InsertArrayElementAtIndex(stageEntriesCount);
//         }
//         if(GUILayout.Button("Update Meta Data of All Scenes"))
//         {
//             var currentOpenScenePath = EditorSceneManager.GetActiveScene().path;
//             for(int i=0;i<stageEntriesCount;++i)
//             {
//                 var sceneEntries = stageEntries.GetArrayElementAtIndex(i).FindPropertyRelative("SceneEntries");
//                 var sceneEntriesCount = sceneEntries.arraySize;
//                 for(int j=0; j<sceneEntriesCount; ++j)
//                 {
//                     var sceneElem = sceneEntries.GetArrayElementAtIndex(j);
//                     UpdateSceneMetaData(sceneElem);
//                 }
//             }
//             EditorSceneManager.OpenScene(currentOpenScenePath);
//         }

//         serializedObject.ApplyModifiedProperties();
//     }

//     void UpdateSceneMetaData(SerializedProperty sceneEntryProp)
//     {
//         var scene = sceneEntryProp.FindPropertyRelative("scene");
//         var sceneName = scene.FindPropertyRelative("m_SceneName").stringValue;
//         var enemyCount = sceneEntryProp.FindPropertyRelative("enemyCount");

//         if (string.IsNullOrEmpty(sceneName))
//         {
//             return;
//         }

//         string scenePath = string.Empty;
//         for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
//         {
//             EditorBuildSettingsScene ebsscene = EditorBuildSettings.scenes[i];
//             if (ebsscene.path.IndexOf(sceneName) != -1)
//             {
//                 scenePath = ebsscene.path;

//                 break;
//             }
//         }
//         if (string.IsNullOrEmpty(scenePath)){ return; }
        
//         Scene sceneStruct = EditorSceneManager.OpenScene(scenePath);
//         GameObject[] roots = sceneStruct.GetRootGameObjects();
//         int enemyCountValue = 0;
//         for (int i=0; i<roots.Length; ++i)
//         {
//             enemyCountValue += roots[i].GetComponentsInChildren<Enemy>(true).Length;
//         }
//         enemyCount.intValue = enemyCountValue;

//     }

//     void DeleteMissingEntry()
//     {
//         var sceneEntry = serializedObject.FindProperty("sceneEntry");
//         var entryCount = sceneEntry.arraySize;
//         for(int i=sceneEntry.arraySize-1; 0<=i; --i)
//         {
//             var scenePath = sceneEntry.GetArrayElementAtIndex(i)
//                 .FindPropertyRelative("scenePath")
//                 .stringValue;

//             scenePath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), scenePath.Replace('/', '\\'));

//             if(!System.IO.File.Exists(scenePath))
//             {
//                 sceneEntry.DeleteArrayElementAtIndex(i);
//             }
//         }
//     }
// }