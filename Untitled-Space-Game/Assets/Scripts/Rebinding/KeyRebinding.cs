using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class KeyRebinding : MonoBehaviour
{
    [SerializeField] CharStateMachine _charController;

    [SerializeField] InputActionReference[] _inputActionReferences;

    [SerializeField] bool _excludeMouse = true;

    [SerializeField] int _selectedBindingBtn;

    [SerializeField] InputBinding.DisplayStringOptions _displayStringOptions;

    [Header("Binding Info - DO NOT EDIT")]

    private int _bindingIndex;

    [SerializeField] string[] _actionName;

    [Header("UI Fields")]
    [SerializeField] TMP_Text[] _rebindText;

    private void OnEnable()
    {
        if (_charController != null)
        {
            for (int i = 0; i < _actionName.Length; i++)
            {
                KeyRebindingUI.LoadBindingOverride(_actionName[i]);
            }
            // GetBindingInfo();
            // UpdateUI();
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

        // GetBindingInfo();
        // UpdateUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown("o"))
        {
            UpdateUI();
        }
    }

    // private void GetBindingInfo()
    // {
    //     for (int i = 0; i < _inputActionReferences.Length; i++)
    //     {
    //         if (_inputActionReferences[i].action != null)
    //         {
    //             _actionName[i] = _inputActionReferences[i].action.name;
    //         }
    //         if (_inputActionReferences[i].action.bindings.Count > _selectedBindingBtn)
    //         {
    //             _inputBinding = _inputActionReferences[i].action.bindings[_selectedBindingBtn];
    //             _bindingIndex = _selectedBindingBtn;
    //         }
    //     }
    // }

    private void UpdateUI()
    {
        for (int i = 0; i < _rebindText.Length; i++)
        {
            if (_rebindText != null)
            {
                if (Application.isPlaying)
                {
                    _rebindText[i].text = KeyRebindingUI.GetBindingName(_actionName[i], GetCorrectBinding(i, _actionName[i]));
                }
                // else
                // {
                //     // _rebindText[i].text = _inputActionReferences[i].action.GetBindingDisplayString(_bindingIndex);
                // }

            }
        }
    }

    int GetCorrectBinding(int IncorrectBinding, string bindingName)
    {
        Debug.Log("Incorrect Binding: " + IncorrectBinding);
        InputAction action = _charController.PlayerInput.actions.FindAction(_actionName[IncorrectBinding]);

        if (action.bindings[0].isComposite)
        {
            InputAction action2 = _charController.PlayerInput.actions.FindAction(_actionName[IncorrectBinding - 1]);
            if (action2.bindings[0].isComposite)
            {

            }
        }

        // MAYBE SOMETHING LIKE THIS IDK YET ALMOST DONE THO :)

        // for (int i = 0; i < _actionName.Length; i++)
        // {
        //     if (_actionName[i])
        //         if (_actionName[i] == bindingName)
        //         {
        //             if (index == IncorrectBinding)
        //             {
        //                 index = (i - index * 2);
        //                 Debug.Log(IncorrectBinding + bindingName + index);
        //                 return index;
        //             }
        //             index++;
        //         }
        // }
        Debug.Log(IncorrectBinding + bindingName + 0);
        return 0;
    }

    public void DoRebind(int btnIndex)
    {
        KeyRebindingUI.StartRebind(_actionName[btnIndex], btnIndex, _rebindText[btnIndex], _excludeMouse);
    }

    public void GetBtnIndex(int btnIndex)
    {
        _bindingIndex = btnIndex;
    }
    public void DoCompositeRebind(string actionName)
    {
        KeyRebindingUI.StartRebind(actionName, _bindingIndex, _rebindText[GetCorrectBinding(_bindingIndex, actionName)], _excludeMouse);
    }

    public void ResetBinding(int btnIndex)
    {
        KeyRebindingUI.ResetBinding(_actionName[btnIndex], btnIndex);
        UpdateUI();
    }

    public void ResetCompositeBinding(int btnIndex, string actionName)
    {
        KeyRebindingUI.ResetBinding(actionName, btnIndex);
        UpdateUI();
    }

    public void SaveBindings()
    {
        for (int i = 0; i < _inputActionReferences.Length; i++)
        {
            KeyRebindingUI.SaveBindingOverride(_inputActionReferences[i]);
        }
    }
}