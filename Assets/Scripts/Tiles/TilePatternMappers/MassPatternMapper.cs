using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static System.Math;
using System.Linq;//どうなのか

[System.Serializable]
public class MassPatternMapper : TilePatternMapperRegionPathsIntShape,ISerializationCallbackReceiver
{

    [SerializeField] List<TileBase> targets;
    HashSet<TileBase> hs_targets = new HashSet<TileBase>();
    Tilemap _tilemap;
    Dictionary<List<Vector3Int>, RegionPathsInt> outForRegionPaths = new Dictionary<List<Vector3Int>, RegionPathsInt>();
    List<List<Vector3Int>> unresolvedInners = new List<List<Vector3Int>>();

    protected override void ReleaseAtEndOfSearch()
    {
        Debug.Log(unresolvedInners.Count);
        outForRegionPaths = null;
        unresolvedInners = null;
    }
    bool Check(Vector3Int pos) { return hs_targets.Contains(_tilemap.GetTile(pos)); }
    public override bool Match(Tilemap tilemap, Vector3Int targetPos, out RegionPathsInt paths)
    {
        //意図
        //  [][][][][][]
        //  [][][][][][]
        //  [][][][]
        //  [][][][]
        //  この領域に対して
        //
        //  XX          XX
        //  [][][][][][]
        //  [][][][]XX[]XX
        //  [][][][]
        //  XX[][][]XX
        //  XXのマスのマップ座標のListを作りたい
        //
        //  境界部分に十字になるような部分を含む領域はその交差点で２つの領域に分割できるので検出する領域は境界に十字を含まないとする

        // Memo
        // 領域は開領域（境界を含まない）　よって十字クロスに見える場所で二分される
        // 斜め45度の坂道を考えてこれを拡張したいが、坂を考えると縦横のみで成り立っていた信念が成り立たなくなる
        // ・一つの格子点に複数の「領域の左下角」が存在しうる
        //
        //    :\###|---/#
        //    :  \#| /###
        //    :----X====:    <-このようなことがありうる
        //    :    :    :
        //    :----:----:
        //
        // 分かること
        // ・各格子点に接続する頂点はせいぜい8つ　各象限に1本ずつまでと、軸方向のみ

        this._tilemap = tilemap;
        List<Vector3Int> detectedPath;

        paths = null;

        bool isTargetInTargetPos = Check(targetPos);
        if (!(isTargetInTargetPos != Check(targetPos + Vector3Int.left) && isTargetInTargetPos != Check(targetPos + Vector3Int.down))) //右下角か？//右下角だけ調べる
        {
            paths = default;
            return false;
        }

        //if((Vector2Int)targetPos==new Vector2Int(0, -3)) { Debug.Log("at 0 -3"); }
        if (isTargetInTargetPos)//境界の走査を行う
        {
            detectedPath = DetectAxesAlongEdge(targetPos, Vector3Int.up,
                v => !Check(v), Check,
                null, null
                );
        }
        else
        {
            detectedPath = DetectAxesAlongEdge(targetPos, Vector3Int.right,
                null, v => !Check(v),
                null, Check
                );
        }
        //var s = "";
        //if ((Vector2Int)targetPos == new Vector2Int(0, -3)) {
        //    foreach(var v in detectedPath) {s += v.ToString(); } Debug.Log(s);
        //}
        if (detectedPath == null)
        {
            paths = null;
            return false;
        }

        List<Vector3Int> lowerLeftCorners = FindLowerLeftCorners(detectedPath);//始点以外の右下角にフラグを立てて二度と探索しないようにする
        lowerLeftCorners.ForEach(
            c => {
                if (detectedPath[0] != c) SetChecked((Vector2Int)c);
            }
            );

        //取得した境界がタイルの塊の一番外側の境界(外縁)なのか調べる
        int edgeRotationMass = 0;
        int cnt = detectedPath.Count;
        Vector3Int prevEdge = (detectedPath[0] - detectedPath[cnt - 1]);
        for (int i = 0; i < cnt; ++i)
        {
            Vector3Int nextEdge = detectedPath[(i + 1) % cnt] - detectedPath[i];
            edgeRotationMass += 90 * (Sign(prevEdge.x) * Sign(nextEdge.y) - Sign(prevEdge.y) * Sign(nextEdge.x));//領域の角の回転角を計算する　右回りなら+=90度　左回りなら-=90度

            prevEdge = nextEdge;
        }
        //Debug.Log($"{targetPos}|rot:{edgeRotationMass}");
        switch (edgeRotationMass)
        {
            case 360://外縁ではなかった　内縁
                RegionPathsInt answer = outForRegionPaths.Values.FirstOrDefault(rp => IsInnerBelongsTo(detectedPath, rp.outer));
                if (answer != null)
                {
                    SolveInnerPath(detectedPath, answer);
                }
                else
                {
                    unresolvedInners.Add(detectedPath);
                }
                return false;

            case -360://外縁であった
                var pathsForFoundOuter = new RegionPathsInt();
                pathsForFoundOuter.outer = detectedPath;
                outForRegionPaths[detectedPath] = pathsForFoundOuter;
                TryResolveInnerBelonging(pathsForFoundOuter);
                if (detectedPath[0] == targetPos)
                {
                    paths = pathsForFoundOuter;
                }
                break;

            default://走査する領域はその境界が交わらない領域であるため回転角の和は+-360度以外はありえない
                throw new System.InvalidProgramException($"{nameof(edgeRotationMass)} cant be {edgeRotationMass} : it must be +-360. Something wrong is in the program.");
            
        }

        if((Vector2Int)targetPos == (Vector2Int)tilemap.cellBounds.max-Vector2Int.one)//最後のチェックである
        {
            //outForRegionPaths = null;
            Debug.Assert(unresolvedInners.Count == 0);
        }

        return paths != null;

        //領域の境界を検出し、頂点リストに追加するローカル関数
        List<Vector3Int> DetectAxesAlongEdge(
            Vector3Int _startPos,
            Vector3Int _direction,
            System.Func<Vector3Int, bool> _f00,System.Func<Vector3Int, bool> _f01,
            System.Func<Vector3Int, bool> _f10,System.Func<Vector3Int, bool> _f11
            )
        {//焦点が領域の内外を行ったり来たりするのが問題
            return Imple(_startPos, _direction, Check(_startPos), _f00, _f01, _f10, _f11);

            List<Vector3Int> Imple(
            Vector3Int startPos,
            Vector3Int direction,
            bool isInMass,
            System.Func<Vector3Int, bool> f00, System.Func<Vector3Int, bool> f01,
            System.Func<Vector3Int, bool> f10, System.Func<Vector3Int, bool> f11
            )
            {
                int edgeLength = 0;
                Vector3Int currentPos = startPos;
                bool b00 = f00 == null || f00(currentPos + Vector3Int.left);
                bool b01 = f01 == null || f01(currentPos);
                bool b10 = f10 == null || f10(currentPos + new Vector3Int(-1, -1, 0));
                bool b11 = f11 == null || f11(currentPos + Vector3Int.down);
                while (b00 && b01 && b10 && b11) 
                {
                    ++edgeLength;
                    currentPos += direction;

                    b00 = f00 == null || f00(currentPos + Vector3Int.left);
                    b01 = f01 == null || f01(currentPos);
                    b10 = f10 == null || f10(currentPos + new Vector3Int(-1, -1, 0));
                    b11 = f11 == null || f11(currentPos + Vector3Int.down);
                }

                if (currentPos == _startPos) //末端に来た
                {
                    var retl = new List<Vector3Int>();
                    retl.Add(currentPos);
                    retl.Add(startPos);
                    return retl;
                }
                else if (edgeLength == 0 || (direction == Vector3Int.left && GetChecked((Vector2Int)currentPos))) //その方向に有効な辺はなかった
                {
                    return null;
                }



                List<Vector3Int> later;
                if (Check(
                    new Vector3Int(
                        currentPos.x + (direction.x + direction.y - 1) / 2,
                        currentPos.y + (direction.y - direction.x - 1) / 2,
                        currentPos.z)
                        )//くぼみか？
                    )
                {
                    later = Imple(currentPos, new Vector3Int(-direction.y, direction.x, 0), isInMass,
                                f01, f11,
                                f00, f10
                                ); //左回転
                }
                else
                {
                    if (isInMass && direction == Vector3Int.down)
                    {
                        SetChecked((Vector2Int)currentPos);
                    }
                    later = Imple(currentPos, new Vector3Int(direction.y, -direction.x, 0), isInMass,
                            f10, f00,
                            f11, f01
                            ); //右回転
                }
                if(later != null)
                {
                    later.Add(startPos);
                    if (later[0] == startPos)
                    {
                        later.Reverse();
                        later.RemoveAt(later.Count - 1);
                    }
                }
                return later;
            }
            
        }

    }

    List<Vector3Int> FindLowerLeftCorners(List<Vector3Int> path)
    {
        List<Vector3Int> detectedLowerLeftCorners = new List<Vector3Int>();
        int cnt = path.Count;
        for (int i = 0; i < cnt; ++i)
        {
            if ((path[i] - path[(i + cnt - 1) % cnt]).x < 0 && (path[(i + 1) % cnt] - path[i]).y>0)//(prevEdge.x < 0 && nextEdge.y > 0)
            {
                detectedLowerLeftCorners.Add(path[i]); //左下角ならば集合に追加
            }
        }
        return detectedLowerLeftCorners;
    }

    void TryResolveInnerBelonging(RegionPathsInt regionPathsInt)
    {
        for(int i = unresolvedInners.Count-1; i>=0;--i)
        {
            if(IsInnerBelongsTo(unresolvedInners[i], regionPathsInt.outer))
            {
                SolveUnresolvedInnerPath(i, regionPathsInt);
                continue;
            }
        }
    }
    bool IsInnerBelongsTo(List<Vector3Int> innerPath, List<Vector3Int> path)
        => IsPointInPath(innerPath[0], path);

    bool IsPointInPath(Vector3Int pos,List<Vector3Int> path)
    {
        double radchange = 0;
        int cnt = path.Count;
        Vector3Int vi = path[0] - pos;
        var ci = new System.Numerics.Complex(vi.x,vi.y);
        for (int i = 0; i < cnt*2; ++i)
        {
            //領域の境界は交差しない閉じた曲線で、かつ領域の境界同士は交わらないため、
            //回転数の計算により点が領域の中にあるか外にあるか調べる　自然数周して回転数が0ならば点が外にあるといえる
            //念の為ちょうど２周調べる　小数点誤差は無視できるほどと期待する
            
            Vector3Int vip1 = path[(i + 1) % cnt] - pos;
            var cip1 = new System.Numerics.Complex(vip1.x,vip1.y);
            radchange += (cip1 / ci).Phase;
            ci = cip1;
        }
        return radchange < -PI * 2;
    }
    void SolveInnerPath(List<Vector3Int> innerPath, RegionPathsInt regionPathsInt)
    {
        regionPathsInt.innerHoles.Add(innerPath);
    }
    void SolveUnresolvedInnerPath(int index,RegionPathsInt regionPathsInt)
    {
        regionPathsInt.innerHoles.Add(unresolvedInners[index]);
        unresolvedInners.RemoveAt(index);
    }
    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        hs_targets.Clear();
        if (targets == null || targets.Count == 0) { return; }
        foreach(var e in targets)
        {
            hs_targets.Add(e);
        }
    }
    void ISerializationCallbackReceiver.OnBeforeSerialize() { }
}

public class RegionPathsInt
{
    public List<Vector3Int> outer = new List<Vector3Int>();
    public List<List<Vector3Int>> innerHoles = new List<List<Vector3Int>>();
}
public abstract class TilePattermMappingSubjectRegionPathsIntShape : TilePatternMappingSubject<RegionPathsInt> { }
public abstract class TilePatternMapperRegionPathsIntShape : TilePatternMapper<RegionPathsInt, TilePattermMappingSubjectRegionPathsIntShape>
{
    public override Vector3 ShapeToInstantiatePosition(Tilemap tilemap, Vector3Int position, RegionPathsInt rps) => tilemap.CellToWorld(rps.outer[0]);
}

//一番外側の境界の中に何個穴があるか調べる
//regionCheckedFlagsの初期化
//IEnumerable<int> vertexXs = detectedPath.Select(v => v.x);
//IEnumerable<int> vertexYs = detectedPath.Select(v => v.y);
//int minX = vertexXs.Min();
//int maxX = vertexXs.Max();
//int minY = vertexYs.Min();
//int maxY = vertexYs.Max();
//BoundsInt massBounds = new BoundsInt(
//    minX, minY, targetPos.z,
//    maxX - minX, maxY - minY, 1
//    );
//bool[,] regionCheckedFlags = new bool[massBounds.size.y, massBounds.size.x];
//System.Array.Clear(regionCheckedFlags, 0, regionCheckedFlags.Length);
//System.Func<int, int, Vector3Int> indToPos = (x, y) => new Vector3Int(x + minX, y + minY, targetPos.z);
//List<Vector3Int> outersLowerLeftCorner = FindLowerLeftCorner(detectedPath);
//for(int i = 0; i < outersLowerLeftCorner.Count; ++i)
//{
//    if (outersLowerLeftCorner[i] != targetPos)
//    {
//        regionCheckedFlags[outersLowerLeftCorner[i].y - minY, outersLowerLeftCorner[i].y - minX] = true;
//    }
//}