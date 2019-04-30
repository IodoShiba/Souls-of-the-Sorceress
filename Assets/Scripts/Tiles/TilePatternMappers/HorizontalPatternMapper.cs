using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class HorizontalPatternMapper : TilePatternMapper
{
    [SerializeField] TileBase target;
    public override bool Match(Tilemap tilemap, Vector3Int targetPos, out Vector3Int findSize)
    {
        if (!(tilemap.GetTile(targetPos) == target && tilemap.GetTile(targetPos + new Vector3Int(-1, 0, 0)) != target))
        {
            findSize = default(Vector3Int);
            return false;
        }

        int x0 = targetPos.x;
        do
        {
            ++targetPos.x;
        }
        while (tilemap.GetTile(targetPos) == target);

        findSize = new Vector3Int(targetPos.x - x0, 1, 1);
        return true;
    }
}
