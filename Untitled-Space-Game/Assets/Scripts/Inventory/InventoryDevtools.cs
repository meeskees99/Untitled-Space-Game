using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDevtools : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public Item[] itemToPickup;

    public void PickupItem(int id)
    {
        bool result = inventoryManager.AddItem(itemToPickup[id]);
        if (result)
        {
            Debug.Log("Item Added");
        }
        else
        {
            Debug.Log("Can't Add Item");
        }
    }
}
