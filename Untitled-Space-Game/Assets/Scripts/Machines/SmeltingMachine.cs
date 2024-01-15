using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmeltingMachine : MonoBehaviour
{
    [Header("Info")]
    [SerializeField] Item _resourceToSmelt;
    [SerializeField] bool _isSmelting;

    [SerializeField] Item _resourceType;
    [SerializeField] Item _outputType;
    [SerializeField] Item _fuelType;

    [SerializeField] int _resourceAmount;
    [SerializeField] int _outputAmount;
    [SerializeField] int _fuelAmount;

    [SerializeField] bool _resourceInitialized;
    [SerializeField] bool _fuelInitialized;
    [SerializeField] bool _fuelTimeInitialized;

    [Header("SmeltingInventory")]
    [SerializeField] InventorySlot _resourceInputSlot;
    [SerializeField] InventorySlot _itemOutputSlot;
    [SerializeField] InventorySlot _fuelInputSlot;

    public Slider fuelLeftSlider;
    public Slider progressSlider;

    [SerializeField] float _currentSmeltProgression;
    [SerializeField] float _fuelLeft;

    public int ResourceAmount { get { return _resourceAmount; } set { _resourceAmount = value; } }
    public int OutputAmount { get { return _outputAmount; } set { _outputAmount = value; } }
    public int FuelAmount { get { return _fuelAmount; } set { _fuelAmount = value; } }

    public InventorySlot ResourceInputSlot { get { return _resourceInputSlot; } set { _resourceInputSlot = value; } }
    public InventorySlot ItemOutputSlot { get { return _itemOutputSlot; } set { _itemOutputSlot = value; } }
    public InventorySlot FuelInputSlot { get { return _fuelInputSlot; } set { _fuelInputSlot = value; } }

    public Item ResourceType { get { return _resourceType; } set { _resourceType = value; } }
    public Item OutputType { get { return _outputType; } set { _outputType = value; } }
    public Item FuelType { get { return _fuelType; } set { _fuelType = value; } }

    public bool ResourceInitialized { get { return _resourceInitialized; } set { _resourceInitialized = value; } }
    public bool FuelInitialized { get { return _fuelInitialized; } set { _fuelInitialized = value; } }

    private void Start()
    {
        SmeltingPanelManager.Instance.SetSmelterInfo(this);
    }

    public bool HandleFuel()
    {
        //Check If Fuel Needs To Be Initialized
        if (_fuelType == null && _fuelInputSlot.GetInventoryItem() && SmeltingPanelManager.Instance.currentSmelter == this)
        {
            InitializeFuelType();
        }
        else if (_fuelType != null && !_fuelInputSlot.GetInventoryItem() && SmeltingPanelManager.Instance.currentSmelter == this)
        {
            //Reset FuelType When FuelSlot Is Empty
            _fuelType = null;
            _fuelInitialized = false;
        }
        if (_resourceType == null && _resourceInputSlot.GetInventoryItem())
        {
            if (SmeltingPanelManager.Instance.currentSmelter == this)
                _resourceType = _resourceInputSlot.GetInventoryItem().item;
        }
        else if (_resourceType != null && !_resourceInputSlot.GetInventoryItem())
        {
            if (SmeltingPanelManager.Instance.currentSmelter == this)
                _resourceType = null;
            // _resourceAmount = 0;
        }

















        return false;



        // if (!_fuelTimeInitialized && _fuelLeft <= 0)
        // {
        //     // Debug.Log("USE ITEM");
        //     if (SmeltingPanelManager.Instance.panelActive && SmeltingPanelManager.Instance.currentSmelter == this)
        //     {
        //         if (FuelAmount == 0 && _fuelLeft <= 0)
        //         {
        //             _fuelTimeInitialized = false;
        //             return false;
        //         }
        //         else if (FuelAmount > 0)
        //         {
        //             _fuelInputSlot.UseItem();
        //             _fuelAmount--;
        //             _fuelLeft = _fuelType.fuelTime;
        //             _fuelTimeInitialized = true;
        //             Debug.Log("Used Some Fuel, Fuel Items Left: " + FuelAmount);
        //             return true;
        //         }
        //     }
        //     else
        //     {
        //         if (FuelAmount == 0 && _fuelLeft <= 0)
        //         {
        //             _fuelTimeInitialized = false;
        //             return false;
        //         }
        //         _fuelAmount--;
        //         _fuelLeft = _fuelType.fuelTime;
        //         _fuelTimeInitialized = true;
        //         return true;
        //     }
        //     if (_fuelType != null && _fuelAmount > 0 && _fuelLeft <= 0)
        //     {
        //         _fuelLeft = _fuelType.fuelTime;
        //         _fuelTimeInitialized = true;
        //         return true;
        //     }
        //     else if (_fuelAmount <= 0 && _fuelLeft <= 0)
        //     {
        //         _fuelTimeInitialized = false;
        //         return false;
        //     }
        //     _fuelTimeInitialized = true;
        //     return true;
        // }
        // else if (_fuelType == null)
        // {
        //     if (_fuelInputSlot.GetInventoryItem() != null)
        //     {
        //         InitializeFuelType();
        //         return false;
        //     }
        // }
        // else if (_fuelLeft > 0 && _resourceInputSlot.GetInventoryItem().item.isSmeltable)
        // {
        //     _fuelLeft -= Time.deltaTime;
        //     _currentSmeltProgression += Time.deltaTime;

        //     return true;
        // }
        // else if (_fuelLeft <= 0)
        // {
        //     _fuelTimeInitialized = false;
        //     return false;
        // }
        // else if (_fuelType == null && _fuelLeft <= 0)
        // {
        //     _fuelTimeInitialized = false;
        //     return false;
        // }
        // else if (!_resourceInputSlot.GetInventoryItem().item.isSmeltable && _fuelLeft > 0)
        // {
        //     Debug.Log("Item Is Not Smeltable!");
        //     return false;
        // }
        // Debug.Log("Digger Has No Fuel");
        // return false;
    }

    private void Update()
    {
        if (progressSlider != null && SmeltingPanelManager.Instance.currentSmelter == this)
        {
            progressSlider.value = _currentSmeltProgression;
        }
        else if (progressSlider == null && SmeltingPanelManager.Instance.currentSmelter == this)
        {
            Debug.LogError("Progress Slider Not Set");
        }

        if (fuelLeftSlider != null && SmeltingPanelManager.Instance.currentSmelter == this)
        {
            fuelLeftSlider.value = _fuelLeft;
        }
        else if (progressSlider == null && SmeltingPanelManager.Instance.currentSmelter == this)
        {
            Debug.LogError("FuelLeft Slider Not Set");
        }
    }

    public void AddMachineItem(InventorySlot slot, Item item = null, int amount = 0, bool calledFromPanel = false)
    {
        //Spawn All Current Items When Panel Got Opened
        if (calledFromPanel)
        {
            SpawnItem(slot, item, amount);
        }
        else
        {
            //Add Singular Item To Output
            if (amount == 0)
            {
                //Add Item To Output When Panel Closed
                if (!SmeltingPanelManager.Instance.panelActive || SmeltingPanelManager.Instance.panelActive && SmeltingPanelManager.Instance.currentSmelter != this)
                {
                    _outputAmount++;
                }
                //Add Item To Output When Panel Opened
                else
                {
                    if (_outputAmount == 0)
                    {
                        //Spawn Item To Output
                        SpawnItem(ItemOutputSlot, _resourceToSmelt.itemToGetAfterSmelt, 1);
                    }
                    else
                    {
                        //Add Item To Output
                        _itemOutputSlot.GetInventoryItem().count++;
                        _itemOutputSlot.GetInventoryItem().RefreshCount();

                        _outputAmount++;
                    }
                }
            }
            else
            {
                Debug.LogError("This Shouldn't be called");
            }
        }

        // if (!SmeltingPanelManager.Instance.panelActive || SmeltingPanelManager.Instance.panelActive && SmeltingPanelManager.Instance.currentSmelter != this)
        // {
        //     if (amount == 0)
        //     {
        //         _resourceAmount++;
        //         _currentSmeltProgression = _resourceToSmelt.smeltTime;
        //     }
        //     else
        //     {
        //         _resourceAmount += amount;
        //         _currentSmeltProgression = _resourceToSmelt.smeltTime;
        //     }
        //     Debug.Log($"Added {amount} Item(s) When Panel Not Shown");
        //     return;
        // }
        // else if (MiningPanelManager.Instance.panelActive && MiningPanelManager.Instance.currentDigger == this)
        // {
        //     if (amount > 0 && _resourceInputSlot.GetInventoryItem() == null)
        //     {
        //         SpawnItem(slot, item, amount);
        //         Debug.Log($"Spawned {amount} {item}");
        //     }
        //     else if (_itemOutputSlot.GetInventoryItem() != null)
        //     {
        //         if (amount == 0)
        //         {
        //             _resourceAmount++;
        //             _itemOutputSlot.GetInventoryItem().count++;
        //             _itemOutputSlot.GetInventoryItem().RefreshCount();
        //         }
        //         else
        //         {
        //             _resourceAmount += amount;
        //             _itemOutputSlot.GetInventoryItem().count += amount;
        //             _itemOutputSlot.GetInventoryItem().RefreshCount();
        //         }
        //     }
        //     else
        //     {
        //         if (amount == 0)
        //         {
        //             SpawnItem(slot, _resourceToSmelt.itemToGetAfterSmelt, 1);
        //             // _itemAmount += _collectedResource.recourceAmount;
        //             Debug.Log("Spawned First Of " + _resourceToSmelt.itemToGetAfterSmelt);
        //             // Debug.Log("Had No resources to spawn");
        //         }
        //         else
        //         {
        //             SpawnItem(slot, item, amount);
        //             Debug.Log($"Spawned {amount} {item.name}");
        //         }
        //     }
        // }


        // if (!SmeltingPanelManager.Instance.panelActive || SmeltingPanelManager.Instance.panelActive && SmeltingPanelManager.Instance.currentSmelter != this)
        // {
        //     // TODO make this work for the smelter
        // }
    }

    public void SpawnItem(InventorySlot slot, Item item, int amount)
    {
        GameObject newItemGO = Instantiate(InventoryManager.Instance.InventoryItemPrefab, slot.transform);
        slot.SetInventoryItem(newItemGO.GetComponent<InventoryItem>());
        slot.GetInventoryItem().lastInventorySlot = _resourceInputSlot;
        slot.GetInventoryItem().count = amount;
        slot.GetInventoryItem().InitializeItem(item, amount);

        if (item.isFuel)
        {
            InitializeFuelType();
        }
        else if (item.isSmeltable)
        {
            _resourceToSmelt = item;
            _resourceInitialized = true;
            _resourceAmount = slot.GetInventoryItem().count;
        }
        else
        {
            Debug.Log($"Spawned {amount} {item} In Output Slot");
        }
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

    bool HasSmeltableResource()
    {
        if (_resourceInputSlot.GetInventoryItem() && _resourceInputSlot.GetInventoryItem().item.isSmeltable)
        {
            return true;
        }
        return false;
    }
}
