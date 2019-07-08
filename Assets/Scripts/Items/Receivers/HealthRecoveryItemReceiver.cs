using UnityEngine;
using System.Collections;

/// <summary>
/// Actorが回復アイテムを受け取る性質を担うコンポーネント
/// ActorのGameObjectにアタッチする
/// </summary>
public class HealthRecoveryItemReceiver : ItemReceiver<HealthRecoveryItem>
{
    Player selfPlayer;
    private void Awake()
    {
        selfPlayer = GetComponent<Player>();
    }
    protected override void OnReceiveItem(HealthRecoveryItem item)
    {
        selfPlayer.RecoverHealth(item.Amount);
    }
}
