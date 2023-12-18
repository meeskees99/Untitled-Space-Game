using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmeltingMachine : MonoBehaviour
{
    [Header("Info")]
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

    public int ItemAmount { get { return _itemAmount; } set { _itemAmount = value; } }
    public int FuelAmount { get { return _fuelAmount; } set { _fuelAmount = value; } }
    public InventorySlot ItemSlot { get { return _itemSlot; } set { _itemSlot = value; } }
    public InventorySlot FuelSlot { get { return _fuelSlot; } set { _fuelSlot = value; } }
    public Item ItemType { get { return _itemType; } set { _itemType = value; } }
    public Item FuelType { get { return _fuelType; } set { _fuelType = value; } }
    public bool ResourceInitialized { get { return _resourceInitialized; } set { _resourceInitialized = value; } }
    public bool FuelInitialized { get { return _fuelInitialized; } set { _fuelInitialized = value; } }

    [SerializeField] bool _fuelTimeInitialized;
    [SerializeField] float _fuelLeft;



}
