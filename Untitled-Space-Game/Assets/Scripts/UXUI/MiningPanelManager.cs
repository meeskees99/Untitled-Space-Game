using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiningPanelManager : MonoBehaviour
{
    public static MiningPanelManager Instance;

    public GameObject miningPanel;
    public InventorySlot itemSlot;
    public InventorySlot fuelSlot;
    public Slider fuelLeftSlider;

    public bool panelActive;

    public DiggingMachine currentDigger;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void SetDiggerInfo(DiggingMachine diggingMachine)
    {
        diggingMachine.ItemSlot = itemSlot;
        diggingMachine.FuelSlot = fuelSlot;
        diggingMachine.fuelLeftSlider = fuelLeftSlider;
    }

    public void ToggleMiningPanel(DiggingMachine diggingMachine)
    {
        if (currentDigger == diggingMachine)
        {
            currentDigger = null;
            if (itemSlot.GetInventoryItem() != null)
                Destroy(itemSlot.GetInventoryItem().gameObject);
            if (fuelSlot.GetInventoryItem() != null)
                Destroy(fuelSlot.GetInventoryItem().gameObject);
            miningPanel.GetComponent<Canvas>().enabled = !miningPanel.GetComponent<Canvas>().enabled;
            miningPanel.GetComponent<GraphicRaycaster>().enabled = !miningPanel.GetComponent<GraphicRaycaster>().enabled;
            panelActive = miningPanel.GetComponent<GraphicRaycaster>().enabled;
            return;
        }
        else
        {
            currentDigger = diggingMachine;
            if (itemSlot.GetInventoryItem() != null)
            {
                Debug.Log("destroy");

                Destroy(itemSlot.GetInventoryItem().gameObject);
            }
            if (fuelSlot.GetInventoryItem() != null)
            {
                Debug.Log("destroy");

                Destroy(fuelSlot.GetInventoryItem().gameObject);
            }

            miningPanel.GetComponent<Canvas>().enabled = !miningPanel.GetComponent<Canvas>().enabled;
            miningPanel.GetComponent<GraphicRaycaster>().enabled = !miningPanel.GetComponent<GraphicRaycaster>().enabled;
            panelActive = miningPanel.GetComponent<GraphicRaycaster>().enabled;

            if (diggingMachine.ItemType != null && diggingMachine.ItemAmount > 0)
            {
                diggingMachine.AddMachineItem(false, diggingMachine.ItemType, diggingMachine.ItemAmount, true);
                Debug.Log("look ma i spawned something");
            }
            if (diggingMachine.FuelType != null && diggingMachine.FuelAmount > 0)
            {
                diggingMachine.AddMachineItem(true, diggingMachine.FuelType, diggingMachine.FuelAmount, true);
            }


        }

    }
}
