using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public List<InventorySlot> inventorySlots = new();

    [SerializeField] InventorySlot[] _toolbarSlots;

    [SerializeField] GameObject _inventoryItemPrefab;

    int selectedSlot = -1;

    public List<ItemInfo> itemsInInventory = new();

    public Item[] _allItems;

    public InventoryItem heldItem;
    [SerializeField] InGameUIManager _uiManager;

    [SerializeField] Item itemToSpawn;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            UpdateItemsInfoList();
        }

        if (Input.inputString != null)
        {
            bool isNumber = int.TryParse(Input.inputString, out int number);
            if (isNumber && number > 0 && number < 3)
            {
                ChangeSelectedSlot(number - 1);
            }
        }
    }

    void ChangeSelectedSlot(int newSlot)
    {
        if (selectedSlot == newSlot)
        {
            _toolbarSlots[selectedSlot].Deselect();
            selectedSlot = -1;
            return;
        }

        if (selectedSlot >= 0)
        {
            _toolbarSlots[selectedSlot].Deselect();
        }

        _toolbarSlots[newSlot].Select();
        selectedSlot = newSlot;
    }

    public Item GetSelectedItem()
    {
        if (selectedSlot > -1)
        {
            return _toolbarSlots[selectedSlot].GetInventoryItem().item;
        }
        else
        {
            return null;
        }
    }


    public bool AddItem(int itemId, int amount)
    {
        if (itemId < 0)
        {
            itemToSpawn = null;
            return false;
        }
        for (int i = 0; i < _allItems.Length; i++)
        {
            if (_allItems[i].itemID == itemId)
            {
                itemToSpawn = _allItems[i];
                Debug.Log($"Adding Item {_allItems[i]}");
            }
        }
        //Check for stackable slot
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();


            if (itemInSlot != null && itemInSlot.item == itemToSpawn && itemInSlot.count < itemInSlot.item.maxStack)
            {
                itemInSlot.count++;
                itemInSlot.RefreshCount();
                UpdateItemsInfoList();
                if (!_uiManager.inventoryShown)
                {
                    if (!slot.isHudSlot)
                        itemInSlot.transform.GetChild(0).gameObject.SetActive(false);
                }
                else
                {
                    if (!slot.isHudSlot)
                        itemInSlot.transform.GetChild(0).gameObject.SetActive(true);
                }
                return true;
            }

        }
        //Check for empty Slot
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (inventorySlots[i].isHudSlot && !itemToSpawn.canBeInHudSlot)
            {
                Debug.Log("Can't Spawn This Item Here As it Is BlackListed");
                UpdateItemsInfoList();
                return false;
            }

            int slot = i;
            InventoryItem itemInSlot = inventorySlots[slot].GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null)
            {
                SpawnNewItem(itemId, amount, slot);
                return true;
            }
        }

        UpdateItemsInfoList();

        return false;
    }

    public void SpawnNewItem(int itemID, int itemCount, int slotID)
    {
        GameObject newItemGO = Instantiate(_inventoryItemPrefab, inventorySlots[slotID].transform);
        InventoryItem inventoryItem = newItemGO.GetComponent<InventoryItem>();
        inventoryItem.count = itemCount;
        for (int i = 0; i < _allItems.Length; i++)
        {
            if (_allItems[i].itemID == itemID)
            {
                Debug.Log($"Id was found as {_allItems[i].itemID}. Initializing Item {_allItems[i]}");
                inventoryItem.InitializeItem(_allItems[i]);
                break;
            }
        }
        if (!_uiManager.inventoryShown && !inventorySlots[slotID].isHudSlot)
        {
            inventoryItem.GetComponent<Image>().enabled = false;
            inventoryItem.transform.GetChild(0).gameObject.SetActive(false);
        }
        UpdateItemsInfoList();
    }

    public void UseItem(int itemID, int itemCount)
    {
        int itemsLeft = itemCount;
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (itemsLeft == 0)
            {
                break;
            }
            if (itemsLeft < 0)
            {
                Debug.LogError("Removed Too Many Items!");
            }
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();

            for (int z = 0; z < _allItems.Length; z++)
            {
                if (_allItems[z].itemID == itemID)
                {
                    if (itemInSlot != null && itemInSlot.item == _allItems[z])
                    {
                        if (itemInSlot.count >= itemCount)
                        {
                            itemInSlot.count -= itemCount;
                            itemsLeft -= itemCount;
                            break;
                        }
                        else
                        {
                            itemsLeft -= itemInSlot.count;
                            itemInSlot.count = 0;
                        }

                        itemInSlot.RefreshCount();
                        UpdateItemsInfoList();
                    }
                }
            }

        }
    }

    public void UpdateItemsInfoList()
    {
        itemsInInventory.Clear();
        for (int i = 0; i < _allItems.Length; i++)
        {
            int amount = GetTotalItemAmount(inventorySlots, _allItems[i]);
            itemsInInventory.Add(new(_allItems[i], amount));
        }
    }

    static int GetTotalItemAmount(List<InventorySlot> list, Item item)
    {

        int result = 0;
        foreach (var itm in list)
        {
            if (itm.transform.childCount > 0)
            {
                InventoryItem thisItem = itm.GetComponentInChildren<InventoryItem>();

                HashSet<Item> items = list.Select(o => thisItem.item).ToHashSet();
                if (thisItem != null)
                {
                    if (thisItem.item != item)
                    {
                        continue;
                    }

                    result += thisItem.count;
                }
            }
        }

        return result;
    }

}
