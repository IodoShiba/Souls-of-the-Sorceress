using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DisabledFieldAttribute : PropertyAttribute
{

}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(DisabledFieldAttribute))]
public class DisabledFieldDrawer :PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginDisabledGroup(true);
        EditorGUI.PropertyField(position, property);
        EditorGUI.EndDisabledGroup();
    }
}
#endif