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

    [Header("Gameplaye Settings")]
    public bool isTool;
    public bool isPlacable;
    public bool isFuel;
    public float fuelTime = 2;
}