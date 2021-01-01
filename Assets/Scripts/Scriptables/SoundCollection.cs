using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(menuName="Sounds/SoundColletion")]
public class SoundCollection : ScriptableObject, ISerializationCallbackReceiver
{
    [System.Serializable]
    class Entry
    {
        public string key;
        public AudioClip clip; 
    }

    [SerializeField] Entry[] entries;
    Entry finalRefered;
    Dictionary<string, Entry> ktoe;

    public int entriesCount { get => entries.Length; }

    public AudioClip this[string key]
    {
        get => (finalRefered = ktoe[key]).clip;
    }

    public AudioClip this[int index]
    {
        get => (finalRefered = entries[index]).clip;
    }

    public void OnBeforeSerialize()
    {
    }

    public void OnAfterDeserialize()
    {
        ktoe = entries.ToDictionary(e => e.key);
    }
}