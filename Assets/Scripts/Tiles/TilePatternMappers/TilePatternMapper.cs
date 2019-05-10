using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

// abstract class TilePatternMapper
//
// Tilemapに配置されたタイルを調べ、特定のパターンでタイルが配置されている場所を検出し、
// TileStructureMappingSubject抽象コンポーネントが取り付けられたPrefabをInstantiateし、
// TilePatternMappingSubject.Initialize()で初期化する
// TilePatternMappingSubject と併せて使う
public abstract class TilePatternMapper/*<MatchDataType,SpecifiedSubjectType>*/ : MonoBehaviour //where SpecifiedSubjectType : List<MatchDataType>//これをジェネリックにしよう
{
    public abstract bool Match(Tilemap tilemap, Vector3Int targetPos, out Vector3Int findSize);

    [SerializeField] TilePatternMappingSubject subjectPrefab;
    Tilemap tilemap;

    private List<(Vector3Int position, Vector3Int size)> Search()
    {
        BoundsInt b = tilemap.cellBounds;
        Debug.Log(b);
        Debug.Log(b.yMin);
        Vector3Int cpos = Vector3Int.zero;
        Vector3Int findSize;
        List<(Vector3Int position,Vector3Int size)> finds = new List<(Vector3Int, Vector3Int)>(16);//ジェネリックにしてここを可変にする
        for (cpos.y = b.yMin; cpos.y < b.yMax; ++cpos.y)
        {
            for (cpos.x = b.xMin; cpos.x < b.xMax; ++cpos.x)
            {
                if(Match(tilemap, cpos, out findSize))
                {
                    Debug.Log(cpos);
                    finds.Add((cpos, findSize));
                }
            }
        }
        return finds;
    }

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        List<(Vector3Int position, Vector3Int size)> matchList = Search();
        for (int i = 0; i < matchList.Count; ++i)
        {
            TilePatternMappingSubject tpms =
                Instantiate(
                    subjectPrefab,
                    tilemap.CellToWorld(matchList[i].position) + tilemap.CellToLocal(matchList[i].size) / 2,//ここを仮想関数化しよう
                    Quaternion.identity);
            tpms.Initialize(tilemap, matchList[i].position, matchList[i].size);
        }
    }
}


public abstract class TilePatternMappingSubject/*<MatchDataType>*/ : MonoBehaviour//これをジェネリックにしよう
{
    public abstract void Initialize(Tilemap tilemap, Vector3Int position, Vector3Int size);
}