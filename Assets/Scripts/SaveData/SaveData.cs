﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SaveData : ScriptableObject
{
    public interface ISaveDataCareer<DataType> : IEventSystemHandler
    {
        void Restore(DataType data);
        void Store(SaveData target, System.Action<DataType> setter);
    }

    public interface IPlayerHealthCareer : ISaveDataCareer<(float health, float maxHealth)> { }

    public interface IPlayerAwakeCareer : ISaveDataCareer<float> { }

    public interface IPlayerProgressLevelCareer : ISaveDataCareer<int> { }

    [SerializeField] float playerHealth;
    [SerializeField] float playerMaxHealth;
    [SerializeField] float playerAwakeGauge;
    [SerializeField,Range(0,4)] int playerProgressLevel;

    static SaveData instance = null;

    public static SaveData Instance {get => instance;}

    private void OnEnable()
    {
        instance = this;
    }

    [RuntimeInitializeOnLoadMethod]
    static void RuntimeInitializeOnLoad()
    {
        Instance.Init();
    }

    public void Save(string path)
    {
        //これのJSON形式を暗号化してファイルに保存
    }

    public void Load(string path)
    {
        //これのJSON形式の暗号化を解読して上書き
    }

    public void Init()
    {
        playerHealth = playerMaxHealth;
        playerAwakeGauge = 0;
        playerProgressLevel = 0;
    }
    public void StorePlayerData(Player player)
    {
        ExecuteEvents.Execute<IPlayerHealthCareer>(player.gameObject, null, 
            (h, _) => h.Store(
                this,
                saves => {
                    this.playerHealth = saves.health;
                    this.playerMaxHealth = saves.maxHealth;
                })
            );
        ExecuteEvents.Execute<IPlayerAwakeCareer>(player.gameObject, null,
            (h, _) => h.Store(
                this,
                saves => {
                    this.playerAwakeGauge = saves;
                })
            );
        ExecuteEvents.Execute<IPlayerProgressLevelCareer>(player.gameObject, null,
            (h, _) => h.Store(
                this,
                saves => {
                    this.playerProgressLevel = saves;
                })
            );
    }

    public void RestorePlayer(Player player)
    {
        ExecuteEvents.Execute<IPlayerHealthCareer>(player.gameObject, null,
            (h, _) => h.Restore((this.playerHealth, this.playerMaxHealth)));
        ExecuteEvents.Execute<IPlayerAwakeCareer>(player.gameObject, null,
            (h, _) => h.Restore(this.playerAwakeGauge));
        ExecuteEvents.Execute<IPlayerProgressLevelCareer>(player.gameObject, null,
            (h, _) => h.Restore(this.playerProgressLevel));
    }
}
