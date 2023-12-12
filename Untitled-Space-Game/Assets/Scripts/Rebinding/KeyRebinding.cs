using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System;

public class KeyRebinding : MonoBehaviour
{
    [SerializeField] CharStateMachine _charController;

    // [SerializeField] InputBinding.DisplayStringOptions _displayStringOptions;

    [SerializeField] List<Keybind> _keyboardKeybinds = new List<Keybind>();
    [SerializeField] List<Keybind> _controllerKeybinds = new List<Keybind>();

    [Serializable]
    public struct Keybind
    {
        public InputActionReference _inputActionReference;
        public int _actionIndex;
        public string _actionName;
        public bool _excludeMouse;
        public TMP_Text _actionTxt;

        public Keybind(InputActionReference inputActionReference, int actionIndex, string actionName, bool excludeMouse, TMP_Text actionTxt)
        {
            _inputActionReference = inputActionReference;
            _actionIndex = actionIndex;
            _actionName = actionName;
            _excludeMouse = excludeMouse;
            _actionTxt = actionTxt;
        }
    }

    private void OnEnable()
    {
        if (_charController != null)
        {
            for (int i = 0; i < _keyboardKeybinds.Count; i++)
            {
                KeyRebindingUI.LoadBindingOverride(_keyboardKeybinds[i]._actionName);
            }
            UpdateUI();
        }

        KeyRebindingUI.rebindComplete += UpdateUI;
        KeyRebindingUI.rebindCanceled += UpdateUI;
    }

    private void OnDisable()
    {
        KeyRebindingUI.rebindComplete -= UpdateUI;
        KeyRebindingUI.rebindCanceled -= UpdateUI;
    }

    private void OnValidate()
    {
        if (_charController == null)
        {
            return;
        }

        UpdateUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown("o"))
        {
            ResetKeyboardBindings();
        }
    }

    private void UpdateUI()
    {
        for (int i = 0; i < _keyboardKeybinds.Count; i++)
        {
            if (_keyboardKeybinds[i]._actionTxt != null)
            {
                if (Application.isPlaying)
                {
                    _keyboardKeybinds[i]._actionTxt.text = KeyRebindingUI.GetBindingName(_keyboardKeybinds[i]);
                }
                else
                {
                    _keyboardKeybinds[i]._actionTxt.text = KeyRebindingUI.GetBindingName(_keyboardKeybinds[i]);
                }

            }
        }
        for (int i = 0; i < _controllerKeybinds.Count; i++)
        {
            if (_controllerKeybinds[i]._actionTxt != null)
            {
                if (Application.isPlaying)
                {
                    _controllerKeybinds[i]._actionTxt.text = KeyRebindingUI.GetBindingName(_keyboardKeybinds[i]);
                }
                else
                {
                    _controllerKeybinds[i]._actionTxt.text = KeyRebindingUI.GetBindingName(_keyboardKeybinds[i]);
                }

            }
        }
    }


    #region Keyboard

    public void DoKeyboardRebind(int keybindIndex)
    {
        KeyRebindingUI.StartRebind(_keyboardKeybinds[keybindIndex]);
    }

    public void ResetKeyboardBindings()
    {
        for (int i = 0; i < _keyboardKeybinds.Count; i++)
        {
            KeyRebindingUI.ResetBinding(_keyboardKeybinds[i]);
        }
        UpdateUI();
    }

    public void ResetAllKeyboardBindings()
    {
        for (int i = 0; i < _keyboardKeybinds.Count; i++)
        {
            KeyRebindingUI.LoadBindingOverride(_keyboardKeybinds[i]._actionName);
        }
        UpdateUI();
    }

    public void SaveKeyboardBindings()
    {
        for (int i = 0; i < _keyboardKeybinds.Count; i++)
        {
            KeyRebindingUI.SaveBindingOverride(_keyboardKeybinds[i]._inputActionReference);
        }
    }

    #endregion

    #region Controller

    public void DoControllerRebind(int keybindIndex)
    {
        KeyRebindingUI.StartRebind(_controllerKeybinds[keybindIndex]);
    }

    public void ResetControllerBindings()
    {
        for (int i = 0; i < _controllerKeybinds.Count; i++)
        {
            KeyRebindingUI.ResetBinding(_controllerKeybinds[i]);
        }
        UpdateUI();
    }

    public void ResetAllControllerBindings()
    {
        for (int i = 0; i < _controllerKeybinds.Count; i++)
        {
            KeyRebindingUI.LoadBindingOverride(_controllerKeybinds[i]._actionName);
        }
        UpdateUI();
    }

    public void SaveControllerBindings()
    {
        for (int i = 0; i < _controllerKeybinds.Count; i++)
        {
            KeyRebindingUI.SaveBindingOverride(_controllerKeybinds[i]._inputActionReference);
        }
    }

    #endregion
}