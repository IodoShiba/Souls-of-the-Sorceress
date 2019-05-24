using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActorFunction<FieldSetType,MethodType> where FieldSetType : ActorFunctionFieldsSet where MethodType : ActorFunctionMethod<FieldSetType>
{
    [SerializeField] MethodType method;
    [SerializeField] FieldSetType fields;

    public void Use() { method.Use(fields); }
}
public class ActorFunctionFieldsSet
{
}
public abstract class ActorFunctionMethod<FieldSetType> : MonoBehaviour where FieldSetType : ActorFunctionFieldsSet
{ 
    public abstract void Use(FieldSetType fields);
}
