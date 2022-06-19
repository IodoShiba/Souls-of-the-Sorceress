using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UniRx;
using System;
using UnityEngine.SceneManagement;
using UniRx;
using UniRx.Async;

namespace SotS.UI.Dialog
{
    public class DialogCambasScene : MonoBehaviour
    {
        [SerializeField] GameObject scrollContent;
        [SerializeField] GameObject dialogObj;
        [SerializeField] UnityEngine.UI.Selectable initialSelected;

        [SerializeField, DisabledField] GameObject returnSelectable = null;

        Subject<Unit> subjectOnModalClose = new Subject<Unit>();
        Subject<Unit> subjectOnOkPressed = new Subject<Unit>();
        public IObservable<Unit> OnModalClose { get => subjectOnModalClose; }
        public IObservable<Unit> OnOkPressed{ get => subjectOnOkPressed; }


        public GameObject ScrollContent {get => scrollContent;}

        public void ShowModal()
        {
            dialogObj.SetActive(true);
            var currentEventSystem = UnityEngine.EventSystems.EventSystem.current;

            // Debug.Log($"{currentEventSystem.GetInstanceID()}::{currentEventSystem.name}");

            returnSelectable = currentEventSystem.currentSelectedGameObject;

            // Debug.Log(returnSelectable);

            currentEventSystem.SetSelectedGameObject(initialSelected.gameObject);
        }

        public void ShowModal(Vector2 offset)
        {
            var rectTransform = dialogObj.GetComponent<RectTransform>();
            rectTransform.offsetMax = offset;
            rectTransform.offsetMin = offset;
            ShowModal();
        }

        public void CloseModal() 
        {
            Debug.Log("CloseModal()");
            subjectOnModalClose.OnNext(Unit.Default);

            Debug.Log(returnSelectable);

            var currentEventSystem = UnityEngine.EventSystems.EventSystem.current;

            // while(true)
            // {
            //     Debug.Log("stop");
            //     await UniRx.Async.UniTask.Yield();
            // }

            // Debug.Log($"{currentEventSystem.GetInstanceID()}::{currentEventSystem.name}");

            currentEventSystem.SetSelectedGameObject(returnSelectable);

            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.UnloadSceneAsync(currentScene);
        }

        public void _OkPressed()
        {
            subjectOnOkPressed.OnNext(Unit.Default);
            CloseModal();
        }

    }

    public static class DialogCambasSceneHelper
    {
        public static DialogCambasScene FindDialogCambasScene(this in Scene scene)
        {
            GameObject[] roots = scene.GetRootGameObjects();

            DialogCambasScene result = null;

            for(int i=0; i<roots.Length; ++i)
            {
                if(roots[i].TryGetComponent<DialogCambasScene>(out result))
                {
                    break;
                }
            }
            return result;
        }
    }
}