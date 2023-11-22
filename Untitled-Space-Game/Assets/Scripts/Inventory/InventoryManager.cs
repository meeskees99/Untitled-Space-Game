using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public List<InventorySlot> inventorySlots = new();

    [SerializeField] InventorySlot[] toolbarSlots;

    [SerializeField] GameObject _inventoryItemPrefab;

    int selectedSlot = -1;

    [SerializeField] List<ItemInfo> itemsInInventory = new();

    [SerializeField] Item[] allItems;

    public InventoryItem heldItem;

    // public Dictionary<Item, int> itemAmounts = new();


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
            toolbarSlots[selectedSlot].Deselect();
            selectedSlot = -1;
            return;
        }

        if (selectedSlot >= 0)
        {
            toolbarSlots[selectedSlot].Deselect();
        }

        toolbarSlots[newSlot].Select();
        selectedSlot = newSlot;
    }


    public bool AddItem(int itemId, int amount)
    {
        if (itemId < 0)
        {
            return false;
        }
        //Check for stackable slot
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();


            if (itemInSlot != null && itemInSlot.item == allItems[itemId] && itemInSlot.count < itemInSlot.item.maxStack)
            {
                itemInSlot.count++;
                itemInSlot.RefreshCount();
                return true;
            }
        }
        //Check for empty Slot
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            for (int x = 0; x < inventorySlots[i].disAllowedItems.Length; x++)
            {
                if (inventorySlots[i].disAllowedItems[x] == allItems[itemId])
                {
                    Debug.Log("Can't Spawn This Item Here As it Is BlackListed");
                    return false;
                }
            }
            int slot = i;
            InventoryItem itemInSlot = inventorySlots[slot].GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null)
            {
                SpawnNewItem(itemId, amount, slot);
                return true;
            }
        }
        return false;
    }

    public void SpawnNewItem(int itemID, int itemCount, int slotID)
    {
        GameObject newItemGO = Instantiate(_inventoryItemPrefab, inventorySlots[slotID].transform);
        InventoryItem inventoryItem = newItemGO.GetComponent<InventoryItem>();
        inventoryItem.count = itemCount;
        inventoryItem.InitializeItem(allItems[itemID]);
    }

    public void LoadItemsInInventory()
    {

    }

    public Item GetSelectedItem(bool use)
    {
        if (selectedSlot == -1) return null;
        InventorySlot slot = toolbarSlots[selectedSlot];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
        if (itemInSlot != null)
        {
            Item item = itemInSlot.item;
            if (use)
            {
                itemInSlot.count--;
                if (itemInSlot.count <= 0)
                {
                    Destroy(itemInSlot.gameObject);
                }
                else
                {
                    itemInSlot.RefreshCount();
                }
            }
            return item;
        }
        return null;
    }

    public void UpdateItemsInfoList()
    {
        itemsInInventory.Clear();
        for (int i = 0; i < allItems.Length; i++)
        {
            int amount = GetTotalItemAmount(inventorySlots, allItems[i]);
            itemsInInventory.Add(new(allItems[i], amount));
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
