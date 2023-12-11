using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiggingMachine : MonoBehaviour
{
    [SerializeField] Resource _collectedResource;
    [SerializeField] bool _isDigging;

    [Header("Miner Settings")]
    [SerializeField] float _miningRange = 3f;
    [SerializeField] LayerMask _resourceLayer;

    [Header("MinerInventory")]
    [SerializeField] InventorySlot _itemSlot;
    [SerializeField] InventorySlot _fuelSlot;
    [SerializeField] GameObject _inventoryItemPrefab;
    public Slider fuelLeftSlider;

    float _currentMineProgression;

    public InventorySlot ItemSlot { get { return _itemSlot; } set { _itemSlot = value; } }
    public InventorySlot FuelSlot { get { return _fuelSlot; } set { _fuelSlot = value; } }

    private void Start()
    {

        if (_collectedResource == null)
        {
            _collectedResource = GetComponentInParent<ResourceVein>().Resource;
        }
        MiningPanelManager.Instance.SetDiggerInfo(this);
    }


    void Update()
    {
        if (_collectedResource == null)
        {
            Debug.LogError("Miner Has No Resource To Gather!");
        }
        else
        {
            if (_itemSlot.GetInventoryItem() != null)
            {
                if (_itemSlot.GetInventoryItem().count >= _itemSlot.GetInventoryItem().item.maxStack)
                {
                    Debug.Log("Item Has Reached Max Stack! Remove It To Continue");
                    return;
                }
            }
            if (_currentMineProgression > 0)
            {
                bool result = _fuelSlot.HandleFuel();
                _isDigging = result;
                if (_fuelSlot.GetInventoryItem() != null)
                {
                    if (fuelLeftSlider != null)
                        fuelLeftSlider.maxValue = _fuelSlot.GetInventoryItem().item.fuelTime;
                    else
                    {
                        InGameUIManager.Instance.SetMinerUIInfo();
                    }
                }

                if (result)
                {
                    _currentMineProgression -= Time.deltaTime;
                    if (fuelLeftSlider != null)
                        fuelLeftSlider.value = _fuelSlot.fuelLeft;
                    else
                    {
                        InGameUIManager.Instance.SetMinerUIInfo();
                    }
                }
                else
                {
                    if (_currentMineProgression < 0.05)
                    {
                        _currentMineProgression = 0;
                    }
                }
            }
            else if (_currentMineProgression <= 0)
            {
                AddMachineItem();
            }
            else if (_currentMineProgression < 0)
            {
                Debug.LogError("Machine Has No Fuel AND Is Not Trying To Mine");
            }
        }
    }

    public void AddMachineItem()
    {
        if (_itemSlot.GetInventoryItem() != null)
        {
            _itemSlot.GetInventoryItem().count += _collectedResource.recourceAmount;
            _itemSlot.GetInventoryItem().RefreshCount();
        }
        else
        {
            SpawnMachineItem(_collectedResource.item, _collectedResource.recourceAmount);
        }
        _currentMineProgression = _collectedResource.mineDuration;
    }

    public void SpawnMachineItem(Item item, int amount)
    {
        GameObject newItemGO = Instantiate(_inventoryItemPrefab, _itemSlot.transform);
        _itemSlot.SetInventoryItem(newItemGO.GetComponent<InventoryItem>());

        _itemSlot.GetInventoryItem().count = amount;
        _itemSlot.GetInventoryItem().InitializeItem(item);
    }
}
