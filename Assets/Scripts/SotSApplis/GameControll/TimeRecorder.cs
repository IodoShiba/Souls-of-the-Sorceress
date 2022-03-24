using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class TimeRecorder : MonoBehaviour
{
    static bool isRecording = false;
    static float timeSum = 0;
    static float timeStarted;

    public static float timeElapsed {get => timeSum + (Time.unscaledTime - timeStarted);}

    // Start is called before the first frame update
    void Start()
    {
        StartTimer();
    }

    void OnDestroy()
    {
        StopTimer();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void RuntimeInitialize()
    {
        GameLifeCycle.observableOnGameOpen.Subscribe(_=>ResetTimer());
        GameLifeCycle.observableOnGameClose.Subscribe(_=>StopTimer());
    }

    public static void ResetTimer()
    {
        timeSum = 0;

        isRecording = false;
    }
    public static void StartTimer()
    {
        if(isRecording) {return;}

        timeStarted = Time.unscaledTime;
        isRecording = true;

        //Debug.Log("timer start");
    }
    public static void StopTimer()
    {
        if(!isRecording){return;}

        timeSum = timeElapsed;
        isRecording = false;

        //Debug.Log("timer stop");
    }

    // for debug purpose
    [SerializeField, DisabledField] string time;
    void Update()
    {
        if(!isRecording){return;}
        time = System.TimeSpan.FromSeconds(timeElapsed).ToString("c");
    }

    // void Awake()
    // {
    //     System.Threading.Thread.Sleep(3000);
    // }
}
