using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class OnewayPlatformPatternSubject : TilePattermMappingSubjectVector3IntShape
{
    public override void Initialize(Tilemap tilemap, Vector3Int position, Vector3Int size)
    {
        float ysize = tilemap.cellSize.y;

        BoxCollider2D bcollider = GetComponent<BoxCollider2D>();
        //Rect rect = new Rect(tilemap.CellToWorld(position), tilemap.CellToLocal(size));

        float width = tilemap.CellToLocal(size).x;

        transform.position += Vector3.up * (tilemap.cellSize.y / 2);
        bcollider.size = new Vector2(width, ysize);
        bcollider.offset = Vector2.up * (-ysize / 2);

        BoxCollider2D ecollider = transform.GetChild(0).GetComponent<BoxCollider2D>();
        ecollider.size = new Vector2(width, ecollider.size.y);//points = new Vector2[] { new Vector2(-width / 2, 0), new Vector2(width / 2, 0) };
    }
}
