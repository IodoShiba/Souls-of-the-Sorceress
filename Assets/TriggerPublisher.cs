using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerPublisher : MonoBehaviour
{
    [System.Serializable] class UnityEvent_Collider2D : UnityEvent<Collider2D> {}

    [SerializeField, TagField] string tag;
    [SerializeField] UnityEvent_Collider2D onEnter;
    [SerializeField] UnityEvent_Collider2D onExit;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(string.IsNullOrEmpty(tag) || other.CompareTag(tag))
        {
            onEnter.Invoke(other);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(string.IsNullOrEmpty(tag) || other.CompareTag(tag))
        {
            onExit.Invoke(other);
        }
    }
}
