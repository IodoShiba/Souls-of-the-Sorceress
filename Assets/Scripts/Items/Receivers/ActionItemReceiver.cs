using UnityEngine;
using System.Collections;

/// <summary>
/// Actorが回復アイテムを受け取る性質を担うコンポーネント
/// ActorのGameObjectにアタッチする
/// </summary>
[DisallowMultipleComponent]
public class ActionItemReceiver : ItemReceiver<ActionItem>
{
    protected override void OnReceiveItem(ActionItem item)
    {
        item.InvokeAction();
    }
}
