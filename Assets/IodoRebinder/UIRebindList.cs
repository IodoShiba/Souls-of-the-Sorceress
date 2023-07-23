using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Samples.RebindUI;
using UnityEngine.Events;

namespace IodoShiba.Rebinder
{
    [System.Serializable]
    struct ActionAliasPair
    {
        public InputActionReference inputAction;
        public string alias;
    }

    struct BindKey
    { 
        public InputActionMap map; public string path;

        public override int GetHashCode()
        {
            return map.GetHashCode() ^ path.GetHashCode();
        }
    }

    [System.Serializable]
    public class UnityEventDuplicationUpdated : UnityEvent<bool> { }

    [System.Serializable]
    public class UnityEventRebindUIObjectAdded : UnityEvent<RebindActionUI> { }

    public class UIRebindList : MonoBehaviour
    {
        // Options
        [SerializeField] string targetControl;
        string TargetControl
        {
            get => targetControl;
            set
            {
                if(!allowTargetControlChange) { return; }
                if(targetControl == value) { return; }

                // Debug.Log($"Control changed: {value}");

                targetControl = value;
                ResetBindingIds();

                targetControlChangedEvent.Invoke(value);
            }
        }

        const string STR_KEYBOARD = "Keyboard";
        const string STR_GAMEPAD = "Gamepad";

        [SerializeField] int uiCacheSize = 32;
        [SerializeField] TargetControlChangedEvent targetControlChangedEvent;
        [SerializeField] UnityEventRebindUIObjectAdded uiAdded;
        [SerializeField] UnityEventDuplicationUpdated duplicationUpdated;
        [SerializeField] RebindActionUI.InteractiveRebindEvent anyStartRebind;
        [SerializeField] RebindActionUI.InteractiveRebindEvent anyStopRebind;
        [SerializeField] ActionAliasPair[] actionAliases;
        [SerializeField] Color colorDuplicationWarning;

        // Dependencies
        [SerializeField] InputActionAsset inputActionAsset;
        [SerializeField] Button buttonReset;
        [SerializeField] RebindActionUI prefabRebindActionUI;
        [SerializeField] UnityEngine.EventSystems.EventSystem eventSystem;
        [SerializeField] UnityEngine.InputSystem.UI.InputSystemUIInputModule inputModule;

        // Caches
        RebindActionUI[] rebindUis;
        InputAction[] actions;
        Dictionary<BindKey, int> boundIndexes;

        // States
        int actionCounts = 0;
        bool allowTargetControlChange = true;

        void OnMoveAction(InputAction.CallbackContext cc)
        {
            var value = cc.ReadValue<Vector2>();
            var dead = InputSystem.settings.defaultDeadzoneMin;
            
            if (value.sqrMagnitude >= dead*dead)
            {
                var device = cc.control.device;
                if (device is Gamepad && targetControl != STR_GAMEPAD)
                {
                    TargetControl = STR_GAMEPAD;
                }
                else if(device is Keyboard && targetControl != STR_KEYBOARD)
                {
                    TargetControl = STR_KEYBOARD;
                }
                else
                {
                    // do nothing
                }
            }
        }
        void OnEnable()
        {
            // InputSystem.onEvent += OnInputSystemEvent;
            inputModule.move.action.performed += OnMoveAction;
        }
        void OnDisable()
        {
            // InputSystem.onEvent -= OnInputSystemEvent;
            inputModule.move.action.performed -= OnMoveAction;
            FindDuplication();
        }

        void Start()
        {
            buttonReset.onClick.AddListener(()=>ResetBindingsToDefault());

            rebindUis = new RebindActionUI[uiCacheSize];
            actions = new InputAction[uiCacheSize];
            boundIndexes = new Dictionary<BindKey, int>(uiCacheSize);

            rebindUis[0] = transform.GetChild(0).GetComponentInChildren<RebindActionUI>();

            actionCounts = 0;
            foreach(var act in inputActionAsset)
            {
                // Debug.Log($"expected control tyoe {act.expectedControlType} active control {act.activeControl}");
                // for (var i = 0; i < act.bindings.Count; ++i)
                // {
                //     Debug.Log($"{act.bindings[i].path} {act.bindings[i].id.ToString()}");
                // }

                RebindActionUI ui = actionCounts == 0 ?
                    rebindUis[actionCounts] :
                    Instantiate(prefabRebindActionUI, Vector3.zero, Quaternion.identity, transform);
                
                var actref = new InputActionReference();
                actref.Set(act);
                ui.actionReference = actref;

                actions[actionCounts] = act;
                rebindUis[actionCounts++] = ui;


                uiAdded.Invoke(ui);
            }

            ResetBindingIds();
            
            // Connect neighbor button navigation 
            for(int i=0; i<actionCounts; ++i)
            {
                Button buttonSelf = rebindUis[i].GetComponentInChildren<Button>();
                Button buttonPrev = rebindUis[(i + actionCounts - 1)%actionCounts].GetComponentInChildren<Button>();
                rebindUis[i].startRebindEvent.AddListener(
                    (ui, op)=>
                    {
                        allowTargetControlChange = false;
                        anyStartRebind.Invoke(ui, op);
                    }
                    );
                rebindUis[i].stopRebindEvent.AddListener(
                    (ui, op)=>
                    {
                        allowTargetControlChange = true;
                        anyStopRebind.Invoke(ui, op);
                    }
                    );
                rebindUis[i].stopRebindEvent.AddListener((ui, op) => FindDuplication(ui, op));

                buttonPrev.navigation = CreateNavigation(buttonPrev.navigation, dn: buttonSelf, r: buttonReset);
                buttonSelf.navigation = CreateNavigation(buttonSelf.navigation, up: buttonPrev, r: buttonReset);
            }
        }



        void ResetBindingsToDefault()
        {
            for (int i=0; i<actionCounts; ++i)
            {
                rebindUis[i].ResetToDefault();
            }
        }

        void ResetBindingIds()
        {
            for(int i=0; i<actionCounts; ++i)
            {
                var act = actions[i];
                int bindingIndex = GetBindingIndex(act);
                if (bindingIndex == -1) { continue; }

                rebindUis[i].bindingId = actions[i].bindings[bindingIndex].id.ToString();
                var alias = actions[i].name;
                for (int j=0; j<actionAliases.Length; ++j)
                {
                    if (actionAliases[j].inputAction.action == actions[i]) { alias = actionAliases[j].alias; break; }
                }
                rebindUis[i].actionLabel.text = alias;
            }
        }

        int GetBindingIndex(InputAction action)
        {
            bool PredicateGamepad(InputBinding b)
            {
                return b.path.Contains("Gamepad");
            }

            bool PredicateKeyboard(InputBinding b)
            {
                if (action.type == InputActionType.Value)
                {
                    return 
                        (action.expectedControlType == "Vector2" || action.expectedControlType == "Stick") && 
                        b.isComposite && b.path == "2DVector";
                }
                else if(action.type == InputActionType.Button)
                {
                    return b.path.Contains("Keyboard");
                }
                else
                {
                    return false;
                }
            }

            return action.bindings.IndexOf(b=>
                targetControl == "Keyboard" ? PredicateKeyboard(b) : 
                targetControl == "Gamepad" ? PredicateGamepad(b) : 
                false
            );
        }

        Navigation CreateNavigation(in Navigation original, Selectable up = null, Selectable dn = null, Selectable l = null, Selectable r = null)
        {
            Navigation navi = new Navigation();
            navi.mode = Navigation.Mode.Explicit;
            navi.selectOnUp = up != null ? up : original.selectOnUp;
            navi.selectOnDown = dn != null ? dn : original.selectOnDown;
            navi.selectOnLeft = l != null ? l : original.selectOnLeft;
            navi.selectOnRight = r != null ? r : original.selectOnRight;
            return navi;
        }

        void FindDuplication(RebindActionUI ui, InputActionRebindingExtensions.RebindingOperation op)
        {
            FindDuplication();
        }
        void FindDuplication()
        {
            for (int i = 0; i < actionCounts; ++i)
            {
                rebindUis[i].bindingText.color = Color.white;
            }

            bool duplicationExist = false;
            boundIndexes.Clear();
            for (int i = 0; i < actionCounts; ++i)
            {
                var act = actions[i];
                int bindingIndex = GetBindingIndex(act);
                if (bindingIndex == -1) { continue; }
                string path = rebindUis[i].bindingText.text; //act.bindings[bindingIndex].path;
                int mapIdx = 0;
                var actionMaps = inputActionAsset.actionMaps;
                for (;mapIdx < inputActionAsset.actionMaps.Count; ++mapIdx) { if (actionMaps[mapIdx] == act.actionMap) { break; } }
                int keyIdx = i;
                BindKey bindKey; bindKey.map = actionMaps[mapIdx]; bindKey.path = path;
                if(boundIndexes.ContainsKey(bindKey))
                {
                    OnDuplicationFound(boundIndexes[bindKey], keyIdx);
                    duplicationExist = true;
                }

                boundIndexes[bindKey] = keyIdx;
            }

            duplicationUpdated.Invoke(duplicationExist);
        }
        void OnDuplicationFound(int idxA, int idxB) 
        {
            if (rebindUis[idxA].bindingText != null) { rebindUis[idxA].bindingText.color = colorDuplicationWarning; }
            if (rebindUis[idxB].bindingText != null) { rebindUis[idxB].bindingText.color = colorDuplicationWarning; }
        }

        [System.Serializable]
        class TargetControlChangedEvent : UnityEvent<string> {}
    }
}