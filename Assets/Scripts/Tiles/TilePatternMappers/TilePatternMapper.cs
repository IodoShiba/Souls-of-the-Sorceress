using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using System;

// abstract class TilePatternMapper
//
// Tilemapに配置されたタイルを調べ、特定のパターンでタイルが配置されている場所を検出し、
// TileStructureMappingSubject抽象コンポーネントが取り付けられたPrefabをInstantiateし、
// TilePatternMappingSubject.Initialize()で初期化する
// TilePatternMappingSubject と併せて使う
public abstract class TilePatternMapper<ShapeExpressionType,SpecifiedSubjectType> : MonoBehaviour where SpecifiedSubjectType : TilePatternMappingSubject<ShapeExpressionType>
{
    public abstract bool Match(Tilemap tilemap, Vector3Int targetPos, out ShapeExpressionType findSize);
    public abstract Vector3 ShapeToInstantiatePosition(Tilemap tilemap ,Vector3Int position,ShapeExpressionType shape);

    [SerializeField] SpecifiedSubjectType subjectPrefab;
    Tilemap tilemap;
    private bool[,] checkedFlags;
    private Action<Vector2Int> setChecked;//(Vector2 position) {checkedFlags[position.y - ][position.x] }
    private Func<Vector2Int, bool> getChecked;

    public bool GetChecked(in Vector2Int position) => getChecked(position);
    protected void SetChecked(in Vector2Int position) => setChecked(position);


    private List<(Vector3Int position, ShapeExpressionType shape)> Search()
    {
        BoundsInt b = tilemap.cellBounds;
        //Debug.Log(b);
        //Debug.Log(b.yMin);
        checkedFlags = new bool[b.size.y, b.size.x];

        for(int y = 0; y<b.size.y;++y)
            for(int x = 0; x < b.size.x; ++x)
            {
                checkedFlags[y, x] = false;
            }

        setChecked = (Vector2Int position) => 
        {
            if (b.yMin <= position.y && position.y < b.yMax && b.xMin <= position.x && position.x < b.xMax)
            {
                checkedFlags[position.y - b.yMin, position.x - b.xMin] = true;
            }
        };
        getChecked = (Vector2Int position) => checkedFlags[position.y - b.yMin, position.x - b.xMin];

        Vector3Int cpos = Vector3Int.zero;
        ShapeExpressionType findShape;

        List<(Vector3Int position, ShapeExpressionType size)> finds = new List<(Vector3Int, ShapeExpressionType)>(16);
        for (cpos.y = b.yMin; cpos.y < b.yMax; ++cpos.y)
        {
            for (cpos.x = b.xMin; cpos.x < b.xMax; ++cpos.x)
            {
                if (!GetChecked((Vector2Int)cpos) && Match(tilemap, cpos, out findShape)) 
                {
                    finds.Add((cpos, findShape));
                }
                SetChecked((Vector2Int)cpos);
            }
        }
        return finds;
    }

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        List<(Vector3Int position, ShapeExpressionType shape)> matchList = Search();
        if (subjectPrefab != null)
        {
            for (int i = 0; i < matchList.Count; ++i)
            {
                SpecifiedSubjectType tpms =
                    Instantiate(
                        subjectPrefab,
                        ShapeToInstantiatePosition(tilemap, matchList[i].position, matchList[i].shape),
                        Quaternion.identity);
                tpms.Initialize(tilemap, matchList[i].position, matchList[i].shape);
            }
        }
    }
}


public abstract class TilePatternMappingSubject<ShapeExpressionType> : MonoBehaviour
{
    public abstract void Initialize(Tilemap tilemap, Vector3Int position, ShapeExpressionType shape);
}

public abstract class TilePattermMappingSubjectVector3IntShape : TilePatternMappingSubject<Vector3Int> { }
public abstract class TilePatternMapperVector3IntShape : TilePatternMapper<Vector3Int, TilePattermMappingSubjectVector3IntShape>
{
    public override Vector3 ShapeToInstantiatePosition(Tilemap tilemap, Vector3Int position, Vector3Int size) => tilemap.CellToWorld(position) + tilemap.CellToLocal(size) / 2;
}