using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "New Particular Rule Tile", menuName = "Tiles/Particular Rule Tile")]
public class ParticularRuleTile : RuleTile<TreeTile.Neighbor>
{
    public bool customField;
    [System.Serializable]
    public class ParticularMatch
    {
        public int index = 0;
        //[HideInInspector]
        public bool valid = false;
        [HideInInspector]
        public List<TileBase> match = new List<TileBase>();
        void ResetMatch()
        {
            match = new List<TileBase> { null, null, null, null, null, null, null, null };
        }

        public ParticularMatch(TileBase[] arg)
        {
            for (int i = 0; i < 8; ++i)
            {
                match[i] = arg[i];
            }
            valid = false;
        }
        public ParticularMatch()
        {
            ResetMatch();
            valid = false;
        }

        public TileBase this[int i] { get => match[i]; }

#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(ParticularMatch))]
        public class Draw : PropertyDrawer
        {
            //おそらく問題なし
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                property.serializedObject.Update();
                EditorGUI.BeginChangeCheck();

                //Debug.Log(property.serializedObject == null);
                if (property.serializedObject == null||!property.FindPropertyRelative("valid").boolValue)
                {
                    EditorGUI.LabelField(position,$"[ Rule {property.FindPropertyRelative("index").intValue} contains no particular ]");
                    return;
                };

                EditorGUI.LabelField(position, $"Particular Match (for rule {property.FindPropertyRelative("index").intValue})");
                position.y += EditorGUIUtility.singleLineHeight;
                var matchp = property.FindPropertyRelative("match");
                //Debug.Log(matchp == null);
                if (matchp == null || !matchp.isArray)
                {
                    return;
                }

                Rect rect = new Rect(position.x, position.y, 64 * 2, 16);
                //EditorGUI.ObjectField(rect, property.FindPropertyRelative("sprite0"), GUIContent.none);
                var v = new Vector2(position.x, position.y);
                var s = new Vector2(96, 16);
                var sm = new Vector2(100, 16);
                int i = 0;
                for (int y = 0; y < 3; ++y)
                {
                    for (int x = 0; x < 3; ++x)
                    {
                        if (x == 1 && y == 1)
                        {
                            EditorGUI.LabelField(new Rect(v + new Vector2(x * s.x, y * s.y), s), "( Self )");
                            continue;
                        }
                        // Debug.Log(i);
                        //Debug.Log(matchp.isArray);
                        if (i >= matchp.arraySize && matchp.arraySize < 8) matchp.InsertArrayElementAtIndex(i);
                        
                        var p = matchp.GetArrayElementAtIndex(i);
                        //EditorGUI.PropertyField(new Rect(v + new Vector2(x * s.x, y * s.y), s), p,GUIContent.none);
                        
                        EditorGUI.ObjectField(new Rect(v + new Vector2(x * sm.x, y * sm.y), s), p, typeof(TileBase),GUIContent.none);
                        ++i;
                    }
                }
                //Debug.Log(matchp.arraySize);

                if (EditorGUI.EndChangeCheck()) property.serializedObject.ApplyModifiedProperties();
            }
            public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => property.FindPropertyRelative("valid").boolValue ? 64 : 16;

        }
#endif
    }

    [HideInInspector]
    public List<ParticularMatch> particularMatches;
    public class Neighbor : RuleTile.TilingRule.Neighbor
    {
        public const int Null = 3;
        public const int NotNull = 4;
        public const int OtherTile = 5;
        public const int Particular = 6;
    }
    protected virtual void OnEnable()
    {
        if (particularMatches == null) particularMatches = new List<ParticularMatch>();

        //Debug.Log(m_TilingRules.Count);
    }
    public void UpdateParticularMatchList()
    {
        //particularMatchesの要素数をm_TilingRulesの要素数に合わせる
        int pmc = particularMatches.Count;
        int trc = m_TilingRules.Count;
        if (pmc > trc)//削る
        {
            Debug.Log("Remove");
            for (int i = pmc - 1; i >= trc; --i) { particularMatches.RemoveAt(i); }
        }
        else if (pmc < trc) //増やす
        {
            Debug.Log("Add");
            for (int i = pmc; i < trc; ++i) { particularMatches.Add(new ParticularMatch()); }
        }
        //Debug.Log(particularMatches.Count == m_TilingRules.Count);
        bool ContainsParticular(TilingRule rule)
        {
            foreach (int i in rule.m_Neighbors)
            {
                if (i == Neighbor.Particular) return true;
            }
            return false;
        }
        //particularMatchesの各要素をm_TilingRulesと合致させる
        for (int i = 0; i < trc; ++i)
        {
            particularMatches[i].valid = ContainsParticular(m_TilingRules[i]);
            particularMatches[i].index = i;
        }
    }
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        base.GetTileData(position, tilemap, ref tileData);
    }
    protected override bool RuleMatches(TilingRule rule, ref TileBase[] neighboringTiles, ref Matrix4x4 transform)
    {
        int selfIndex = m_TilingRules.FindIndex(i => i == rule);
        for (int i = 0; i < neighborCount; ++i)
        {
            if (rule.m_Neighbors[i] == Neighbor.Particular && !(particularMatches[selfIndex][i] == neighboringTiles[i]))
            {
                 return false;
            }
        }
        return base.RuleMatches(rule, ref neighboringTiles, ref transform);
    }
    public override bool RuleMatch(int neighbor, TileBase tile)
    {
        switch (neighbor)
        {
            case Neighbor.Null: return tile == null;
            case Neighbor.NotNull: return tile != null;
            case Neighbor.OtherTile: return tile != null && tile != m_Self;
        }
        return base.RuleMatch(neighbor, tile);
    }
}