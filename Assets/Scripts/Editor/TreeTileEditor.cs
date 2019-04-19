using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngineInternal;

[CustomEditor(typeof(TreeTile), true)]
[CanEditMultipleObjects]
internal class TreeTileEditor : ParticularRuleTileEditor
{
    public override void OnInspectorGUI()
    {
        if(GUILayout.Button("Reset Tiling Rules"))
        {
            ((TreeTile)target).ResetTilingRules();
        }
        base.OnInspectorGUI();
    }
}
