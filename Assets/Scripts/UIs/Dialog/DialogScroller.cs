using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DialogScroller : MonoBehaviour
{
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] float maxSpeed = 1.5f;

    void Update()
    {
        // float input = Input.GetAxisRaw("Vertical");
        float input = InputDaemon.GetVector2("Navigate").y;

        if(!scrollRect.verticalScrollbar.IsInteractable())
        {
            return;
        }

        _ = Scroll(maxSpeed*input);
    }

    public float Scroll(float speed) 
    {
        Scrollbar vscroll = scrollRect.verticalScrollbar;
        vscroll.value = Mathf.Clamp01(vscroll.value+vscroll.size*speed*Time.deltaTime);
        return vscroll.value;
    }
}
