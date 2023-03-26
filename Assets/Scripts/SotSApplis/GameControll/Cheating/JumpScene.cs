using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace SotS.Cheating
{
    public class JumpScene : MonoBehaviour
    {
        [SerializeField] SceneChanger sceneChanger;
        [SerializeField] InputField inputField;

        readonly string secretCommand = "forward";
        int i = 0;
        bool inputEnabled = false;

        void Start()
        {
            inputField.onEndEdit.AddListener((destinationSceneName)=>{sceneChanger.ChangeScene(destinationSceneName);});
        }

        void Update() // FIXME: Find altenative way to realize this by InputSystem.
        {
            // if(inputEnabled){return;}

            // if(Input.anyKeyDown)
            // {
            //     string inputString = Input.inputString;
            //     if(string.IsNullOrEmpty(inputString)){return;}
                
            //     i = (i < secretCommand.Length && inputString[0] == secretCommand[i]) ? i+1 : 0;

            //     if(i == secretCommand.Length)
            //     {
            //         EnableInput();
            //     }
            // }
        }

        void EnableInput()
        {
            inputField.enabled = true;
            inputField.image.enabled = true;
            inputField.placeholder.enabled = true;

            inputEnabled = true;
        }
    }
}