using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[UnityEditor.CustomPropertyDrawer(typeof(FlagFieldAttribute))]
public class FlagFieldDrawer : UnityEditor.PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        FlagFieldAttribute theAttribute = attribute as FlagFieldAttribute;
        if (property.propertyType == SerializedPropertyType.Enum && theAttribute.EnumType.GetEnumUnderlyingType() == typeof(System.Int32))
        {
            property.intValue = System.Convert.ToInt32(EditorGUI.EnumFlagsField(position, label, (System.Enum)System.Enum.ToObject(theAttribute.EnumType, property.intValue)));
        }
    }
}
