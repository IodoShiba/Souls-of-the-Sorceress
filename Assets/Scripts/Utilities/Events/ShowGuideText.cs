using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowGuideText : MonoBehaviour
{
    [SerializeField, TextArea] string instruction;
    [SerializeField, TextArea] string description;

    public void Show()
    {
        GuideText.MainInstance.Show(instruction, description);
    }

    public void Hide()
    {
        GuideText.MainInstance.Hide();
    }
}
