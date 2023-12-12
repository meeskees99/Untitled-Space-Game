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
    [SerializeField] List<Keybind> _combinedKeybinds = new List<Keybind>();

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
            UpdateAllUI();
        }

        KeyRebindingUI.rebindComplete += UpdateAllUI;
        KeyRebindingUI.rebindCanceled += UpdateAllUI;
    }

    private void OnDisable()
    {
        KeyRebindingUI.rebindComplete -= UpdateAllUI;
        KeyRebindingUI.rebindCanceled -= UpdateAllUI;
    }

    private void OnValidate()
    {
        if (_charController == null)
        {
            return;
        }

        UpdateAllUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown("o"))
        {
            ResetKeyboardBindings();
        }
    }

    private void UpdateAllUI()
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
                    _controllerKeybinds[i]._actionTxt.text = KeyRebindingUI.GetBindingName(_controllerKeybinds[i]);
                }
                else
                {
                    _controllerKeybinds[i]._actionTxt.text = KeyRebindingUI.GetBindingName(_controllerKeybinds[i]);
                }

            }
        }
        for (int i = 0; i < _combinedKeybinds.Count; i++)
        {
            if (_combinedKeybinds[i]._actionTxt != null)
            {
                if (Application.isPlaying)
                {
                    _combinedKeybinds[i]._actionTxt.text = KeyRebindingUI.GetBindingName(_combinedKeybinds[i]);
                }
                else
                {
                    _combinedKeybinds[i]._actionTxt.text = KeyRebindingUI.GetBindingName(_combinedKeybinds[i]);
                }

            }
        }
    }

    private void SaveAllBindings()
    {
        SaveKeyboardBindings();
        SaveControllerBindings();
        SaveCombinedBindings();
    }

    #region Keyboard

    public void KeyboardRebindBtn(int keybindIndex)
    {
        KeyRebindingUI.StartRebind(_keyboardKeybinds[keybindIndex]);
    }

    public void ResetKeyboardBindings()
    {
        for (int i = 0; i < _keyboardKeybinds.Count; i++)
        {
            KeyRebindingUI.ResetBinding(_keyboardKeybinds[i]);
        }
        UpdateAllUI();
    }

    public void ReloadKeyboardBindings()
    {
        for (int i = 0; i < _keyboardKeybinds.Count; i++)
        {
            KeyRebindingUI.LoadBindingOverride(_keyboardKeybinds[i]._actionName);
        }
        UpdateAllUI();
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

    public void ControllerRebindBtn(int keybindIndex)
    {
        KeyRebindingUI.StartRebind(_controllerKeybinds[keybindIndex]);
    }

    public void ResetControllerBindings()
    {
        for (int i = 0; i < _controllerKeybinds.Count; i++)
        {
            KeyRebindingUI.ResetBinding(_controllerKeybinds[i]);
        }
        UpdateAllUI();
    }

    public void ReloadControllerBindings()
    {
        for (int i = 0; i < _controllerKeybinds.Count; i++)
        {
            KeyRebindingUI.LoadBindingOverride(_controllerKeybinds[i]._actionName);
        }
        UpdateAllUI();
    }

    public void SaveControllerBindings()
    {
        for (int i = 0; i < _controllerKeybinds.Count; i++)
        {
            KeyRebindingUI.SaveBindingOverride(_controllerKeybinds[i]._inputActionReference);
        }
    }

    #endregion

    #region Combined

    public void CombinedRebindBtn(int keybindIndex)
    {
        KeyRebindingUI.StartRebind(_combinedKeybinds[keybindIndex]);
    }

    public void ResetCombinedBindings()
    {
        for (int i = 0; i < _combinedKeybinds.Count; i++)
        {
            KeyRebindingUI.ResetBinding(_combinedKeybinds[i]);
        }
        UpdateAllUI();
    }

    public void ReloadCombinedBindings()
    {
        for (int i = 0; i < _combinedKeybinds.Count; i++)
        {
            KeyRebindingUI.LoadBindingOverride(_combinedKeybinds[i]._actionName);
        }
        UpdateAllUI();
    }

    public void SaveCombinedBindings()
    {
        for (int i = 0; i < _combinedKeybinds.Count; i++)
        {
            KeyRebindingUI.SaveBindingOverride(_combinedKeybinds[i]._inputActionReference);
        }
    }

    #endregion
}