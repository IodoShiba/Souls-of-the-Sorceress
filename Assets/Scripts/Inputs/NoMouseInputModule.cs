using System;
using UnityEngine.Serialization;
using UnityEngine.EventSystems;

public class NoMouseInputModule : StandaloneInputModule
{

    public override void Process()
    {
        if (!eventSystem.isFocused)
            return;

        bool usedEvent = SendUpdateEventToSelectedObject();

        // if (!ProcessTouchEvents() && input.mousePresent)
        //     ProcessMouseEvent();                             // omit mouse input

        if (eventSystem.sendNavigationEvents)
        {
            if (!usedEvent)
                usedEvent |= SendMoveEventToSelectedObject();

            if (!usedEvent)
                SendSubmitEventToSelectedObject();
        }
    }
}