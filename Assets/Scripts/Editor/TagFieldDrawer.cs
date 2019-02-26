using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;


[UnityEditor.CustomPropertyDrawer(typeof(TagFieldAttribute))]
public class TagFieldDrawer : UnityEditor.PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        TagFieldAttribute theAttribute = attribute as TagFieldAttribute;
        if (property.propertyType == SerializedPropertyType.String)
        {
            property.stringValue = EditorGUI.TagField(position, label, property.stringValue);
        }
    }
}
#endif