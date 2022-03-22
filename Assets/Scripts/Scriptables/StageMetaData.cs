using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "StageMetaData")]
public class StageMetaData : ScriptableObject
{
    public const string path = "Assets/StageMetaData.asset";

    public enum Stage
    {
        StageEX = -1,
        Stage0 = 1,
        Stage1 = 1,
        Stage2 = 2,
        Stage3 = 3,
        Stage4 = 4,
    }

    [System.Serializable]
    public struct SceneEntry
    {
        [DisabledField] public string scenePath;
        public Stage stageBelongTo;
        [DisabledField] public int enemyCount;
    }

    [SerializeField] List<SceneEntry> sceneEntry = new List<SceneEntry>();

    public bool TryGetEntry(string scenePath, out SceneEntry outSceneEntry)
    {
        IEnumerable<SceneEntry> match = sceneEntry.Where(e => e.scenePath == scenePath);
        if(match.Count() == 0){ outSceneEntry = default; return false; }

        outSceneEntry = match.FirstOrDefault();
        return true;
    }

}
