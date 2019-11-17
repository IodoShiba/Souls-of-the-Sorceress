using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IodoShibaUtil.Vector2DUtility;

public class AwakeChargeItemPool : MonoBehaviour
{
    [SerializeField] int poolSize = 128;
    [SerializeField] Sprite[] spriteVariations;
    [SerializeField] AwakeChargeItem prefab;
    AwakeChargeItem[] pool;
    int next;
    static AwakeChargeItemPool instance;

    public static AwakeChargeItemPool Instance { get => instance; }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Debug.LogError($"Two or more {this.GetType().Name} cannot exist in one scene. GameObject '{name}' has been Deleted because it has second {this.GetType().Name}.");
            Destroy(gameObject);
            return;
        }

        pool = new AwakeChargeItem[poolSize];
        for (int i = 0; i < pool.Length; ++i)
        {
            pool[i] = Instantiate(prefab, Vector2DUtilityClass.ModifiedXY(transform.position, Vector2.zero), Quaternion.identity);
            pool[i].transform.SetParent(transform);
            pool[i].gameObject.SetActive(false);
        }
        next = 0;
    }

    public bool _Borrow(in Vector2 position, in Vector2 velocity, float chargeAmount)
    {
        for (int i = 0; i < pool.Length; ++i)
        {
            if (pool[next] != null && !pool[next].gameObject.activeSelf)
            {
                break;
            }
            next = (next + 1) % pool.Length;
        }
        if(pool[next] == null || pool[next].gameObject.activeSelf) { return false; }

        pool[next].Activate(position, velocity, chargeAmount);
        next = (next + 1) % pool.Length;
        return true;
    }

    public static bool Borrow(in Vector2 position, in Vector2 velocity, float chargeAmount) => Instance._Borrow(position, velocity, chargeAmount);
}
