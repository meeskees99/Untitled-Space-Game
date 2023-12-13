using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiggingMachine : MonoBehaviour
{
    [Header("")]
    [SerializeField] Resource _collectedResource;
    [SerializeField] bool _isDigging;
    [SerializeField] Item _itemType;
    [SerializeField] int _itemAmount;
    [SerializeField] Item _fuelType;
    [SerializeField] int _fuelAmount;

    [SerializeField] bool _resourceInitialized;
    [SerializeField] bool _fuelInitialized;

    [Header("Miner Settings")]
    [SerializeField] float _miningRange = 3f;
    [SerializeField] LayerMask _resourceLayer;

    [Header("MinerInventory")]
    [SerializeField] InventorySlot _itemSlot;
    [SerializeField] InventorySlot _fuelSlot;
    public Slider fuelLeftSlider;

    float _currentMineProgression;

    public InventorySlot ItemSlot { get { return _itemSlot; } set { _itemSlot = value; } }
    public InventorySlot FuelSlot { get { return _fuelSlot; } set { _fuelSlot = value; } }
    public Item FuelType { get { return _fuelType; } set { _fuelType = value; } }
    public Item ItemType { get { return _itemType; } set { _itemType = value; } }

    [SerializeField] bool _fuelTimeInitialized;
    [SerializeField] float _fuelLeft;
    private void Start()
    {

        if (_collectedResource == null)
        {
            _collectedResource = GetComponentInParent<ResourceVein>().Resource;
        }
        MiningPanelManager.Instance.SetDiggerInfo(this);
    }

    public bool HandleFuel()
    {
        if (!_fuelTimeInitialized && _fuelSlot.GetInventoryItem() != null)
        {
            Debug.Log("USE ITEM");
            _fuelSlot.UseItem();
            if (_fuelSlot.GetInventoryItem() != null)
            {
                _fuelLeft = _fuelSlot.GetInventoryItem().item.fuelTime;
            }
            if (_fuelAmount > 0)
            {
                _fuelAmount--;
            }
            else
            {
                _fuelTimeInitialized = false;
                return false;
            }
            _fuelTimeInitialized = true;
            return true;
        }
        else if (_fuelSlot.GetInventoryItem() == null && _fuelLeft > 0)
        {
            if (_fuelLeft > 0)
            {
                _fuelLeft -= Time.deltaTime;
                return true;
            }
            else
            {
                _fuelTimeInitialized = false;
            }
            Debug.Log("This Item Cannot Be Used As Fuel");
        }
        else if (_fuelSlot.GetInventoryItem() != null && _fuelTimeInitialized)
        {
            if (_fuelLeft > 0)
            {
                _fuelLeft -= Time.deltaTime;
                return true;
            }
            else
            {
                _fuelTimeInitialized = false;
                return false;
            }
        }
        else if (_fuelSlot.GetInventoryItem() == null && _fuelLeft <= 0)
        {
            _fuelTimeInitialized = false;
            return false;
        }
        return false;
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
            else
            {

            }
            if (_currentMineProgression > 0)
            {
                bool result = HandleFuel();
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
                        fuelLeftSlider.value = _fuelLeft;
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
            else if (_currentMineProgression <= 0 && _fuelLeft > 0)
            {
                AddMachineItem(false, _itemType);
                _itemAmount++;
            }
            else if (_currentMineProgression < 0)
            {
                Debug.LogError("Machine Has No Fuel AND Is Not Trying To Mine");
            }
        }
    }

    public void AddMachineItem(bool isFuel, Item item = null, int amount = 0)
    {
        if (!isFuel)
        {
            if (_itemAmount > 0 && _itemSlot.GetInventoryItem() == null)
            {
                SpawnMachineItem(item, _itemAmount);

            }
            else if (_itemSlot.GetInventoryItem() != null)
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
        else
        {
            if (amount > 0)
            {
                SpawnMachineFuel(item, amount);
            }
            else if (_fuelSlot.GetInventoryItem() != null && _fuelSlot.GetInventoryItem() == item)
            {
                _fuelSlot.GetInventoryItem().count += _collectedResource.recourceAmount;
                _fuelSlot.GetInventoryItem().RefreshCount();
            }
            else
            {
                SpawnMachineFuel(item, _fuelAmount);
            }
            _currentMineProgression = _collectedResource.mineDuration;
        }

    }

    public void SpawnMachineItem(Item item, int amount)
    {
        GameObject newItemGO = Instantiate(InventoryManager.Instance.InventoryItemPrefab, _itemSlot.transform);
        _itemSlot.SetInventoryItem(newItemGO.GetComponent<InventoryItem>());

        _itemSlot.GetInventoryItem().count = amount;
        _itemSlot.GetInventoryItem().InitializeItem(item);

        if (_resourceInitialized)
        {
            Debug.Log("Resource was already Initialized");
            return;
        }
        _itemType = item;
        _resourceInitialized = true;
        _itemAmount = _itemSlot.GetInventoryItem().count;
    }

    void SpawnMachineFuel(Item item, int amount)
    {
        GameObject newItemGO = Instantiate(InventoryManager.Instance.InventoryItemPrefab, _fuelSlot.transform);
        _fuelSlot.SetInventoryItem(newItemGO.GetComponent<InventoryItem>());
        _fuelSlot.GetInventoryItem().count = amount;
        _fuelSlot.GetInventoryItem().InitializeItem(item);
        _fuelInitialized = true;

        if (_fuelInitialized)
        {
            Debug.Log("Fuel was already Initialized");
            return;
        }
        InitializeFuelType();
    }

    public void InitializeFuelType()
    {
        _fuelSlot.SetInventoryItem(_fuelSlot.transform.GetChild(0).GetComponent<InventoryItem>());
        _fuelType = _fuelSlot.GetInventoryItem().item;
        _fuelAmount = _fuelSlot.GetInventoryItem().count;
        Debug.Log("Initialized fuel type: " + _fuelAmount + _fuelType);
    }
}
