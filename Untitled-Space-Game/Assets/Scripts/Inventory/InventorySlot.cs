using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IDropHandler, IDataPersistence
{
    public int slotId;

    public Image image;
    public Color selectedColor, notSelectedColor;

    public InventoryItem itemInThisSlot;



    public float fuelLeft;

    public bool isHudSlot;
    public bool isMachineSlot;
    public bool isFuelSlot;


    private void Awake()
    {
        Deselect();
    }

    bool fuelTimeInitialized;

    public void Smelt()
    {
        if (isFuelSlot && itemInThisSlot != null)
        {
            if (itemInThisSlot.item.isFuel)
            {
                if (!fuelTimeInitialized)
                {
                    fuelLeft = itemInThisSlot.item.fuelTime;
                    fuelTimeInitialized = true;
                }
                if (fuelLeft > 0)
                {
                    fuelLeft -= Time.deltaTime;
                }
                else
                {
                    if (itemInThisSlot.count > 1)
                    {
                        itemInThisSlot.count--;
                        fuelLeft = itemInThisSlot.item.fuelTime;
                    }
                    else
                    {
                        fuelTimeInitialized = false;
                        Destroy(itemInThisSlot.gameObject);
                    }
                }
            }
            else
            {
                fuelTimeInitialized = false;
                Debug.Log("This Item Cannot Be Used As Fuel");
            }
        }
        else
        {
            fuelTimeInitialized = false;
            Debug.Log("Smelter Needs Fuel");
        }
    }

    public bool Mine()
    {
        if (isFuelSlot && itemInThisSlot != null)
        {
            if (itemInThisSlot.item.isFuel)
            {
                if (!fuelTimeInitialized)
                {
                    fuelLeft = itemInThisSlot.item.fuelTime;
                    fuelTimeInitialized = true;
                }
                if (fuelLeft > 0)
                {
                    fuelLeft -= Time.deltaTime;
                    return true;
                }
                else
                {
                    if (itemInThisSlot.count > 1)
                    {
                        itemInThisSlot.count--;
                        fuelLeft = itemInThisSlot.item.fuelTime;
                        return true;
                    }
                    else
                    {
                        fuelTimeInitialized = false;
                        Destroy(itemInThisSlot.gameObject);
                        return false;
                    }
                }
            }
            else
            {
                fuelTimeInitialized = false;
                Debug.Log("This Item Cannot Be Used As Fuel");
            }
        }
        else
        {
            fuelTimeInitialized = false;
            Debug.Log("Smelter Needs Fuel");
        }
        return false;
    }

    public void Select()
    {
        image.color = selectedColor;
    }

    public void Deselect()
    {
        image.color = notSelectedColor;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (isMachineSlot)
        {
            Debug.Log("Can't Place Items Into The Machine");
            return;
        }
        else if (isFuelSlot && !InventoryManager.Instance.heldItem.item.isFuel)
        {
            itemInThisSlot = transform.GetChild(0).GetComponent<InventoryItem>();
            Debug.Log("This Item Cannot Be Used As Fuel.");
            return;
        }
        else if (isHudSlot && !InventoryManager.Instance.heldItem.item.canBeInHudSlot)
        {
            Debug.Log("This Item Is Not Allowed In This Slot! Change This In The Inspector");
            return;
        }

        if (transform.childCount == 0)
        {
            if (InventoryManager.Instance.heldItem != null)
            {
                InventoryManager.Instance.heldItem.parentAfterDrag = transform;
                InventoryManager.Instance.heldItem.SetItemParent(transform);
                itemInThisSlot = transform.GetChild(0).GetComponent<InventoryItem>();
                InventoryManager.Instance.heldItem = null;
            }

        }
        else
        {
            itemInThisSlot = transform.GetChild(0).GetComponent<InventoryItem>();

            if (InventoryManager.Instance.heldItem.item == itemInThisSlot.item)
            {
                if (itemInThisSlot.count < itemInThisSlot.item.maxStack)
                {
                    int spaceLeft = itemInThisSlot.item.maxStack - itemInThisSlot.count;
                    int overFlow = InventoryManager.Instance.heldItem.count - spaceLeft;
                    if (overFlow > 0)
                    {
                        InventoryManager.Instance.heldItem.count = overFlow;
                        itemInThisSlot.count = itemInThisSlot.item.maxStack;
                        itemInThisSlot.RefreshCount();
                        InventoryManager.Instance.heldItem.RefreshCount();
                        return;
                    }
                    itemInThisSlot.count += InventoryManager.Instance.heldItem.count;
                    Destroy(InventoryManager.Instance.heldItem.gameObject);
                    itemInThisSlot.RefreshCount();
                }
                else
                {
                    Debug.Log("This Slot Has Reached It's Max Stack!");
                }
            }
            else
            {
                Debug.Log("You Can't Stack These Items Together!");
            }

        }
    }


    public void AddItemToSlot(InventoryItem inventoryItem)
    {
        // heldItem = eventData.pointerDrag.GetComponent<InventoryItem>();
        itemInThisSlot = transform.GetChild(0).GetComponent<InventoryItem>();
        if (InventoryManager.Instance.heldItem.item == itemInThisSlot.item)
        {
            if (itemInThisSlot.count < itemInThisSlot.item.maxStack)
            {
                itemInThisSlot.count++;
                inventoryItem.count--;
                if (inventoryItem.count <= 0)
                {
                    Destroy(inventoryItem.gameObject);
                }
                itemInThisSlot.RefreshCount();
                inventoryItem.RefreshCount();
                Debug.Log("SuccesFully Added Item To Slot: " + gameObject.name);
            }
            else
            {
                Debug.Log("This Slot Has Reached It's Max Stack!");
            }
        }
        else
        {
            Debug.Log("You Can't Stack These Items Together!");
        }
    }

    public void LoadData(GameData data)
    {
        if (data.itemId[slotId] == -1)
        {
            return;
        }

        InventoryManager.Instance.SpawnNewItem(data.itemId[slotId], data.itemAmount[slotId], this.slotId);
    }

    public void SaveData(ref GameData data)
    {
        if (itemInThisSlot == null)
        {
            data.itemId[slotId] = -1;
            data.itemAmount[slotId] = 0;
            return;
        }

        data.itemId[slotId] = itemInThisSlot.item.itemID;
        data.itemAmount[slotId] = itemInThisSlot.count;
    }
}
