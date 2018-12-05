using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputA : MonoBehaviour {
    [SerializeField] List<string> monitoringButtonNames = new List<string>();
    [SerializeField] float shortDowningThreshold;
    [SerializeField] float MultiDowningLimit;
    
    private class ButtonTimeRecorder
    {
        public float time=0;
        public bool upAtPreviousFrame = false;
        public bool startingLongDowning = false;
    }
    Dictionary<string,ButtonTimeRecorder> buttonTimeRecorders = new Dictionary<string,ButtonTimeRecorder>();
    
    // Use this for initialization
    void Awake () {
        buttonTimeRecorders.Clear();
        foreach(var b in monitoringButtonNames)
        {
            buttonTimeRecorders.Add(b,new ButtonTimeRecorder());
        }
	}
	
	// Update is called once per frame
	void Update () {
        foreach(var nr in buttonTimeRecorders)
        {
            if (nr.Value.upAtPreviousFrame)
            {
                nr.Value.time = 0;
                nr.Value.upAtPreviousFrame = false;
            }
            if (nr.Value.startingLongDowning)
            {
                nr.Value.startingLongDowning = false;
            }

            if (Input.GetButton(nr.Key))
            {
                float t0 = nr.Value.time;
                float t1 = t0 + Time.deltaTime;
                if(t0 < shortDowningThreshold && shortDowningThreshold < t1)
                {
                    nr.Value.startingLongDowning = true;
                }

                nr.Value.time = t1;
            }
            else if (nr.Value.time > 0)
            {
                nr.Value.upAtPreviousFrame = true;
            }
        }
        //debug
        //var tx = GetComponent<UnityEngine.UI.Text>();
        //tx.text = "";
        //Debug end
	}

    private bool _MultiButtonCond(string firstButtonName, params string[] additionalButtonNames)
    {
        float most, least;

        var r = buttonTimeRecorders[firstButtonName];
        if (r.time == 0) { return false; }
        most = r.time;
        least = most;

        for (int i = 0; i < additionalButtonNames.Length; ++i)
        {
            r = buttonTimeRecorders[additionalButtonNames[i]];
            if (r.time == 0) { return false; }
            float t = r.time;

            if (most < t) { most = t; }
            if (t < least) { least = t; }
        }
        return most - least < MultiDowningLimit;

    }
    
    public bool GetButton(string buttonName)
    {
        return Input.GetButton(buttonName);
    }

    public bool GetMultiButton(string firstButtonName,params string[] additionalButtonNames)
    {
        float most, least;

        var r = buttonTimeRecorders[firstButtonName];
        if (r.time == 0 || r.upAtPreviousFrame) { return false; }
        most = r.time;
        least = most;

        for(int i=0;i<additionalButtonNames.Length;++i)
        {
            r = buttonTimeRecorders[additionalButtonNames[i]];
            if (r.time == 0 || r.upAtPreviousFrame) { return false; }
            float t = r.time;

            if (most < t) { most = t; }
            if (t < least) { least = t; }
        }
        return most - least < MultiDowningLimit;
    }

    public bool GetButtonShortDownUp(string buttonName)
    {
        var r = buttonTimeRecorders[buttonName];
        return r.upAtPreviousFrame && r.time < shortDowningThreshold;
    }

    public bool GetMultiButtonShortDownUp(string firstButtonName, params string[] additionalButtonNames)
    {
        float most, least;

        var r = buttonTimeRecorders[firstButtonName];
        if (r.time == 0) { return false; }
        most = r.time;
        least = most;

        bool cond = r.upAtPreviousFrame;
        for (int i = 0; i < additionalButtonNames.Length; ++i)
        {
            r = buttonTimeRecorders[additionalButtonNames[i]];
            if (r.time == 0) { return false; }
            float t = r.time;

            if (most < t) { most = t; }
            if (t < least) { least = t; }

            cond = cond || r.upAtPreviousFrame;
        }
        return cond&&(most<shortDowningThreshold)&&(most - least < MultiDowningLimit);
    }

    public bool GetButtonStartingLongDown(string buttonName)
    {
        var r = buttonTimeRecorders[buttonName];
        return r.startingLongDowning;
    }

    public float GetButtonBeingDownedTime(string buttonName)
    {
        return buttonTimeRecorders[buttonName].time;
    }

    public float GetButtonLongDownUp(string buttonName)
    {
        var r = buttonTimeRecorders[buttonName];
        return r.upAtPreviousFrame && shortDowningThreshold <= r.time ? r.time : 0;
    }

    public bool GetMultiButtonDown(string firstButtonName, params string[] additionalButtonNames)
    {
        float most, least;
        var r = buttonTimeRecorders[firstButtonName];
        bool isAButtonJustDowned = Input.GetButtonDown(firstButtonName);
        bool isAnyJustDowned = isAButtonJustDowned;
        if (r.time == 0 && !isAButtonJustDowned) { return false; }
        most = r.time;
        least = most;

        for (int i = 0; i < additionalButtonNames.Length; ++i)
        {
            r = buttonTimeRecorders[additionalButtonNames[i]];
            isAButtonJustDowned = Input.GetButtonDown(additionalButtonNames[i]);
            isAnyJustDowned = isAnyJustDowned || isAButtonJustDowned;
            if (r.time == 0 && !isAButtonJustDowned) { return false; }
            float t = r.time;

            if (most < t) { most = t; }
            if (t < least) { least = t; }
        }
        return isAnyJustDowned && most - least < MultiDowningLimit;
    }
}
