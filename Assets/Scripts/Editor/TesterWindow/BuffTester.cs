using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Buffs;

public class BuffTester : EditorWindow
{
    private const string targetLabel = "Target";
    private const string buffAffectorLabel = "BuffAffector";
    Actor target;
    BuffAffectorScriptable buffAffector;

    public void OnGUI()
    {
        target = EditorGUILayout.ObjectField(targetLabel, target, typeof(Actor), true) as Actor;
        buffAffector = EditorGUILayout.ObjectField(buffAffectorLabel, buffAffector, typeof(BuffAffectorScriptable), true) as BuffAffectorScriptable;
        
        if(GUILayout.Button("Apply"))
        {
            target.BuffReceiver.Receive(buffAffector);
        }
        EditorGUILayout.Space();
        if(GUILayout.Button("Clear"))
        {
            target = null;
            buffAffector = null;
        }
    }

    [MenuItem("Window/Tester/Buff Tester")]
    static void ShowWindow()
    {
        EditorWindow.GetWindow<BuffTester>();
    }
}
