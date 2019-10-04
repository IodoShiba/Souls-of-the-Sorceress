using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObject/SoundManager")]
public class SoundManagerScriptable : SoundManager.Scriptable, ISerializationCallbackReceiver
{
    public void OnBeforeSerialize()
    {

    }

    public void OnAfterDeserialize()
    {
        sPrefab = prefab;
    }
}
