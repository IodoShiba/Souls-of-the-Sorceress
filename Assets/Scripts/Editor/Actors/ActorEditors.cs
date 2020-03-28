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

[UnityEditor.CustomEditor(typeof(ActorWalkingMushroom.AscWalkingMushroom))]
public class AscWalkingMashroomEditor : ActorStateConectorEditor
{
}

[UnityEditor.CustomEditor(typeof(ActorEnemyElement.AscEnemyElement))]
public class AscEnemyElementEditor : ActorStateConectorEditor
{
}

[UnityEditor.CustomEditor(typeof(ActorHugeMashroom.AscHugeMush))]
public class AscHugeMashEditor : ActorStateConectorEditor
{
}

[UnityEditor.CustomEditor(typeof(ActorSlime.AscSlime))]
public class AscSlimeEditor : ActorStateConectorEditor
{
}

[UnityEditor.CustomEditor(typeof(ActorBossTitan.AscBossTitan))]
public class AscBossTitanEditor : ActorStateConectorEditor
{
}

[UnityEditor.CustomEditor(typeof(ActorBomb.AscBomb))]
public class AscBombEditor : ActorStateConectorEditor
{
}