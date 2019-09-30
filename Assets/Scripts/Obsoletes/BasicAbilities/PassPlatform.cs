using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PassPlatform : MonoBehaviour//,IodoShiba.ManualUpdateClass.IManualUpdate
{
    enum Mode {SwitchLayer,UsePlatformContactor }
    [SerializeField] Mode mode;
    [SerializeField, LayerField] int onPasingLayer;
    [SerializeField] Collider2D platformContactor;
    [SerializeField] float minContinueTime;
    int ordinaryLayer;
    float t = 0;
    bool usingThis = false;
    bool ordered;

    public bool UsingThis
    {
        get => usingThis;
    }

    private void Awake()
    {
        ordinaryLayer = gameObject.layer;
        usingThis = false;
        
    }

    private void Update()
    {
        if (usingThis)
        {
            t += Time.deltaTime;
            if (t > minContinueTime && !ordered)
            {
                OnTerminate();
                usingThis = false;
            }
        }
    }

    protected void OnInitialize()
    {
        t = 0;
        switch (mode)
        {
            case Mode.SwitchLayer:
                gameObject.layer = onPasingLayer;
                break;
            case Mode.UsePlatformContactor:
                platformContactor.enabled = false;
                break;
        }
    }

    protected void OnTerminate()
    {
        t = 0;
        if (mode == Mode.SwitchLayer)
        {
            gameObject.layer = ordinaryLayer;
        }
        if(platformContactor!=null) platformContactor.enabled = true;
    }
    public void Use(bool value)        
    {
        if(value && !usingThis)
        {
            OnInitialize();
            usingThis = true;
        }

        if (!value && usingThis && t>minContinueTime)
        {
            OnTerminate();
            usingThis = false;
        }

        ordered = value;
    }
}
