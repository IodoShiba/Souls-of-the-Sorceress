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
        [SerializeField] private bool hideWhenPushed = false;
        [SerializeField] GameObject initialSelected;
        [SerializeField] bool rememberFinalSelected;
        [SerializeField] private EventSystem _referenceEventSystem;
        GameObject finalSelected = null;

        CanvasGroup canvasGroup;

        public bool IsShown => canvasGroup != null && canvasGroup.alpha != 0;
        public bool HideWhenPushed => hideWhenPushed;

        void Start()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void Show() 
        {
            EffectIn();
        }

        public void Hide() 
        {
            EffectOut();
        }

        EventSystem GetReferenceEventSystem()
        {
            return _referenceEventSystem != null ? _referenceEventSystem : EventSystem.current;
        }
        
        public void EnterSelection(bool final)
        {
            if(final)
            {
                EventSystem referenceEventSystem = GetReferenceEventSystem();
                referenceEventSystem.SetSelectedGameObject(finalSelected == null ? initialSelected : finalSelected);
            }
            
            canvasGroup.interactable = true;

            if (! IsShown)
            {
                Show();
            }
        }
        public void ExitSelection()
        {
            EventSystem eventSystem = GetReferenceEventSystem();
            if (rememberFinalSelected)
            {
                var currentSelected = eventSystem.currentSelectedGameObject;
                finalSelected = currentSelected != null ? currentSelected : initialSelected;
            }

            canvasGroup.interactable = false;
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
