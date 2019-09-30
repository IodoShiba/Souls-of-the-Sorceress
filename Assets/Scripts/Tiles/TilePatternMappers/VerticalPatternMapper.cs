using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class VerticalPatternMapper : TilePatternMapperVector3IntShape
{
    [SerializeField] TileBase target;
    public override bool Match(Tilemap tilemap, Vector3Int targetPos, out Vector3Int findSize)
    {
        if (!(tilemap.GetTile(targetPos) == target))
        {
            findSize = default(Vector3Int);
            return false;
        }

        int y0 = targetPos.y;
        ++targetPos.y;
        while (tilemap.GetTile(targetPos) == target)
        {
            SetChecked((Vector2Int)targetPos);
            ++targetPos.y;
        }

        findSize = new Vector3Int(1, targetPos.y - y0, 1);
        return true;
    }
}
