using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SmeltingPanelManager : MonoBehaviour
{
    public static SmeltingPanelManager Instance;

    public SmeltingMachine currentSmelter;

    [SerializeField] PlayerInput _playerInput;

    public InventorySlot fuelInputSlot;
    public InventorySlot resourceInputSlot;
    public InventorySlot outputSlot;

    public Slider fuelLeftSlider;
    public Slider progressSlider;

    public bool panelActive;
    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Found Multiple SmeltingPanelManagers! Destroyed The One On " + gameObject.name);
            Destroy(this);
        }
    }

    // Update is called once per frame
    public void SetSmelterInfo(SmeltingMachine smeltingMachine)
    {
        smeltingMachine.ResourceInputSlot = resourceInputSlot;
        smeltingMachine.FuelInputSlot = fuelInputSlot;
        smeltingMachine.ItemOutputSlot = outputSlot;

        smeltingMachine.fuelLeftSlider = fuelLeftSlider;
        smeltingMachine.progressSlider = progressSlider;
    }

    public void ToggleSmeltingPanel(SmeltingMachine smeltingMachine)
    {
        if (fuelInputSlot.GetInventoryItem() != null)
            Destroy(fuelInputSlot.GetInventoryItem().gameObject);
        if (outputSlot.GetInventoryItem() != null)
            Destroy(outputSlot.GetInventoryItem().gameObject);
        if (resourceInputSlot.GetInventoryItem() != null)
            Destroy(resourceInputSlot.GetInventoryItem().gameObject);

        if (currentSmelter == smeltingMachine)
        {
            currentSmelter = null;
            GetComponent<Canvas>().enabled = !GetComponent<Canvas>().enabled;
            GetComponent<GraphicRaycaster>().enabled = !GetComponent<GraphicRaycaster>().enabled;
            panelActive = GetComponent<GraphicRaycaster>().enabled;
            _playerInput.currentActionMap = _playerInput.actions.FindActionMap("Game");
            Debug.Log("Changed Actionmap To " + _playerInput.currentActionMap.name);
            return;
        }
        else
        {
            currentSmelter = smeltingMachine;

            SetSmelterInfo(smeltingMachine);

            GetComponent<Canvas>().enabled = !GetComponent<Canvas>().enabled;
            GetComponent<GraphicRaycaster>().enabled = !GetComponent<GraphicRaycaster>().enabled;
            panelActive = GetComponent<GraphicRaycaster>().enabled;

            if (smeltingMachine.ResourceType != null && smeltingMachine.ResourceAmount > 0)
            {
                smeltingMachine.AddMachineItem(resourceInputSlot, smeltingMachine.ResourceType, smeltingMachine.ResourceAmount, true);
                // Debug.Log("look ma i spawned something");
            }
            if (smeltingMachine.FuelType != null && smeltingMachine.FuelAmount > 0)
            {
                smeltingMachine.AddMachineItem(fuelInputSlot, smeltingMachine.FuelType, smeltingMachine.FuelAmount, true);
            }
            if (smeltingMachine.OutputType != null && smeltingMachine.OutputAmount > 0)
            {
                smeltingMachine.AddMachineItem(outputSlot, smeltingMachine.OutputType, smeltingMachine.OutputAmount, true);
            }
        }

        if (panelActive)
        {
            _playerInput.currentActionMap = _playerInput.actions.FindActionMap("Menu");
            Debug.Log("Changed Actionmap To " + _playerInput.currentActionMap.name);
        }
    }
}
