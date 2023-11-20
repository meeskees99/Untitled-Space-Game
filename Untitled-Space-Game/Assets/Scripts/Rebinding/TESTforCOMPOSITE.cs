using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
public class TESTforCOMPOSITE : MonoBehaviour
{
    [SerializeField] CharStateMachine _charController;

    [SerializeField] InputActionReference inputActionReference; //this is on the SO

    [SerializeField] bool excludeMouse = true;

    [SerializeField] int selectedBindingBtn;

    [SerializeField] InputBinding.DisplayStringOptions displayStringOptions;
    [Header("Binding Info - DO NOT EDIT")]

    [SerializeField] InputBinding inputBinding;

    private int bindingIndex;

    private string actionName;

    [Header("UI Fields")]
    [SerializeField] TMP_Text actionText;
    [SerializeField] Button rebindButton;
    [SerializeField] TMP_Text rebindText;
    [SerializeField] Button resetButton;

    private void OnEnable()
    {
        if (_charController != null)
        {
            print(_charController);
            TESTINPUTforCOMPOSITE.LoadBindingOverride(actionName);
            GetBindingInfo();
            UpdateUI();
        }

        TESTINPUTforCOMPOSITE.rebindComplete += UpdateUI;
        TESTINPUTforCOMPOSITE.rebindCanceled += UpdateUI;
    }

    private void OnDisable()
    {
        TESTINPUTforCOMPOSITE.rebindComplete -= UpdateUI;
        TESTINPUTforCOMPOSITE.rebindCanceled -= UpdateUI;
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
        if (inputActionReference.action != null)
            actionName = inputActionReference.action.name;

        if (inputActionReference.action.bindings.Count > selectedBindingBtn)
        {
            inputBinding = inputActionReference.action.bindings[selectedBindingBtn];
            bindingIndex = selectedBindingBtn;
        }
    }

    private void UpdateUI()
    {
        if (actionText != null)
            actionText.text = actionName;

        if (rebindText != null)
        {
            if (Application.isPlaying)
            {
                print(actionName);
                print(_charController);
                print(bindingIndex + " bindingIndex");
                rebindText.text = TESTINPUTforCOMPOSITE.GetBindingName(actionName, bindingIndex);
            }
            else
                rebindText.text = inputActionReference.action.GetBindingDisplayString(bindingIndex);
        }
    }

    public void DoRebind(int btnIndex)
    {
        Debug.Log("hai");
        // btnIndex++;
        TESTINPUTforCOMPOSITE.StartRebind(actionName, btnIndex, rebindText, excludeMouse);
    }

    private void ResetBinding(int btnIndex)
    {
        TESTINPUTforCOMPOSITE.ResetBinding(actionName, btnIndex);
        UpdateUI();
    }
}