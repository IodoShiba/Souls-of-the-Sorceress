using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FlagFieldAttribute : PropertyAttribute
{
    
    System.Type enumType;
    public FlagFieldAttribute(System.Type enumType) 
    {
        this.enumType = enumType; 
    }

    public System.Type EnumType { get => enumType; }
}