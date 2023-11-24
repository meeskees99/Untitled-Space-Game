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

    // public Item[] disAllowedItems;

    public bool isHudSlot;

    private void Awake()
    {
        Deselect();
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
        if (isHudSlot && !InventoryManager.Instance.heldItem.item.canBeInHudSlot)
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
