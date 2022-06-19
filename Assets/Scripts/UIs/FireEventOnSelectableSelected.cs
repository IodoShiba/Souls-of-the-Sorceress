using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class FireEventOnSelectableSelected : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [System.Serializable]
    public class UnityEvent_BaseEventData : UnityEvent<BaseEventData> {}

    //FIXME: Unity2019の、privateのUnityEventをシリアライズできないバグによりpublicにしている
    [SerializeField] public UnityEvent_BaseEventData onSelected;
    [SerializeField] public UnityEvent_BaseEventData onDeselected;

    public void OnSelect(BaseEventData eventData)
    {
        onSelected.Invoke(eventData);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        onDeselected.Invoke(eventData);
    }

    // void Start()
    // {
    //     onSelected.AddListener(e=>Debug.Log($"selected:{e.selectedObject.name}"));
    // }
}
