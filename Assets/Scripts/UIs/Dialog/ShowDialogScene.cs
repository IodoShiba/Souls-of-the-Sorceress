using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SotS.UI.Dialog;
using UniRx.Async;
using UniRx;
using UnityEngine.Events;

namespace SotS.UI
{
    public class ShowDialogScene : MonoBehaviour
    {
        [SerializeField] Hertzole.SceneObject dialogScene;
        [SerializeField] bool dontSubscribeOnModalClosedEvent;
        [SerializeField] UnityEvent onModalClosed;

        bool isModalOpen = false;

        public bool IsModalOpen {get => isModalOpen;}

        public async void Show()
        {
            isModalOpen = true;

            var currentEventSystem = UnityEngine.EventSystems.EventSystem.current;

            GameObject lastSelected = currentEventSystem.currentSelectedGameObject;

            await SceneManager.LoadSceneAsync((string)dialogScene, LoadSceneMode.Additive);
            Scene dialogSceneInstance = SceneManager.GetSceneByName(dialogScene);
            DialogCambasScene dialogCambasScene = dialogSceneInstance.FindDialogCambasScene();
            if(dialogCambasScene == null)
            {
                await SceneManager.UnloadSceneAsync(dialogScene);
                throw new System.ArgumentException($"Selected scene was not dialog scene. selected: {(string)dialogScene}");
            }

            if(!dontSubscribeOnModalClosedEvent)
            {
                dialogCambasScene.OnModalClose.Subscribe(_=>onModalClosed.Invoke());
            }
            dialogCambasScene.OnModalClose.Subscribe(_=>isModalOpen = false);
            // SceneManager.SetActiveScene(dialogSceneInstance);

            // dialogCambasScene.OnModalClose.Subscribe(
            //     _=>
            //     {
            //         Debug.Log(lastSelected);
            //         UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(lastSelected);
            //     }
            //     );

            dialogCambasScene.ShowModal();
        }

        public void SubscribeOnModalClosed(UnityAction action)
        {
            onModalClosed.AddListener(action);
        }
        
        public void UnsubscribeOnModalClosed(UnityAction action)
        {
            onModalClosed.RemoveListener(action);
        }
    }
}