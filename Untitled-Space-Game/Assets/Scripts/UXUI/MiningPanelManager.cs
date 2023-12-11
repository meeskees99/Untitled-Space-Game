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
        Destroy(itemSlot.GetInventoryItem());
        Destroy(fuelSlot.GetInventoryItem());
        if (diggingMachine.ItemSlot.GetInventoryItem() != null)
        {
            diggingMachine.AddMachineItem();
            fuelSlot.AddItemToSlot(diggingMachine.FuelSlot.GetInventoryItem());
            miningPanel.GetComponent<Canvas>().enabled = !miningPanel.GetComponent<Canvas>().enabled;
            miningPanel.GetComponent<GraphicRaycaster>().enabled = !miningPanel.GetComponent<Canvas>().enabled;
        }

    }
}
