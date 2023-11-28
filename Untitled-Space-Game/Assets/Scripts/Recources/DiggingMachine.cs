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
    [SerializeField] float miningSpeed = 2f;

    float currentMineProgression;

    private void Start()
    {
        currentMineProgression = miningSpeed;
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
            }
        }
        else
        {
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
                        Debug.LogError("fuelLeftSlider == null");
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
                        Debug.LogError("fuelLeftSlider == null");
                    }
                }
                else
                {
                    if (currentMineProgression < 0.05)
                    {
                        currentMineProgression = 0;
                    }
                    Debug.Log("Smelter Needs Fuel");
                }
            }
            else if (currentMineProgression <= 0)
            {
                currentMineProgression = miningSpeed;
                AddMachineItem();
            }
            else if (currentMineProgression < 0)
            {
                Debug.Log("Machine Has No Fuel AND Is Not Mining");
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
        currentMineProgression = miningSpeed;
    }

    public void SpawnMachineItem(Item item, int amount)
    {
        GameObject newItemGO = Instantiate(_inventoryItemPrefab, itemSlot.transform);
        itemSlot.SetInventoryItem(newItemGO.GetComponent<InventoryItem>());

        itemSlot.GetInventoryItem().count = amount;
        itemSlot.GetInventoryItem().InitializeItem(item);
    }
}
