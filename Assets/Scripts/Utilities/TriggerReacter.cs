using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerReacter : MonoBehaviour
{
    [System.Serializable] public class UnityEvent_Collider2D : UnityEvent<Collider2D> {}

    [SerializeField] bool useTargetTag;
    [SerializeField] string targetTag;
    [SerializeField] UnityEvent_Collider2D onTriggerEnter;

    public UnityEvent_Collider2D OnTriggerEnter { get => onTriggerEnter; }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(useTargetTag && !other.CompareTag(targetTag)){ return; }
        onTriggerEnter.Invoke(other);
    }
}
