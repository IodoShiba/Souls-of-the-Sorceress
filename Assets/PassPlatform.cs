using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassPlatform : BasicAbility
{
    enum Mode {SwitchLayer,UsePlatformContactor }
    [SerializeField] Mode mode;
    [SerializeField, LayerField] int onPasingLayer;
    [SerializeField] Collider2D platformContactor;
    int ordinaryLayer;

    private void Awake()
    {
        ordinaryLayer = gameObject.layer;
    }

    protected override bool ShouldContinue(bool ordered) => ordered;


    protected override void OnInitialize()
    {
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

    protected override void OnTerminate()
    {
        gameObject.layer = ordinaryLayer;
        if(platformContactor!=null) platformContactor.enabled = true;
    }
}
