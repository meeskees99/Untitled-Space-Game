using UnityEngine;

[CreateAssetMenu(menuName = "Items/Item")]
public class Item : ScriptableObject
{
    [Tooltip("Set This As It's Own Unique ID")]
    public int itemID;

    [Header("UI Only")]
    public bool stackable = true;
    [Header("Both")]
    public Sprite image;
    public int maxStack = 1;

    [Header("Gameplayer Settings")]
    public bool isTool;
    public bool isPlacable;
}