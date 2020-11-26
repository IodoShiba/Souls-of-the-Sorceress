using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GuideText : MonoBehaviour
{
    static GuideText mainInstance;

    public static GuideText MainInstance { get => mainInstance; }

    [SerializeField] GameObject root;
    [SerializeField] TMP_Text instructionText;
    [SerializeField] TMP_Text descriptionText;

    public bool IsShow => root.activeSelf;

    void Awake()
    {
        mainInstance = this;
    }

    public void Show(string instruction, string description)
    {
        instructionText.text = instruction;
        descriptionText.text = description;
        root.SetActive(true);
    }

    public void Hide()
    {
        root.SetActive(false);
    }
}
