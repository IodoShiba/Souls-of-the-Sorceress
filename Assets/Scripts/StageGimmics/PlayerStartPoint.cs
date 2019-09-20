using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStartPoint : MonoBehaviour
{
    enum StartMode
    {
        UsePlacedPlayer,
        InstantiateNewPlayer,
    }

    [SerializeField] StartMode startMode;
    [SerializeField] Player player;
    [SerializeField] SaveData saveData;

    private void Awake()
    {
        switch (startMode)
        {
            case StartMode.UsePlacedPlayer:
                if(
                    !UnityEngine.EventSystems.ExecuteEvents.CanHandleEvent<SaveData.IPlayerHealthCareer>(player.gameObject)||
                    !UnityEngine.EventSystems.ExecuteEvents.CanHandleEvent<SaveData.IPlayerAwakeCareer>(player.gameObject)||
                    !UnityEngine.EventSystems.ExecuteEvents.CanHandleEvent<SaveData.IPlayerProgressLevelCareer>(player.gameObject)
                    )
                {
                    Debug.Log("player cannot handle event.");
                }
                player.transform.position = transform.position;
                saveData.RestorePlayer(player);
                break;

            case StartMode.InstantiateNewPlayer:
                if (
                    !UnityEngine.EventSystems.ExecuteEvents.CanHandleEvent<SaveData.IPlayerHealthCareer>(player.gameObject) ||
                    !UnityEngine.EventSystems.ExecuteEvents.CanHandleEvent<SaveData.IPlayerAwakeCareer>(player.gameObject) ||
                    !UnityEngine.EventSystems.ExecuteEvents.CanHandleEvent<SaveData.IPlayerProgressLevelCareer>(player.gameObject)
                    )
                {
                    Debug.Log("player cannot handle event.");
                }
                saveData.RestorePlayer(Instantiate(player, transform.position, Quaternion.identity));
                break;
        }
    }
}
