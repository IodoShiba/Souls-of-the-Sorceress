using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MassTileColiderPatternSubject : TilePattermMappingSubjectRegionPathsIntShape
{
    public override void Initialize(Tilemap tilemap, Vector3Int position, RegionPathsInt shape)
    {
        var pc = GetComponent<PolygonCollider2D>();
        pc.pathCount = 1 + shape.innerHoles.Count;
        SetPath(0, tilemap, pc, shape.outer);
        for (int i = 0; i < shape.innerHoles.Count; ++i) 
        {
            SetPath(i + 1, tilemap, pc, shape.innerHoles[i]);
        }
    }

    void SetPath(int index,Tilemap tilemap,PolygonCollider2D collider,List<Vector3Int> pointsRaw)
    {
        Vector2[] points = new Vector2[pointsRaw.Count];
        for (int i = 0; i < pointsRaw.Count; ++i)
        {
            points[i] = tilemap.CellToWorld(pointsRaw[i]) - transform.position;
        }
        collider.SetPath(index, points);
    }
}
