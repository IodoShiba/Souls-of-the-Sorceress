using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class LayerFieldAttribute : PropertyAttribute
{
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(LayerFieldAttribute))]
public class LayerFieldDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if(property.propertyType==SerializedPropertyType.Integer)
        {
            property.intValue = EditorGUI.LayerField(position,property.name ,property.intValue);
        }
    }
}
#endif
