﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class ObjectPatternMapper : TilePatternMapperVector3IntShape//TilePatternMapper
{
    [SerializeField] TileObjectMap map;
    public override bool Match(Tilemap tilemap, Vector3Int targetPos, out Vector3Int findSize)
    {
        GameObject prefab = null;
        if(!map.TryGet(tilemap.GetTile(targetPos), out prefab))
        {
            findSize = default(Vector3Int);
            return false;
        }

        SetChecked((Vector2Int)targetPos);

        findSize = new Vector3Int(1, 1, 1);

        Instantiate(prefab, ShapeToInstantiatePosition(tilemap, targetPos, findSize), Quaternion.identity);

        tilemap.SetTile(targetPos, null);

        return false;
    }
    // public override bool Match(Tilemap tilemap, Vector3Int targetPos, out Vector3Int findSize)
    // {
    //     var originTile = tilemap.GetTile(targetPos);
    //     var oneNextTile = tilemap.GetTile(targetPos + new Vector3Int(1,0,0));
    //     if (originTile == null || (originTile != target && oneNextTile != target))
    //     {
    //         findSize = default(Vector3Int);
    //         return false;
    //     }

    //     int x0 = targetPos.x;
    //     ++targetPos.x;
    //     UnityEngine.Tilemaps.TileBase pointingTile;
    //     while ((pointingTile = tilemap.GetTile(targetPos)) == target)
    //     {
    //         SetChecked((Vector2Int)targetPos);
    //         ++targetPos.x;
    //     }

    //     findSize = new Vector3Int(targetPos.x - x0 + (pointingTile==null?0:1), 1, 1);
    //     return true;
    // }
 
}
