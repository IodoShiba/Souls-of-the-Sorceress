using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "PatternMapperHelper/TileObjectMap")]
public class TileObjectMap : ScriptableObject
{
    [System.Serializable]
    class Entry
    {
        public TileBase tile;
        public GameObject prefab;
    }

    [SerializeField] Entry[] list;

    Dictionary<TileBase, GameObject> map;

    void Awake()
    {
        if(map == null)
        {
            MakeMap();
        }
    }

    void MakeMap()
    {
        map = new Dictionary<TileBase, GameObject>();
        if(list == null){return;}
        for(int i=0; i<list.Length; ++i)
        {
            map.Add(list[i].tile, list[i].prefab);
        }
    }

    public GameObject this[TileBase tile]
    {
        get
        {
            if(map == null)
            {
                MakeMap();
            }

            return map[tile];
        }
    }

    public bool TryGet(TileBase keyTile, out GameObject prefab)
    {
        if(map == null)
        {
            MakeMap();
        }
        if(keyTile == null)
        {
            prefab = null;
            return false;
        }

        return map.TryGetValue(keyTile, out prefab);
    }
}
