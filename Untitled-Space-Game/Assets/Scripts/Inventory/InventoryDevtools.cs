using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDevtools : MonoBehaviour
{
    public InventoryManager inventoryManager;

    public int stackAmount = 1;

    private void Start()
    {
        inventoryManager = InventoryManager.Instance;
    }

    public void PickupItem(int id)
    {
        bool result = inventoryManager.AddItem(id, 1);
        if (result)
        {
            Debug.Log($"{inventoryManager.GetItemById(id).name} Added");
        }
        else
        {
            Debug.Log($"Can't Add {inventoryManager.GetItemById(id).name}");
        }
    }

    public void PickUpStack(int id)
    {
        bool result = inventoryManager.AddItem(id, stackAmount);
        if (result)
        {
            Debug.Log($"{inventoryManager.GetItemById(id).name} Stack Added");
        }
        else
        {
            Debug.Log($"Can't Add {inventoryManager.GetItemById(id).name} Stack");
        }
    }

}
