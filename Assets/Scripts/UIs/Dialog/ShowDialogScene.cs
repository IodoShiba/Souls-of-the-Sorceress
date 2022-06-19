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

        public async void Show()
        {
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
            SceneManager.SetActiveScene(dialogSceneInstance);

            // dialogCambasScene.OnModalClose.Subscribe(
            //     _=>
            //     {
            //         Debug.Log(lastSelected);
            //         UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(lastSelected);
            //     }
            //     );

            dialogCambasScene.ShowModal();
        }
    }
}