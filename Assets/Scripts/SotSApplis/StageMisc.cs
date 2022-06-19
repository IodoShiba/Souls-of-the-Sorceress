using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SotSGame/StageMisc")]
public class StageMisc : ScriptableObject
{
    [SerializeField] string stageName;  // 20220620現在　無意味
    [SerializeField] string stageSubName;   // 20220620現在　役目無し
    [SerializeField, TextArea] string stageDescription; // ステージの説明｜フレーバーテキスト　あると盛り上がる
    [SerializeField] Sprite stageImage; // ステージのイメージ画像

    public string StageName { get => stageName; }
    public string StageSubName { get => stageSubName; }
    public string StageDescription { get => stageDescription; }
    public Sprite StageImage { get => stageImage; }
}
