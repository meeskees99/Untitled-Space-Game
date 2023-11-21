using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IDropHandler, IDragHandler
{
    public Image image;
    public Color selectedColor, notSelectedColor;

    InventoryItem heldItem;
    InventoryItem itemInThisSlot;

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
        heldItem = eventData.pointerDrag.GetComponent<InventoryItem>();

        if (transform.childCount == 0)
        {
            if (heldItem != null)
                heldItem.parentAfterDrag = transform;
        }
        else
        {
            itemInThisSlot = transform.GetChild(0).GetComponent<InventoryItem>();
            if (itemInThisSlot.count < itemInThisSlot.item.maxStack)
            {
                if (heldItem.item == itemInThisSlot.item)
                {
                    int spaceLeft = itemInThisSlot.item.maxStack - itemInThisSlot.count;
                    int overFlow = heldItem.count - spaceLeft;
                    if (overFlow > 0)
                    {
                        heldItem.count = overFlow;
                        itemInThisSlot.count = itemInThisSlot.item.maxStack;
                        itemInThisSlot.RefreshCount();
                        heldItem.RefreshCount();
                        return;
                    }
                    itemInThisSlot.count += heldItem.count;
                    Destroy(heldItem.gameObject);
                    itemInThisSlot.RefreshCount();
                }
                else
                {
                    Debug.Log("You Can't Stack These Items Together!");
                }
            }
            else
            {
                Debug.Log("This Slot Has Reached It's Max Stack!");
            }
        }
    }

    public void AddItemToSlot(InventoryItem inventoryItem)
    {
        // heldItem = eventData.pointerDrag.GetComponent<InventoryItem>();
        itemInThisSlot = transform.GetChild(0).GetComponent<InventoryItem>();
        if (heldItem.item == itemInThisSlot.item)
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

    public void OnDrag(PointerEventData eventData)
    {
        heldItem = eventData.pointerDrag.GetComponent<InventoryItem>();
    }
}
