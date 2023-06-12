using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleToStage0 : MonoBehaviour
{
    bool readyForMoveScene;

    public bool ReadyForMoveScene { get => readyForMoveScene; set => readyForMoveScene = value; }

    // Start is called before the first frame update
    void Start()
    {
        readyForMoveScene = false;

        readyForMoveScene = true;

        StartCoroutine(WaitForButtonInput());
    }

    IEnumerator WaitForButtonInput()
    {
        while (!InputDaemon.IsPressed(InputName.Button.attack)) { yield return null; } 

        while (!readyForMoveScene) { yield return null; }

        UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName.stage0);
    }
}
