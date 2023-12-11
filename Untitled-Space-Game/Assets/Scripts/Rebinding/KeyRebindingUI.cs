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

    public static void StartRebind(string actionName, int bindingIndex, TMP_Text statusText, bool excludeMouse)
    {
        InputAction action = _charController.PlayerInput.actions.FindAction(actionName);
        if (action == null)
        {
            return;
        }
        if (action.bindings.Count > bindingIndex && action.bindings[bindingIndex].isComposite)
        {
            var firstPartIndex = bindingIndex + 1;
            bindingIndex += 1;

            if (firstPartIndex < action.bindings.Count && action.bindings[firstPartIndex].isPartOfComposite)
            {
                DoRebind(action, bindingIndex, statusText, true, excludeMouse);
            }
        }
        else if (action.bindings.Count > bindingIndex && action.bindings[bindingIndex].isComposite)
        {
            var firstPartIndex = bindingIndex + 1;
            bindingIndex += 1;

            if (firstPartIndex < action.bindings.Count && action.bindings[firstPartIndex].isPartOfComposite)
            {
                DoRebind(action, bindingIndex, statusText, true, excludeMouse);
            }
        }
        else if (action.bindings.Count > bindingIndex && action.bindings[bindingIndex].isPartOfComposite)
        {
            DoRebind(action, bindingIndex, statusText, false, excludeMouse);
        }
        else
        {
            DoRebind(action, 0, statusText, false, excludeMouse);
        }
    }

    private static void DoRebind(InputAction actionToRebind, int bindingIndex, TMP_Text statusText, bool allCompositeParts, bool excludeMouse)
    {
        if (actionToRebind == null || bindingIndex < 0)
        {
            return;
        }
        statusText.text = $"Press a Button";

        actionToRebind.Disable();

        var rebind = actionToRebind.PerformInteractiveRebinding(bindingIndex);

        rebind.OnComplete(operation =>
        {
            actionToRebind.Enable();
            operation.Dispose();

            if (allCompositeParts)
            {
                var nextBindingIndex = bindingIndex + 1;
                if (nextBindingIndex < actionToRebind.bindings.Count && actionToRebind.bindings[nextBindingIndex].isPartOfComposite)
                {
                    DoRebind(actionToRebind, nextBindingIndex, statusText, allCompositeParts, excludeMouse);
                }
            }

            // SaveBindingOverride(actionToRebind);
            rebindComplete?.Invoke();
        });

        rebind.OnCancel(operation =>
        {
            actionToRebind.Enable();
            operation.Dispose();

            rebindCanceled?.Invoke();
        });

        rebind.WithCancelingThrough("<Keyboard>/escape");

        if (excludeMouse)
        {
            rebind.WithControlsExcluding("Mouse");
        }

        rebindStarted?.Invoke(actionToRebind, bindingIndex);
        rebind.Start();
    }

    public static string GetBindingName(string actionName, int bindingIndex)
    {

        if (_charController == null)
        {
            _charController = FindObjectOfType<CharStateMachine>();
        }

        InputAction action = _charController.PlayerInput.actions.FindAction(actionName);
        string newBinindString = "";
        char[] newBindingChars;
        if (action.bindings[0].isComposite)
        {
            newBinindString = action.GetBindingDisplayString(0);
            newBindingChars = newBinindString.ToCharArray();
            int slashes = 0;

            string finalBinding = "";

            foreach (char c in newBindingChars)
            {
                // Debug.Log("NewBinindCount: " + newBindingChars.Length + "\nchars: " + c);
                if (c == '/')
                {
                    slashes++;
                    // Debug.Log("slashes: " + slashes);
                    continue;
                }
                Debug.Log(bindingIndex);
                if (bindingIndex == slashes)
                {
                    finalBinding += c;
                }
            }
            return finalBinding;
        }
        return action.GetBindingDisplayString(0);
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

    public static void ResetBinding(string actionName, int bindingIndex)
    {
        InputAction action = _charController.PlayerInput.actions.FindAction(actionName);

        if (action == null || action.bindings.Count <= bindingIndex)
        {
            return;
        }

        if (action.bindings[bindingIndex].isComposite)
        {
            for (int i = bindingIndex; i < action.bindings.Count && action.bindings[i].isComposite; i++)
            {
                action.RemoveBindingOverride(i);
            }
        }
        else
        {
            action.RemoveBindingOverride(bindingIndex);
        }

        SaveBindingOverride(action);
    }
}