using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnStartFireEvent : MonoBehaviour
{
    [SerializeField] UnityEvent events;

    void Start()
    {
        events.Invoke();
    }
}
