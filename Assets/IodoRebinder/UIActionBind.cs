using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UIActionBind : MonoBehaviour
{
    // User use
    [SerializeField] InputActionReference action;

    // Dependencies
    [SerializeField] TMPro.TMP_Text _actionText;
    [SerializeField] Button _bindingButton;

    TMPro.TMP_Text _bindingButtonText;

    // Properties
    public string actionTextContent { get => _actionText.text; set => _actionText.text = value; }
    public string bindingButtonTextContent { get => _bindingButtonText.text; set => _bindingButtonText.text = value; }

    void _BindButtonPressed()
    {
        // action.action.PerformInteractiveRebinding();
    }

    

    // Unity event functions
    void Start()
    {
        _bindingButtonText = _bindingButton.GetComponentInChildren<TMPro.TMP_Text>();
        _bindingButton.onClick.AddListener(()=>_BindButtonPressed());
    }

    void Update()
    {
        
    }

}
