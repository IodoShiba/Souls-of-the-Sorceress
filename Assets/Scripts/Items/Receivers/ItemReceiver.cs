using UnityEngine;
using System.Collections;

/// <summary>
/// Actorにアイテムを受け取る性質を付与するコンポーネント
/// </summary>
/// <typeparam name="ItemType">受け取るItemコンポーネントの型名</typeparam>
public abstract class ItemReceiver<ItemType> : MonoBehaviour where ItemType : ItemBase
{
    protected abstract void OnReceiveItem(ItemType item);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Item")
        {
            var item = collision.GetComponent<ItemType>();
            if (item != null)
            {
                OnReceiveItem(item);
                item.OnReceived();
            }
        }
    }
}
