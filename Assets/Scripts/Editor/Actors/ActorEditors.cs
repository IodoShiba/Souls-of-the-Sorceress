using UnityEngine;
using UnityEditor;

[UnityEditor.CustomEditor(typeof(ActorSarah.ActorStateConnectorSarah))]
public class ActorStateConnectorSarahEditor : ActorStateConectorEditor
{
    protected override void SetEssentials(SerializedProperty stateProp)
    {
        var p = stateProp.FindPropertyRelative("commands");
        if (p != null)
        {
            p.objectReferenceValue = Target.GetComponent<ActorSarah.PlayerCommander>();
        }
    }
}
