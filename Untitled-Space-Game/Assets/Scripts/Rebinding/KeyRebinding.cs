using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class KeyRebinding : MonoBehaviour
{
    [SerializeField] CharStateMachine _charController;

    [SerializeField] InputActionReference[] _inputActionReferences; //this is on the SO

    [SerializeField] bool _excludeMouse = true;

    [SerializeField] int _selectedBindingBtn;

    [SerializeField] InputBinding.DisplayStringOptions displayStringOptions;
    [Header("Binding Info - DO NOT EDIT")]

    [SerializeField] InputBinding inputBinding;

    private int bindingIndex;

    [SerializeField] string[] _actionName;

    [Header("UI Fields")]
    // [SerializeField] TMP_Text actionText;
    [SerializeField] TMP_Text[] _rebindText;

    [SerializeField] TMP_Text[] compositeRebindText;

    // [SerializeField] Button[] rebindButton;
    // [SerializeField] Button resetButton;

    private void OnEnable()
    {
        if (_charController != null)
        {
            print(_charController);
            for (int i = 0; i < _actionName.Length; i++)
            {
                KeyRebindingUI.LoadBindingOverride(_actionName[i]);
            }
            GetBindingInfo();
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
            return;

        GetBindingInfo();
        UpdateUI();
    }

    private void GetBindingInfo()
    {
        for (int i = 0; i < _inputActionReferences.Length; i++)
        {
            if (_inputActionReferences[i].action != null)
                _actionName[i] = _inputActionReferences[i].action.name;

            if (_inputActionReferences[i].action.bindings.Count > _selectedBindingBtn)
            {
                inputBinding = _inputActionReferences[i].action.bindings[_selectedBindingBtn];
                bindingIndex = _selectedBindingBtn;
            }
        }

    }

    private void UpdateUI()
    {
        // if (actionText != null)
        //     actionText.text = actionName;

        bool allchecked = false;
        for (int i = 0; i <= _actionName.Length - 1; i++)
        {
            if (allchecked)
            {
                break;
            }
            if (i < _actionName.Length - 1)
            {
                if (_actionName[i] == _actionName[i + 1])
                {
                    int firstindex = i;

                    Debug.Log("Is composite: " + firstindex);


                    for (int j = firstindex; j < _actionName.Length - 1; j++)
                    {
                        if ((j + 1) != _actionName.Length - 1)
                        {
                            // Debug.Log(j + " " + _actionName.Length);
                            if (_actionName[firstindex] == _actionName[j + 1])
                            {
                                // TODO - do the thing
                                Debug.Log("Is part of composite: " + (j + 1) + " with: " + firstindex);

                                // if ((j + 1) == )
                            }
                            else
                            {
                                if (_actionName[j + 1] == _actionName[j + 2])
                                {
                                    int nextindex = (j + 1);
                                    firstindex = nextindex;
                                    Debug.Log("Is composite: " + (j + 1));
                                }
                                else
                                {
                                    Debug.Log("Not a composite: " + (j + 1));
                                }
                            }
                        }
                        else
                        {
                            if (_actionName[firstindex] == _actionName[j + 1])
                            {
                                Debug.Log("Is part of composite: " + (j + 1) + " with: " + firstindex + " last");
                                allchecked = true;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    Debug.Log("Not a composite");
                }
            }
            // else if (i == _actionName.Length - 1)
            // {
            //     if (_actionName[i] == _actionName[i - 1])
            //     {
            //         // TODO - do the thing
            //         Debug.Log("Is part composite: " + i);
            //     }
            //     else
            //     {
            //         Debug.Log("Not a composite: " + i + " with: " + (i + 1));
            //     }
            // }
        }


        for (int i = 0; i < _rebindText.Length; i++)
        {
            if (_rebindText != null)
            {
                if (Application.isPlaying)
                {
                    print(_actionName);
                    print(_charController);
                    print(bindingIndex + " bindingIndex");
                    _rebindText[i].text = KeyRebindingUI.GetBindingName(_actionName[i], bindingIndex);
                }
                else
                    _rebindText[i].text = _inputActionReferences[i].action.GetBindingDisplayString(bindingIndex);

            }
        }
    }

    public void DoRebind(int btnIndex)
    {
        KeyRebindingUI.StartRebind(_actionName[btnIndex], btnIndex, _rebindText[btnIndex], _excludeMouse);
    }

    public void GetBtnIndex(int btnIndex)
    {
        bindingIndex = btnIndex;
    }
    public void DoCompositeRebind(string actionName)
    {
        KeyRebindingUI.StartRebind(actionName, bindingIndex, compositeRebindText[bindingIndex], _excludeMouse);
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
}