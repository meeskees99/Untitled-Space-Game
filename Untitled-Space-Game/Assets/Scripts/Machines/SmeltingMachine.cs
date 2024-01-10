using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmeltingMachine : MonoBehaviour
{
    [Header("Info")]
    [SerializeField] Resource _resourceToSmelt;
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

    [Header("SmeltingInventory")]
    [SerializeField] InventorySlot _resourceInputSlot;
    [SerializeField] InventorySlot _fuelInputSlot;
    [SerializeField] InventorySlot _itemOutputSlot;
    public Slider fuelLeftSlider;
    public Slider progressSlider;

    float _currentSmeltProgression;

    public int ItemAmount { get { return _itemAmount; } set { _itemAmount = value; } }
    public int FuelAmount { get { return _fuelAmount; } set { _fuelAmount = value; } }
    public InventorySlot ItemSlot { get { return _resourceInputSlot; } set { _resourceInputSlot = value; } }
    public InventorySlot FuelSlot { get { return _fuelInputSlot; } set { _fuelInputSlot = value; } }
    public Item ItemType { get { return _itemType; } set { _itemType = value; } }
    public Item FuelType { get { return _fuelType; } set { _fuelType = value; } }
    public bool ResourceInitialized { get { return _resourceInitialized; } set { _resourceInitialized = value; } }
    public bool FuelInitialized { get { return _fuelInitialized; } set { _fuelInitialized = value; } }

    [SerializeField] bool _fuelTimeInitialized;
    [SerializeField] float _fuelLeft;

    public bool HandleFuel()
    {
        if (!_fuelTimeInitialized && _fuelLeft <= 0)
        {
            // Debug.Log("USE ITEM");
            if (MiningPanelManager.Instance.panelActive && MiningPanelManager.Instance.currentDigger == this)
            {
                if (FuelAmount == 0 && _fuelLeft <= 0)
                {
                    _fuelTimeInitialized = false;
                    return false;
                }
                else if (FuelAmount > 0)
                {
                    _fuelInputSlot.UseItem();
                    _fuelAmount--;
                    _fuelLeft = _fuelType.fuelTime;
                    _fuelTimeInitialized = true;
                    Debug.Log("Used Some Fuel, Fuel Items Left: " + FuelAmount);
                    return true;
                }
            }
            else
            {
                if (FuelAmount == 0 && _fuelLeft <= 0)
                {
                    _fuelTimeInitialized = false;
                    return false;
                }
                _fuelAmount--;
                _fuelLeft = _fuelType.fuelTime;
                _fuelTimeInitialized = true;
                return true;
            }
            if (_fuelType != null && _fuelAmount > 0 && _fuelLeft <= 0)
            {
                _fuelLeft = _fuelType.fuelTime;
                _fuelTimeInitialized = true;
                return true;
            }
            else if (_fuelAmount <= 0 && _fuelLeft <= 0)
            {
                _fuelTimeInitialized = false;
                return false;
            }
            _fuelTimeInitialized = true;
            return true;
        }
        else if (_fuelType == null)
        {
            if (_fuelInputSlot.GetInventoryItem() != null)
            {
                InitializeFuelType();
                return false;
            }
        }
        else if (_fuelLeft > 0)
        {
            _fuelLeft -= Time.deltaTime;
            fuelLeftSlider.value = _fuelLeft;
            return true;

        }
        else if (_fuelLeft <= 0)
        {
            _fuelTimeInitialized = false;
            return false;
        }
        else if (_fuelType == null && _fuelLeft <= 0)
        {
            _fuelTimeInitialized = false;
            return false;
        }
        Debug.Log("Digger Has No Fuel");
        return false;
    }

    private void Update()
    {
        if (progressSlider != null && SmeltingPanelManager.Instance._currentSmelter == this)
        {
            progressSlider.value = _currentSmeltProgression;
        }
        else if (progressSlider == null && SmeltingPanelManager.Instance._currentSmelter == this)
        {
            Debug.LogError("Progress Slider Not Set");
        }
    }

    public void AddMachineItem(bool isFuel, Item item = null, int amount = 0, bool calledFromPanel = false)
    {
        if (!isFuel)
        {
            if (!MiningPanelManager.Instance.panelActive || MiningPanelManager.Instance.panelActive && MiningPanelManager.Instance.currentDigger != this)
            {
                if (amount == 0)
                {
                    _itemAmount++;
                    _currentSmeltProgression = _resourceToSmelt.smeltDuration;
                }
                else
                {
                    _itemAmount += amount;
                    _currentSmeltProgression = _resourceToSmelt.smeltDuration;
                }
                Debug.Log($"Added {amount} Item(s) When Panel Not Shown");
                return;
            }
            else if (MiningPanelManager.Instance.panelActive && MiningPanelManager.Instance.currentDigger == this)
            {
                if (amount > 0 && _resourceInputSlot.GetInventoryItem() == null)
                {
                    SpawnMachineItem(item, amount);
                    Debug.Log($"Spawned {amount} {item}");
                }
                else if (_resourceInputSlot.GetInventoryItem() != null)
                {
                    if (amount == 0)
                    {
                        _itemAmount++;
                        _resourceInputSlot.GetInventoryItem().count++;
                        _resourceInputSlot.GetInventoryItem().RefreshCount();
                    }
                    else
                    {
                        _itemAmount += amount;
                        _resourceInputSlot.GetInventoryItem().count += amount;
                        _resourceInputSlot.GetInventoryItem().RefreshCount();
                    }
                }
                else
                {
                    if (amount == 0)
                    {
                        SpawnMachineItem(_resourceToSmelt.item, 1);
                        // _itemAmount += _collectedResource.recourceAmount;
                        Debug.Log("Spawned First Of " + _resourceToSmelt.item);
                        // Debug.Log("Had No resources to spawn");
                    }
                    else
                    {
                        SpawnMachineItem(item, amount);
                        Debug.Log($"Spawned {amount} {item.name}");
                    }
                }
            }
            if (calledFromPanel)
            {

            }
            else
            {
                _itemAmount += amount;
                _currentSmeltProgression = _resourceToSmelt.smeltDuration;
            }

        }
        else
        {
            if (!MiningPanelManager.Instance.panelActive || MiningPanelManager.Instance.panelActive && MiningPanelManager.Instance.currentDigger != this)
            {
                if (amount == 0)
                {
                    Debug.Log("Did absolutly nothing");
                }
                else
                {
                    _fuelAmount += amount;
                }
                Debug.Log($"Added {amount} {_fuelType} When Panel Not Shown");
                return;
            }
            if (amount > 0)
            {
                SpawnMachineFuel(item, amount);
            }
            else if (_fuelInputSlot.GetInventoryItem() != null && amount > 0 && _fuelInputSlot.GetInventoryItem() == item)
            {
                _fuelInputSlot.GetInventoryItem().count += amount;
                _fuelInputSlot.GetInventoryItem().RefreshCount();
            }
            else
            {
                SpawnMachineFuel(item, _fuelAmount);
            }
            if (!calledFromPanel)
            {
                _currentSmeltProgression = _resourceToSmelt.smeltDuration;
            }
        }

    }

    public void SpawnMachineItem(Item item, int amount)
    {
        GameObject newItemGO = Instantiate(InventoryManager.Instance.InventoryItemPrefab, _resourceInputSlot.transform);
        _resourceInputSlot.SetInventoryItem(newItemGO.GetComponent<InventoryItem>());
        _resourceInputSlot.GetInventoryItem().lastInventorySlot = _resourceInputSlot;
        _resourceInputSlot.GetInventoryItem().count = amount;
        _resourceInputSlot.GetInventoryItem().InitializeItem(item, amount);

        Debug.Log("Resource was Initialized: " + newItemGO.name);

        if (_resourceInitialized)
        {
            Debug.Log("Resource was already Initialized");
            return;
        }
        _itemType = item;
        _resourceInitialized = true;
        _itemAmount = _resourceInputSlot.GetInventoryItem().count;
    }

    public void InitializeFuelType()
    {
        if (_fuelInputSlot.GetInventoryItem() == null)
        {
            return;
        }
        _fuelInputSlot.SetInventoryItem(_fuelInputSlot.transform.GetChild(0).GetComponent<InventoryItem>());
        _fuelType = _fuelInputSlot.GetInventoryItem().item;
        _fuelAmount = _fuelInputSlot.GetInventoryItem().count;
        Debug.Log("Initialized fuel type: " + _fuelAmount + " " + _fuelType);
        _fuelInitialized = true;
    }

    void SpawnMachineFuel(Item item, int amount)
    {
        GameObject newItemGO = Instantiate(InventoryManager.Instance.InventoryItemPrefab, _fuelInputSlot.transform);
        _fuelInputSlot.SetInventoryItem(newItemGO.GetComponent<InventoryItem>());
        _fuelInputSlot.GetInventoryItem().count = amount;
        _fuelInputSlot.GetInventoryItem().InitializeItem(item, amount);
        if (_fuelInitialized)
        {
            Debug.Log("Fuel was already Initialized");
            return;
        }
        InitializeFuelType();
    }
}
