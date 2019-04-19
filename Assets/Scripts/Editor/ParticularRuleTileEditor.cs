using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngineInternal;

[CustomEditor(typeof(ParticularRuleTile), true)]
[CanEditMultipleObjects]
internal class ParticularRuleTileEditor : RuleTileEditor
{
    RuleTile _baseTile;
    RuleTile baseTile { get => _baseTile == null ? _baseTile = base.target as RuleTile : _baseTile; }
    public override void OnInspectorGUI()
    {
        var tt = target as ParticularRuleTile;
        base.OnInspectorGUI();
        //serializedObject.Update();
        //EditorGUI.BeginChangeCheck();
        EditorGUILayout.Space();
        //using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            GUILayout.Label("Particular Matches");
            if (GUILayout.Button("Update Particular Match List"))
            {
                tt.UpdateParticularMatchList();
            }
            var pmp = serializedObject.FindProperty(nameof(tt.particularMatches));
            var trp = serializedObject.FindProperty(nameof(tt.m_TilingRules));
            for (int i = 0; i < (pmp.isArray ? pmp.arraySize : 0); ++i)
            {
                var p = pmp.GetArrayElementAtIndex(i);
                
                if (!p.FindPropertyRelative("valid").boolValue) continue;
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    EditorGUILayout.LabelField("Element " + i.ToString());
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.PropertyField(p,true); // ???
                        GUILayout.FlexibleSpace();
                        var rect = GUILayoutUtility.GetLastRect();
                        rect.size = new Vector2(48, 48);
                        rect.position = new Vector2(rect.position.x + 96+8, rect.position.y);

                        EditorGUI.DrawRect(new Rect(rect.position.x - 4, rect.position.y - 4, rect.width + 8, rect.height + 8), new Color(.6f,.6f,.6f));
                        if (tt.m_TilingRules[i].m_Sprites[0] == null)
                        {
                            EditorGUI.LabelField(rect, $"{i}");
                        }
                        else
                        {
                            GUI.DrawTexture(rect, AssetPreview.GetAssetPreview(tt.m_TilingRules[i].m_Sprites[0]));
                        }
                    }
                }
            }
        }
        //if (EditorGUI.EndChangeCheck())
        //    serializedObject.ApplyModifiedProperties();
    }
}
