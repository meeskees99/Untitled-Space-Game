using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiggingMachine : MonoBehaviour
{
    [SerializeField] Resource collectedResource;
    [SerializeField] InventorySlot itemSlot;
    [SerializeField] InventorySlot fuelSlot;
    [SerializeField] GameObject _inventoryItemPrefab;
    [SerializeField] bool isDigging;


    [Header("Miner Settings")]
    [SerializeField] float miningRange = 3f;
    [SerializeField] LayerMask resourceLayer;
    [SerializeField] float miningSpeed = 2f;

    float currentMineProgression;

    private void Start()
    {
        currentMineProgression = miningSpeed;
    }

    void Update()
    {
        if (collectedResource == null)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, miningRange, resourceLayer))
            {
                collectedResource = hit.transform.GetComponent<ResourceVein>().Resource;
            }
        }
        else
        {
            if (currentMineProgression < 0 && fuelSlot.itemInThisSlot != null)
            {
                bool result = fuelSlot.Mine();
                isDigging = result;
                if (result)
                {
                    currentMineProgression -= Time.deltaTime;
                }
                else
                {
                    Debug.Log("Smelter Needs Fuel");
                }

            }
            else if (currentMineProgression <= 0 && fuelSlot.itemInThisSlot != null)
            {
                currentMineProgression = miningSpeed;
                AddMachineItem();
            }
            else if (currentMineProgression <= 0)
            {
                currentMineProgression = miningSpeed;
                AddMachineItem();
            }
            else if (currentMineProgression > 0 && fuelSlot.itemInThisSlot == null)
            {
                Debug.Log("Machine Has No Fuel AND Is Not Mining");
            }
        }
    }

    public void AddMachineItem()
    {
        if (itemSlot.itemInThisSlot != null)
        {
            itemSlot.itemInThisSlot.count += collectedResource.recourceAmount;
            itemSlot.itemInThisSlot.RefreshCount();
        }
        else
        {
            SpawnMachineItem(collectedResource.item, collectedResource.recourceAmount);
        }
        currentMineProgression = miningSpeed;
    }

    public void SpawnMachineItem(Item item, int amount)
    {
        GameObject newItemGO = Instantiate(_inventoryItemPrefab, itemSlot.transform);
        itemSlot.itemInThisSlot = newItemGO.GetComponent<InventoryItem>();

        itemSlot.itemInThisSlot.count = amount;
        itemSlot.itemInThisSlot.InitializeItem(item);
    }
}
