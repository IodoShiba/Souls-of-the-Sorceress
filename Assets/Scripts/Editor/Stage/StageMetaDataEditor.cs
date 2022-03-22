using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEditor;
using System.Linq;
using UnityEngine.SceneManagement;
using System;

[CustomEditor(typeof(StageMetaData))]
public class StageMetaDataEditor : Editor
{
    GameObject[] enemysGameObjects = new GameObject[0];
    Vector2 scrpos = Vector2.zero;
    bool nullObjectContained = false;
    string currentScenePath = "";

    public override async void OnInspectorGUI()
    {   
        currentScenePath = SceneManager.GetActiveScene().path;

        ShowCurrentSceneStatus();

        EditorGUILayout.LabelField("wahh");

        EditorGUILayout.Separator();

        EditorGUILayout.LabelField("Entries");
        var sceneEntry = serializedObject.FindProperty("sceneEntry");
        var entryCount = sceneEntry.arraySize;
        for (int i=0; i<entryCount; ++i)
        {
            var elem = sceneEntry.GetArrayElementAtIndex(i);
            EditorGUILayout.PropertyField(elem, true);
        }

        serializedObject.ApplyModifiedProperties();
    }

    void ShowCurrentSceneStatus()
    {
        if (GUILayout.Button("Search Enemies In Current Scene"))
        {
            enemysGameObjects = Resources.FindObjectsOfTypeAll<GameObject>()
                .Where(obj => obj.scene.IsValid() && obj.TryGetComponent<Enemy>(out _))
                .ToArray();
            nullObjectContained = false;
        }


        EditorGUILayout.LabelField("Current Scene Path: ", currentScenePath);
        EditorGUILayout.LabelField("Enemy counts: ", enemysGameObjects.Length.ToString());
        if(nullObjectContained)
        {
            EditorGUILayout.HelpBox("Null GameObject is contained in GameObject List.\nRetry search.", MessageType.Warning);
        }
        using(new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        using(var scrscope = new EditorGUILayout.ScrollViewScope(scrpos))
        {
            for(int i=0; i<enemysGameObjects.Length; ++i)
            {
                if (enemysGameObjects[i] == null)
                {
                    EditorGUILayout.LabelField("-- null --");
                    continue;
                }

                EditorGUILayout.LabelField(enemysGameObjects[i].name);
            }

            scrpos = scrscope.scrollPosition;
        }

        if(GUILayout.Button("Write This Data"))
        {
            WriteSceneMetaData();
        }
        if(GUILayout.Button("Delete Missing Entry"))
        {
            DeleteMissingEntry();
        }
    }

    void WriteSceneMetaData()
    {
        var sceneEntry = serializedObject.FindProperty("sceneEntry");
        var entryCount = sceneEntry.arraySize;
        for (int i=0; i<entryCount; ++i)
        {
            var elem = sceneEntry.GetArrayElementAtIndex(i);
            if(currentScenePath == elem.FindPropertyRelative("scenePath").stringValue)
            {
                WriteEntryProperty(elem);
                return;
            }
        }
        sceneEntry.InsertArrayElementAtIndex(sceneEntry.arraySize);
        var newelem = sceneEntry.GetArrayElementAtIndex(sceneEntry.arraySize-1);
        WriteEntryProperty(newelem);
    }

    void WriteEntryProperty(SerializedProperty entry)
    {
        entry.FindPropertyRelative("scenePath").stringValue = currentScenePath;
        entry.FindPropertyRelative("enemyCount").intValue = enemysGameObjects.Length;
    }

    async void DeleteMissingEntry()
    {
        var sceneEntry = serializedObject.FindProperty("sceneEntry");
        var entryCount = sceneEntry.arraySize;
        for(int i=sceneEntry.arraySize-1; 0<=i; --i)
        {
            var scenePath = sceneEntry.GetArrayElementAtIndex(i)
                .FindPropertyRelative("scenePath")
                .stringValue;

            scenePath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), scenePath.Replace('/', '\\'));

            if(!System.IO.File.Exists(scenePath))
            {
                sceneEntry.DeleteArrayElementAtIndex(i);
            }
        }
    }
}