using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsFixer : MonoBehaviour
{
    public const int TARGET_FPS = 60;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void RuntimeInitialize()
    {
        Application.targetFrameRate = TARGET_FPS;
    }
}
