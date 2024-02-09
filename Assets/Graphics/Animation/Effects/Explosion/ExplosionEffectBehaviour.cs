using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEffectBehaviour: MonoBehaviour
{
    public void DestroyThis()
    {
        Debug.Log("ExplosionEffectBehaviour.DestroyThis");
        Destroy(gameObject);
    }
}
