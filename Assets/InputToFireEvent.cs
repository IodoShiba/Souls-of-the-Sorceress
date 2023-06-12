using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputToFireEvent : MonoBehaviour
{
    [System.Serializable]
    class InputEvent
    {
        public string buttonName;
        public UnityEngine.Events.UnityEvent events;
    }

    [SerializeField] List<InputEvent> inputEvents;

    private void Update()
    {
        for(int i = 0; i < inputEvents.Count; ++i)
        {
            if (InputDaemon.IsPressed(inputEvents[i].buttonName))
            {
                inputEvents[i].events.Invoke();
            }
        }
    }

}
