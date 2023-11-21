using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDevtools : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public Item[] itemToPickup;

    private void Start()
    {
        inventoryManager = InventoryManager.Instance;
    }

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

    public void GetSelectedItem()
    {
        Item currentItem = inventoryManager.GetSelectedItem(false);
        if (currentItem != null)
        {
            print($"Currently Holding {currentItem.name}");
        }
        else
        {
            Debug.Log("No Item Being Held");
        }
    }

    public void UseSelectedItem()
    {
        Item currentItem = inventoryManager.GetSelectedItem(true);
        if (currentItem != null)
        {
            print($"Used Item: {currentItem.name}");
        }
        else
        {
            Debug.Log("No Item To Use!");
        }
    }
}
