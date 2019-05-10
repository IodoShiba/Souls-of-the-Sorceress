﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class HorizontalPatternMapper : TilePatternMapperVector3IntShape//TilePatternMapper
{
    [SerializeField] TileBase target;
    public override bool Match(Tilemap tilemap, Vector3Int targetPos, out Vector3Int findSize)
    {
        if (!(tilemap.GetTile(targetPos) == target))// == target && tilemap.GetTile(targetPos + new Vector3Int(-1, 0, 0)) != target))
        {
            findSize = default(Vector3Int);
            return false;
        }

        int x0 = targetPos.x;
        ++targetPos.x;
        while (tilemap.GetTile(targetPos) == target)
        {
            SetChecked((Vector2Int)targetPos);
            ++targetPos.x;
        }

        findSize = new Vector3Int(targetPos.x - x0, 1, 1);
        return true;
    }
}
