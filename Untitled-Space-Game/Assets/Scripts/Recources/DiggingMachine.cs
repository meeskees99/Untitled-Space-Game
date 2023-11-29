using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiggingMachine : MonoBehaviour
{
    [SerializeField] Resource collectedResource;
    [SerializeField] InventorySlot itemSlot;
    [SerializeField] InventorySlot fuelSlot;
    [SerializeField] GameObject _inventoryItemPrefab;
    public Slider fuelLeftSlider;
    [SerializeField] bool isDigging;


    [Header("Miner Settings")]
    [SerializeField] float miningRange = 3f;
    [SerializeField] LayerMask resourceLayer;
    // [SerializeField] float miningSpeed = 2f;

    float currentMineProgression;

    private void Start()
    {
        if (collectedResource == null)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, miningRange, resourceLayer))
            {
                collectedResource = hit.transform.GetComponent<ResourceVein>().Resource;
                currentMineProgression = collectedResource.mineDuration;
            }
        }
        InGameUIManager.Instance.SetMinerUIInfo();
    }


    void Update()
    {
        if (collectedResource == null)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, miningRange, resourceLayer))
            {
                collectedResource = hit.transform.GetComponent<ResourceVein>().Resource;
                currentMineProgression = collectedResource.mineDuration;
            }
        }
        else
        {
            if (itemSlot.GetInventoryItem() != null)
            {
                if (itemSlot.GetInventoryItem().count >= itemSlot.GetInventoryItem().item.maxStack)
                {
                    Debug.Log("Item Has Reached Max Stack! Remove It To Continue");
                    return;
                }
            }
            if (currentMineProgression > 0)
            {
                bool result = fuelSlot.HandleFuel();
                isDigging = result;
                if (fuelSlot.GetInventoryItem() != null)
                {
                    if (fuelLeftSlider != null)
                        fuelLeftSlider.maxValue = fuelSlot.GetInventoryItem().item.fuelTime;
                    else
                    {
                        InGameUIManager.Instance.SetMinerUIInfo();
                    }
                }

                if (result)
                {
                    currentMineProgression -= Time.deltaTime;
                    if (fuelLeftSlider != null)
                        fuelLeftSlider.value = fuelSlot.fuelLeft;
                    else
                    {
                        InGameUIManager.Instance.SetMinerUIInfo();
                    }
                }
                else
                {
                    if (currentMineProgression < 0.05)
                    {
                        currentMineProgression = 0;
                    }
                }
            }
            else if (currentMineProgression <= 0)
            {
                AddMachineItem();
            }
            else if (currentMineProgression < 0)
            {
                Debug.LogError("Machine Has No Fuel AND Is Not Trying To Mine");
            }
        }
    }

    public void AddMachineItem()
    {
        if (itemSlot.GetInventoryItem() != null)
        {
            itemSlot.GetInventoryItem().count += collectedResource.recourceAmount;
            itemSlot.GetInventoryItem().RefreshCount();
        }
        else
        {
            SpawnMachineItem(collectedResource.item, collectedResource.recourceAmount);
        }
        currentMineProgression = collectedResource.mineDuration;
    }

    public void SpawnMachineItem(Item item, int amount)
    {
        GameObject newItemGO = Instantiate(_inventoryItemPrefab, itemSlot.transform);
        itemSlot.SetInventoryItem(newItemGO.GetComponent<InventoryItem>());

        itemSlot.GetInventoryItem().count = amount;
        itemSlot.GetInventoryItem().InitializeItem(item);
    }
}
