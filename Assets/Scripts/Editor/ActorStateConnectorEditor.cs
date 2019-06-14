using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;
using static ActorState;

[CustomEditor(typeof(ActorStateConnector))]
public class ActorStateConectorEditor : Editor
{
    ActorStateConnector _target;
    ActorStateConnector Target { get => _target == null ? (_target = (target as ActorStateConnector)) : _target; }

    public override void OnInspectorGUI()
    {
        IEnumerable<FieldInfo> fields = target.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
        //IEnumerable<FieldInfo> actorStateFields = fields.Where(fi => typeof(ActorState).IsAssignableFrom(fi.FieldType));
        IEnumerable<IGrouping<bool,FieldInfo>> fieldg = fields.GroupBy(fi => typeof(ActorState).IsAssignableFrom(fi.FieldType));

        foreach(FieldInfo fi in fieldg.Where(g => !g.Key).First())
        {
            SerializedProperty p = serializedObject.FindProperty(fi.Name);
            if (p != null)
            {
                EditorGUILayout.PropertyField(p, true);
            }
        }

        Color defcol = GUI.backgroundColor;
        //foreach (FieldInfo fi in actorStateFields)
        foreach (FieldInfo fi in fieldg.Where(g => g.Key).First())
        {
            bool isDefaultState = fi.GetValue(Target) == Target.DefaultState;
            SerializedProperty p = serializedObject.FindProperty(fi.Name);
            if (p == null) { continue; }

            if (isDefaultState) { GUI.backgroundColor = Color.gray; }
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                if (isDefaultState) { EditorGUILayout.LabelField("< Default State >", new GUIStyle{fontStyle = FontStyle.Bold}); }
                GUI.backgroundColor = defcol;
                EditorGUILayout.PropertyField(p, true);
            }
            GUI.backgroundColor = defcol;
        }
        if (GUILayout.Button("Set Game Object and Representer"))
        {
            foreach (FieldInfo fi in fieldg.Where(g => g.Key).First())
            {
                var go = serializedObject.FindProperty(fi.Name).FindPropertyRelative("gameObject");
                go.objectReferenceValue = Target.gameObject;
                var ar = serializedObject.FindProperty(fi.Name).FindPropertyRelative("connector");
                ar.objectReferenceValue = Target;
                serializedObject.ApplyModifiedProperties();
            }
            EditorUtility.SetDirty(Target);
            serializedObject.ApplyModifiedProperties();
        }
        serializedObject.ApplyModifiedProperties();
    }

}



