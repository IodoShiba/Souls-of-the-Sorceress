using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


[System.Serializable]
public class MassPatternMapper : TilePatternMapperVector3IntListShape,ISerializationCallbackReceiver
{
    [SerializeField] List<TileBase> targets;
    HashSet<TileBase> hs_targets = new HashSet<TileBase>();
    Vector3 cpos;
    public override bool Match(Tilemap tilemap, Vector3Int targetPos, out List<Vector3Int> vertexes)
    {
        if (hs_targets.Contains(tilemap.GetTile(targetPos)))// == target && tilemap.GetTile(targetPos + new Vector3Int(-1, 0, 0)) != target))
        {
            vertexes = default(List<Vector3Int>);
            Debug.Log("Detect:" + tilemap.GetTile(targetPos).ToString() + targetPos.ToString());
            return false;
        }

        vertexes = default(List<Vector3Int>);
        return false;
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        hs_targets.Clear();
        if (targets == null || targets.Count == 0) { return; }
        foreach(var e in targets)
        {
            hs_targets.Add(e);
        }
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize() { }
}

public abstract class TilePattermMappingSubjectListVector3IntShape : TilePatternMappingSubject<List<Vector3Int>> { }
public abstract class TilePatternMapperVector3IntListShape : TilePatternMapper<List<Vector3Int>, TilePattermMappingSubjectListVector3IntShape>
{
    public override Vector3 ShapeToInstantiatePosition(Tilemap tilemap, Vector3Int position, List<Vector3Int> vs) => tilemap.CellToWorld(position);
}