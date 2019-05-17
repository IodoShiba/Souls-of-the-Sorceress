using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;

[UnityEditor.InitializeOnLoad]
#endif

[CreateAssetMenu(fileName = "New Barrier Tile", menuName = "Tiles/Barrier Tile")]
public class BarrierTile : TileBase
{
#if UNITY_EDITOR
    static BarrierTile()
    {
        UnityEditor.EditorApplication.playModeStateChanged += RefleshWhenPlaySwitched;
    }

    static void RefleshWhenPlaySwitched(PlayModeStateChange playModeStateChange)
    {
        switch (playModeStateChange)
        {
            case PlayModeStateChange.EnteredEditMode:
                actualSprite = _fake;
                break;
            case PlayModeStateChange.EnteredPlayMode:
                actualSprite = null;
                break;
        }
        //if (instance.itilemap != null) instance.itilemap.GetComponent<Tilemap>().SetEditorPreviewTile();
    }
#endif 
    
    public BarrierTile()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    
    [SerializeField] Sprite fakeSprite;
    static Sprite _fake;
    static Sprite actualSprite = null;
    static BarrierTile instance;
    ITilemap itilemap;

    public static BarrierTile Instance { get => instance;}

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite = fakeSprite;
        tileData.color = Color.clear;
//        _fake = fakeSprite;
//        itilemap = tilemap;
//        Debug.Log("Called:GetTileData()");
//#if UNITY_EDITOR
//        if (!EditorApplication.isPlaying)
//        {
//            actualSprite = _fake;
//        }
//#endif
//        tileData.sprite = actualSprite;
    }
}

