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

    [SerializeField] List<Keybind> _keybinds = new List<Keybind>();

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
            for (int i = 0; i < _keybinds.Count; i++)
            {
                KeyRebindingUI.LoadBindingOverride(_keybinds[i]._actionName);
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
            ResetBinding();
        }
    }

    private void UpdateUI()
    {
        for (int i = 0; i < _keybinds.Count; i++)
        {
            if (_keybinds[i]._actionTxt != null)
            {
                if (Application.isPlaying)
                {
                    _keybinds[i]._actionTxt.text = KeyRebindingUI.GetBindingName(_keybinds[i]);
                }
                else
                {
                    _keybinds[i]._actionTxt.text = KeyRebindingUI.GetBindingName(_keybinds[i]);
                }

            }
        }
    }

    public void DoRebind(int keybindIndex)
    {
        KeyRebindingUI.StartRebind(_keybinds[keybindIndex]);
    }

    public void ResetBinding()
    {
        for (int i = 0; i < _keybinds.Count; i++)
        {
            KeyRebindingUI.ResetBinding(_keybinds[i]);
        }
        UpdateUI();
    }

    public void ResetAllBindings()
    {
        for (int i = 0; i < _keybinds.Count; i++)
        {
            KeyRebindingUI.LoadBindingOverride(_keybinds[i]._actionName);
        }
        UpdateUI();
    }

    public void SaveBindings()
    {
        for (int i = 0; i < _keybinds.Count; i++)
        {
            KeyRebindingUI.SaveBindingOverride(_keybinds[i]._inputActionReference);
        }
    }
}