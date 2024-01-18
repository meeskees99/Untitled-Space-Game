using UnityEngine;

[CreateAssetMenu(menuName = "Items/Item")]
public class Item : ScriptableObject
{
    [Tooltip("Set This As It's Own Unique ID")]
    public int itemID;
    public GameObject itemPrefab;

    [Header("UI Only")]
    public bool stackable = true;
    [Header("Both")]
    public Sprite image;
    public int maxStack = 1;
    public bool canBeInHudSlot = false;
    public Item itemToGetAfterSmelt;
    [Tooltip("Amount Of ItemToGetAfterSmelt To Get When Smelting This In The Furnace")]
    public int amountToGetAfterSmelt = 1;

    [Header("Gameplay Settings")]
    public bool isTool;
    public bool isPlacable;
    public bool isFuel;
    public float fuelTime = 2;
    public bool isSmeltable;
    public float smeltTime = 4;

    [Header("Machine Specific")]
    public GameObject machinePrefab;
    public GameObject machineBlueprint;
}