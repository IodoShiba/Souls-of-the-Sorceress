using UnityEngine;
using UnityEditor;

[UnityEditor.CustomEditor(typeof(ActorSarah.ActorStateConnectorSarah))]
public class ActorStateConnectorSarahEditor : ActorStateConectorEditor
{
    protected override void SetEssentials(SerializedProperty stateProp)
    {
        stateProp.FindPropertyRelative("commands").objectReferenceValue = Target.GetComponent<ActorSarah.PlayerCommander>();
    }
}
