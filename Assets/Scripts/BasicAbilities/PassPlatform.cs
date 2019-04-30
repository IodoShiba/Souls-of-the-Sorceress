using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassPlatform : BasicAbility
{
    enum Mode {SwitchLayer,UsePlatformContactor }
    [SerializeField] Mode mode;
    [SerializeField, LayerField] int onPasingLayer;
    [SerializeField] Collider2D platformContactor;
    [SerializeField] float minContinueTime;
    int ordinaryLayer;
    float t = 0;

    private void Awake()
    {
        ordinaryLayer = gameObject.layer;
    }

    protected override bool ShouldContinue(bool ordered) => t < minContinueTime || ordered;


    protected override void OnInitialize()
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

    protected override void OnActive(bool ordered)
    {
        t += Time.deltaTime;
        base.OnActive(ordered);
    }
    protected override void OnTerminate()
    {
        t = 0;
        gameObject.layer = ordinaryLayer;
        if(platformContactor!=null) platformContactor.enabled = true;
    }
}
