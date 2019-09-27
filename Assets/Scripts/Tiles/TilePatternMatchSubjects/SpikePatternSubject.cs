using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpikePatternSubject : TilePattermMappingSubjectVector3IntShape
{
    [SerializeField] BoxCollider2D barrierR;
    [SerializeField] BoxCollider2D barrierL;
    public override void Initialize(Tilemap tilemap, Vector3Int position, Vector3Int size)
    {
        float ysize = tilemap.cellSize.y;
        BoxCollider2D bcollider = GetComponent<BoxCollider2D>();

        Vector3 realSize = tilemap.CellToLocal(size);

        float length = System.Math.Max(realSize.x, realSize.y);
        bcollider.size = new Vector2(length ,1) - 0.1f * Vector2.one;
        barrierR.size = barrierL.size = new Vector2(length / 2, 1);
        barrierR.offset = new Vector2(length / 4, 0);
        barrierL.offset = new Vector2(-length / 4, 0);

        if(size.y > size.x)
        {
            transform.rotation *= Quaternion.Euler(0, 0, 90);
        }
    }
}
