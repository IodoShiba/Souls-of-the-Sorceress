using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SotS;
using UniRx;

public class RemainCountViewer : MonoBehaviour
{
    const string format = "<sprite name=\"sarah\">x{0}";

    [SerializeField] TMPro.TMP_Text text;

    System.IDisposable subscribed = null;
    string formatted;

    void Start()
    {
        subscribed = ReviveController.RemainingCountListener.Subscribe(ListenRemainingCount);
        
        formatted = string.Format(format, ReviveController.Remaining);
        text.text = formatted;
    }

    void OnDestroy()
    {
        if(subscribed != null)
        {
            subscribed.Dispose();
        }
    }

    void ListenRemainingCount(int count)
    {
        formatted = string.Format(format, count);
        text.text = formatted;
    }

    [System.Serializable]
    class UnityEvent_string : UnityEngine.Events.UnityEvent<string> {}
}
