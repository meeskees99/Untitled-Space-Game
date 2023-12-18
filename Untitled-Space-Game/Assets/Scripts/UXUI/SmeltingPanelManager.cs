using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SmeltingPanelManager : MonoBehaviour
{
    public static SmeltingPanelManager Instance;

    SmeltingMachine _currentSmelter;

    [SerializeField] PlayerInput _playerInput;

    public GameObject miningPanel;
    public InventorySlot inputSlot;
    public InventorySlot outputSlot;
    public Slider fuelLeftSlider;

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
    void Update()
    {

    }

    public void ToggleSmeltingPanel(SmeltingMachine smeltingMachine)
    {
        if (_currentSmelter == smeltingMachine)
        {
            _currentSmelter = null;
            if (inputSlot.GetInventoryItem() != null)
                Destroy(inputSlot.GetInventoryItem().gameObject);
            if (outputSlot.GetInventoryItem() != null)
                Destroy(outputSlot.GetInventoryItem().gameObject);
            miningPanel.GetComponent<Canvas>().enabled = !miningPanel.GetComponent<Canvas>().enabled;
            miningPanel.GetComponent<GraphicRaycaster>().enabled = !miningPanel.GetComponent<GraphicRaycaster>().enabled;
            panelActive = miningPanel.GetComponent<GraphicRaycaster>().enabled;
            _playerInput.currentActionMap = _playerInput.actions.FindActionMap("Game");
            Debug.Log("Changed Actionmap To " + _playerInput.currentActionMap.name);
            return;
        }
        else
        {
            _currentSmelter = smeltingMachine;
            if (inputSlot.GetInventoryItem() != null)
            {
                Debug.Log("destroy");

                Destroy(inputSlot.GetInventoryItem().gameObject);
            }
            if (outputSlot.GetInventoryItem() != null)
            {
                Debug.Log("destroy");

                Destroy(outputSlot.GetInventoryItem().gameObject);
            }

            miningPanel.GetComponent<Canvas>().enabled = !miningPanel.GetComponent<Canvas>().enabled;
            miningPanel.GetComponent<GraphicRaycaster>().enabled = !miningPanel.GetComponent<GraphicRaycaster>().enabled;
            panelActive = miningPanel.GetComponent<GraphicRaycaster>().enabled;

            if (smeltingMachine.ItemType != null && smeltingMachine.ItemAmount > 0)
            {
                // smeltingMachine.AddMachineItem(false, smeltingMachine.ItemType, smeltingMachine.ItemAmount, true);
                Debug.Log("look ma i spawned something");
            }
            if (smeltingMachine.FuelType != null && smeltingMachine.FuelAmount > 0)
            {
                // smeltingMachine.AddMachineItem(true, smeltingMachine.FuelType, smeltingMachine.FuelAmount, true);
            }


        }

        if (panelActive)
        {
            _playerInput.currentActionMap = _playerInput.actions.FindActionMap("Menu");
            Debug.Log("Changed Actionmap To " + _playerInput.currentActionMap.name);
        }
    }
}
