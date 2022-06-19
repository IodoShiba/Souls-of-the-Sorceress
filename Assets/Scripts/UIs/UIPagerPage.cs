using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using DG.Tweening;

namespace SotS.UI
{

    [RequireComponent(typeof(UnityEngine.CanvasGroup))]
    public class UIPagerPage : MonoBehaviour
    {
        [SerializeField] GameObject initialSelected;
        GameObject finalSelected = null;

        CanvasGroup canvasGroup;

        void Start()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void Show() 
        {
            EventSystem currentEventSystem = EventSystem.current;
            currentEventSystem.SetSelectedGameObject(finalSelected == null ? initialSelected : finalSelected);
            
            canvasGroup.interactable = true;

            EffectIn();
        }

        public void Hide() 
        {
            EventSystem currentEventSystem = EventSystem.current;
            finalSelected = currentEventSystem.currentSelectedGameObject;

            canvasGroup.interactable = false;

            EffectOut();
        }

        void EffectIn()
        {
            canvasGroup.alpha = 1;
        }

        void EffectOut()
        {
            canvasGroup.alpha = 0;
        }
    }
}
