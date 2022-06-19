using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActorCommanderUtility;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace SotS.UI
{
    public class SotSButton : 
        UnityEngine.UI.Selectable,

        IPointerClickHandler
    {
        [SerializeField] UnityEvent onClick;

        public void OnPointerClick(PointerEventData eventData)
        {
            onClick.Invoke();
            eventData.Use();
        }
    }
}