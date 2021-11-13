using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStarter : MonoBehaviour
{
    enum StartMode
    {
        UsePlacedPlayer,
        InstantiateNewPlayer,
    }

    [SerializeField] bool allowStartInAir;
    [SerializeField] StartMode startMode;
    [SerializeField] Player player;
    [SerializeField] SaveData saveData;

    private void Start()
    {
        switch (startMode)
        {
            case StartMode.UsePlacedPlayer:
                //saveData.RestorePlayer(player);
                SotS.GameCommonInterface.Instance.Player.TryRestorePlayer(player);
                break;

            case StartMode.InstantiateNewPlayer:
                //saveData.RestorePlayer(Instantiate(player, GetStartPoint(player), Quaternion.identity));
                SotS.GameCommonInterface.Instance.Player.TryRestorePlayer(Instantiate(player, GetStartPoint(player), Quaternion.identity));
                break;
        }
        
    }

    Vector3 GetStartPoint(Player player)
    {
        if (allowStartInAir)
        {
            return transform.position;
        }
        else //うまく動かない
        {
            int mask = LayerMask.NameToLayer(LayerName.ground);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 5, mask);
            if (hit.collider == null)
            {
                return transform.position;
            }
            BoxCollider2D playerBoxCollider = player.GetComponent<BoxCollider2D>();
            return (Vector3)(hit.point + (playerBoxCollider.bounds.size.y / 2) * Vector2.up)+Vector3.forward*transform.position.z;
        }
    }
}
