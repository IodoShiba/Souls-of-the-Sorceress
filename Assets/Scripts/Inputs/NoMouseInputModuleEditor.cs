using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Inputs
{
    [CustomEditor(typeof(NoMouseInputModule))]
    public class NoMouseInputModuleEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}