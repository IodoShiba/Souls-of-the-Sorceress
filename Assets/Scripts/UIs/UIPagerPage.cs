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
        [SerializeField] private bool hideWhenExited = false;
        [SerializeField] GameObject initialSelected;
        [SerializeField] bool rememberFinalSelected;
        GameObject finalSelected = null;

        CanvasGroup canvasGroup;

        public bool IsShown => canvasGroup != null && canvasGroup.alpha != 0;

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
        public void EnterSelection()
        {
            EventSystem currentEventSystem = EventSystem.current;
            currentEventSystem.SetSelectedGameObject(finalSelected == null ? initialSelected : finalSelected);

            canvasGroup.interactable = true;

            if (! IsShown)
            {
                Show();
            }
        }
        public void ExitSelection()
        {
            EventSystem currentEventSystem = EventSystem.current;
            if (rememberFinalSelected)
            {
                finalSelected = currentEventSystem.currentSelectedGameObject;
            }

            canvasGroup.interactable = false;

            if (hideWhenExited)
            {
                Hide();
            }
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
