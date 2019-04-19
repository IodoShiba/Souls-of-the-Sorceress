using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class CaseRuleTile : UnityEngine.Tilemaps.TileBase
{
    protected abstract TileBase SelectTile(Vector3Int position, ITilemap tilemap);
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        var t = SelectTile(position, tilemap);
        tilemap.GetComponent<Tilemap>().SetTile(position, t);
    }
}
