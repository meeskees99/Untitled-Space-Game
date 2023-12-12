using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class KeyRebindingUI : MonoBehaviour
{
    private static CharStateMachine _charController;
    public static NewControls inputActions;
    public static event Action rebindComplete;
    public static event Action rebindCanceled;
    public static event Action<InputAction, int> rebindStarted;

    private void OnEnable()
    {
        if (_charController == null)
        {
            _charController = FindObjectOfType<CharStateMachine>();
        }
    }
    private void Awake()
    {
        if (_charController == null)
        {
            _charController = FindObjectOfType<CharStateMachine>();
        }
        if (inputActions == null)
        {
            inputActions = new();
        }
    }

    public static void StartRebind(KeyRebinding.Keybind keybind)
    {
        Debug.Log(keybind._actionName + " " + keybind._actionIndex);
        InputAction action = _charController.PlayerInput.actions.FindAction(keybind._actionName);



        if (action == null)
        {
            return;
        }
        if (action.bindings.Count > keybind._actionIndex && action.bindings[keybind._actionIndex].isComposite)
        {
            Debug.Log("IsComposite");
            var firstPartIndex = keybind._actionIndex + 1;

            if (firstPartIndex < action.bindings.Count && action.bindings[firstPartIndex].isPartOfComposite)
            {
                Debug.Log("DoExtraRebind");
                DoRebind(action, keybind, firstPartIndex, true);
            }
        }
        // else if (action.bindings.Count > keybind._actionIndex && action.bindings[keybind._actionIndex].isComposite)
        // {
        //     var firstPartIndex = keybind._actionIndex + 1;
        //     keybind._actionIndex += 1;

        //     if (firstPartIndex < action.bindings.Count && action.bindings[firstPartIndex].isPartOfComposite)
        //     {
        //         DoRebind(action, keybind, keybind._actionIndex, true);
        //     }
        // }
        else if (action.bindings.Count > keybind._actionIndex && action.bindings[keybind._actionIndex].isPartOfComposite)
        {
            DoRebind(action, keybind, keybind._actionIndex, false);
        }
        else
        {
            DoRebind(action, keybind, keybind._actionIndex, false);
        }
    }

    private static void DoRebind(InputAction actionToRebind, KeyRebinding.Keybind keybind, int bindingIndex, bool allCompositeParts)
    {
        if (actionToRebind == null || keybind._actionIndex < 0)
        {
            return;
        }
        keybind._actionTxt.text = $"Press a Button";

        actionToRebind.Disable();

        var rebind = actionToRebind.PerformInteractiveRebinding(bindingIndex);

        rebind.WithBindingGroup("Gamepad");

        rebind.OnComplete(operation =>
        {
            actionToRebind.Enable();
            operation.Dispose();

            if (allCompositeParts)
            {
                var nextBindingIndex = bindingIndex + 1;
                if (nextBindingIndex < actionToRebind.bindings.Count && actionToRebind.bindings[nextBindingIndex].isPartOfComposite)
                {
                    DoRebind(actionToRebind, keybind, nextBindingIndex, allCompositeParts);
                }
            }

            rebindComplete?.Invoke();
        });

        rebind.OnCancel(operation =>
        {
            actionToRebind.Enable();
            operation.Dispose();

            rebindCanceled?.Invoke();
        });

        rebind.WithCancelingThrough("<Keyboard>/escape");
        if (keybind._excludeMouse)
        {
            rebind.WithControlsExcluding("Mouse");
        }

        rebindStarted?.Invoke(actionToRebind, bindingIndex);
        rebind.Start();
    }

    public static string GetBindingName(KeyRebinding.Keybind keybind)
    {

        if (_charController == null)
        {
            _charController = FindObjectOfType<CharStateMachine>();
        }

        InputAction action = _charController.PlayerInput.actions.FindAction(keybind._actionName);

        if (action.bindings[keybind._actionIndex].isComposite)
        {
            // Debug.Log("IsComposite: " + keybind._actionName);
            return action.GetBindingDisplayString(0);
        }
        else if (action.bindings[keybind._actionIndex].isPartOfComposite)
        {
            // Debug.Log("IsPartOfComposite");
            return GetPartOfCompositeButton(keybind._actionIndex, action);
        }
        // Debug.Log(keybind._actionName + " " + keybind._actionIndex);
        return action.GetBindingDisplayString(keybind._actionIndex);
    }

    public static string GetPartOfCompositeButton(int compositePartIndex, InputAction action)
    {
        char[] unModifiedBindingChars = action.GetBindingDisplayString(0).ToCharArray();

        int index = 1;

        string modifiedBindingString = "";

        foreach (char c in unModifiedBindingChars)
        {
            if (c != '/')
            {
                if (index == compositePartIndex)
                {
                    modifiedBindingString += c;
                }
                else if (index > compositePartIndex)
                {
                    break;
                }
            }
            else
            {
                index++;
            }
        }

        return modifiedBindingString;
    }

    public static void SaveBindingOverride(InputAction action)
    {
        for (int i = 0; i < action.bindings.Count; i++)
        {
            PlayerPrefs.SetString(action.actionMap + action.name + i, action.bindings[i].overridePath);
        }
    }

    public static void LoadBindingOverride(string actionName)
    {
        if (inputActions == null)
        {
            inputActions = new NewControls();
        }

        InputAction action = _charController.PlayerInput.actions.FindAction(actionName);

        for (int i = 0; i < action.bindings.Count; i++)
        {
            if (!string.IsNullOrEmpty(PlayerPrefs.GetString(action.actionMap + action.name + i)))
            {
                action.ApplyBindingOverride(i, PlayerPrefs.GetString(action.actionMap + action.name + i));
            }
        }
    }

    public static void ResetBinding(KeyRebinding.Keybind keybind)
    {
        InputAction action = _charController.PlayerInput.actions.FindAction(keybind._actionName);

        if (action == null || action.bindings.Count <= keybind._actionIndex)
        {
            return;
        }

        if (action.bindings[keybind._actionIndex].isComposite)
        {
            for (int i = keybind._actionIndex; i < action.bindings.Count && action.bindings[i].isComposite; i++)
            {
                action.RemoveBindingOverride(i);
            }
        }
        else
        {
            action.RemoveBindingOverride(keybind._actionIndex);
        }

        SaveBindingOverride(action);
    }
}