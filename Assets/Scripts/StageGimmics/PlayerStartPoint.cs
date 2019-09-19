using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStartPoint : MonoBehaviour
{
    [SerializeField] Player playerPrefab;
    [SerializeField] SaveData saveData;

    private void Awake()
    {
        saveData.RestorePlayer(Instantiate(playerPrefab, transform.position, Quaternion.identity));
    }
}
