using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
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
            print("null");
            _charController = FindObjectOfType<CharStateMachine>();
            print(_charController);
        }
    }
    private void Awake()
    {
        if (_charController == null)
        {
            print(_charController);
            _charController = FindObjectOfType<CharStateMachine>();
            print(_charController);
        }
        if (inputActions == null)
            inputActions = new();
    }

    public static void StartRebind(string actionName, int bindingIndex, TMP_Text statusText, bool excludeMouse)
    {
        Debug.Log("hoi");
        print(_charController);
        InputAction action = _charController.PlayerInput.actions.FindAction(actionName);
        if (action == null)
        {
            Debug.Log(action);
            print((action.bindings.Count <= bindingIndex) + " Bool " + bindingIndex.ToString() + " Index " + action.bindings.Count + " Count");

            Debug.Log("Couldn't find action or binding");
            return;
        }

        if (action.bindings[bindingIndex + 1].isPartOfComposite)
        {
            Debug.Log("hui");
            var firstPartIndex = bindingIndex + 1;
            bindingIndex += 1;
            //print(bindingIndex);
            //print(firstPartIndex);

            //print(action.bindings[bindingIndex].isComposite);
            //print(action.bindings[firstPartIndex].isPartOfComposite);

            if (firstPartIndex < action.bindings.Count && action.bindings[firstPartIndex].isPartOfComposite)
            {
                DoRebind(action, bindingIndex, statusText, true, excludeMouse);
                Debug.Log("hii");
            }
        }
        else
        {
            print(bindingIndex);
            DoRebind(action, bindingIndex, statusText, false, excludeMouse);
        }
    }

    private static void DoRebind(InputAction actionToRebind, int bindingIndex, TMP_Text statusText, bool allCompositeParts, bool excludeMouse)
    {
        print(allCompositeParts + " Composite Parts");
        if (actionToRebind == null || bindingIndex < 0)
        {
            print("returns");
            return;
        }
        statusText.text = $"Press a {actionToRebind.expectedControlType}";

        actionToRebind.Disable();
        print("0");

        var rebind = actionToRebind.PerformInteractiveRebinding(bindingIndex);
        print(rebind);

        rebind.OnComplete(operation =>
        {
            print("1");

            actionToRebind.Enable();
            operation.Dispose();

            if (allCompositeParts)
            {
                Debug.Log("DO REBIND AGAIN");
                var nextBindingIndex = bindingIndex + 1;
                if (nextBindingIndex < actionToRebind.bindings.Count && actionToRebind.bindings[nextBindingIndex].isPartOfComposite)
                {
                    DoRebind(actionToRebind, nextBindingIndex, statusText, allCompositeParts, excludeMouse);
                }
            }

            SaveBindingOverride(actionToRebind);
            rebindComplete?.Invoke();
            print("2");
        });

        rebind.OnCancel(operation =>
        {
            actionToRebind.Enable();
            operation.Dispose();

            rebindCanceled?.Invoke();
        });

        rebind.WithCancelingThrough("<Keyboard>/escape");

        if (excludeMouse)
            rebind.WithControlsExcluding("Mouse");

        print(actionToRebind + bindingIndex.ToString());
        rebindStarted?.Invoke(actionToRebind, bindingIndex);
        rebind.Start(); //actually starts the rebinding process
        print("3");

    }

    public static string GetBindingName(string actionName, int bindingIndex)
    {
        if (_charController == null)
            _charController = FindObjectOfType<CharStateMachine>();

        print(actionName);
        InputAction action = _charController.PlayerInput.actions.FindAction(actionName);
        return action.GetBindingDisplayString(bindingIndex);
    }

    private static void SaveBindingOverride(InputAction action)
    {
        for (int i = 0; i < action.bindings.Count; i++)
        {
            PlayerPrefs.SetString(action.actionMap + action.name + i, action.bindings[i].overridePath);
        }
    }

    public static void LoadBindingOverride(string actionName)
    {
        if (inputActions == null)
            inputActions = new NewControls();

        InputAction action = _charController.PlayerInput.actions.FindAction(actionName);

        for (int i = 0; i < action.bindings.Count; i++)
        {
            if (!string.IsNullOrEmpty(PlayerPrefs.GetString(action.actionMap + action.name + i)))
                action.ApplyBindingOverride(i, PlayerPrefs.GetString(action.actionMap + action.name + i));
        }
    }

    public static void ResetBinding(string actionName, int bindingIndex)
    {
        InputAction action = _charController.PlayerInput.actions.FindAction(actionName);

        if (action == null || action.bindings.Count <= bindingIndex)
        {
            Debug.Log("Could not find action or binding");
            return;
        }

        if (action.bindings[bindingIndex].isComposite)
        {
            for (int i = bindingIndex; i < action.bindings.Count && action.bindings[i].isComposite; i++)
                action.RemoveBindingOverride(i);
        }
        else
            action.RemoveBindingOverride(bindingIndex);

        SaveBindingOverride(action);
    }

}