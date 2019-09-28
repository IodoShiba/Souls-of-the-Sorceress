using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeToFireEvent : MonoBehaviour
{
    [SerializeField] float time;
    [SerializeField] UnityEngine.Events.UnityEvent events;

    public float Time { get => time;
        private set
        {
            if(value <= 0 && time > 0)
            {
                events.Invoke();
            }
            time = value;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Time -= UnityEngine.Time.deltaTime;
    }
}
