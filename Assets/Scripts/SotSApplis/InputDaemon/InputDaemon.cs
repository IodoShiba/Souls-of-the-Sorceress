using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputDaemon : MonoBehaviour
{
    public static Vector2 GetVector2(string actionName) 
    {
        return input.actions[actionName].ReadValue<Vector2>();
    }

    public static bool IsPressed(string actionName)
    {
        return input.actions[actionName].IsPressed();
    }

    public static bool WasPressedThisFrame(string actionName)
    {
        return input.actions[actionName].WasPressedThisFrame();
    }

    public static bool WasReleasedThisFrame(string actionName)
    {
        return input.actions[actionName].WasReleasedThisFrame();
    }

    
    public static PlayerInput input {get; private set;}
    public static InputDaemon instance {get; private set;}

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void RuntimeInitializeOnLoad()
    {
        // instance = GameObject.Instantiate(Resources.Load<GameObject>("SoundManager")).GetComponent<SoundManager>();
        instance = Instantiate<GameObject>(Resources.Load<GameObject>("SotSApplis/InputDaemon/InputDaemon")).GetComponent<InputDaemon>();
        DontDestroyOnLoad(instance); 
        input = instance.GetComponent<PlayerInput>();
    }
}
