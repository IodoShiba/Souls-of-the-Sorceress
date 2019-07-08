using UnityEngine;
using System.Collections;

/// <summary>
/// アイテムのコンポーネントの基底クラス
/// </summary>
public class ItemBase : MonoBehaviour
{
    /// <summary>
    /// アイテムが受け取られたときに呼び出されるメソッド
    /// </summary>
    public virtual void OnReceived()
    {
        Destroy(gameObject);
    }

}
