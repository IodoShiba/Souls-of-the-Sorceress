using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif


[CreateAssetMenu(fileName = "New Tree Tile", menuName = "Tiles/Tree Tile")]
public class TreeTile : ParticularRuleTile
{
    //public List<RuleToParticularMatch> ruleToParticularMatches;

    //残す
    [ContextMenu("Reset Tiling Rules")]
    public void ResetTilingRules(){
        m_TilingRules.Clear();

        void AddRule(int[] vs, bool setHorizontalSwap = true) //vsへの副作用あります
        {
            void SetRule(int[] vs2)
            {
                TilingRule target = new TilingRule();
                target.m_ColliderType = Tile.ColliderType.Grid;
                for (int i = 0; i < neighborCount; ++i)
                {
                    target.m_Neighbors[i] = vs2[i];
                }
                m_TilingRules.Add(target);
            }
            void SwapVs(int i1, int i2) { int t = vs[i1]; vs[i1] = vs[i2]; vs[i2] = t; }

            SetRule(vs);
            if (vs[0] == vs[2] && vs[3] == vs[4] && vs[5] == vs[7]) return;
            SwapVs(0, 2);
            SwapVs(3, 4);
            SwapVs(5, 7);
            SetRule(vs);

        }
        Debug.Log("reset called");
        const int T = Neighbor.This;
        const int X = Neighbor.NotThis;
        const int Z = Neighbor.OtherTile;
        const int P = Neighbor.Particular;
        
        AddRule(new int[] { //幹　枝接合部
            X,T,0,
            P,  0,
            X,T,0});
        AddRule(new int[] { //幹　通常
            0,T,0,
            T,  T,
            0,T,0});
        AddRule(new int[] { //幹　根上部接合部
            0,T,0,
            X,  T,
            T,T,0});
        AddRule(new int[] { //幹　中間端
            0,T,0,
            X,  T,
            0,T,0});
        AddRule(new int[] { //幹　根接合部
            X,T,0,
            T,  T,
            0,0,0});
        AddRule(new int[] { //幹　根接合部　両側
            X,T,X,
            T,  T,
            0,X,0});
        AddRule(new int[] { //幹　頂上　端
            0,X,0,
            X,  T,
            X,T,0});
        AddRule(new int[] { //幹　頂上　中間
            0,X,0,
            T,  T,
            0,T,0});
        AddRule(new int[] { //幹　頂上　端　根接合
            0,X,0,
            X,  T,
            T,T,0});
        AddRule(new int[] { //根
            0,0,0,
            0,  T,
            0,0,0});
        //AddRule(new int[] { //
        //    0,0,0,
        //    0,  0,
        //    0,0,0});
        Debug.Log(m_TilingRules.Count);

        UpdateParticularMatchList();
    }

    //移す

    public class Neighbor : ParticularRuleTile.Neighbor
    {
        //public const int Null = 3;
        //public const int NotNull = 4;
        //public const int OtherTile = 5;
        //public const int Particular = 6;
    }
    protected override void OnEnable()
    {
        if (m_TilingRules.Count == 0)
        {
            ResetTilingRules();
        }
    }
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        base.GetTileData(position, tilemap, ref tileData);
    }
}

