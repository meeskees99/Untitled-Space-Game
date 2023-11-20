using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Items/Item")]
public class Item : ScriptableObject
{
    [Header("Gameplay Only")]
    public TileBase tile;
    public Vector2Int range = new(5, 4);
    public ItemType type;
    public ActionType actionType;

    [Header("UI Only")]
    public bool stackable = true;
    [Header("Both")]
    public Sprite image;
    public int maxStack = 1;

    public enum ItemType
    {
        BuildingBlock,
        Consumable,
        Tool
    }

    public enum ActionType
    {
        Mine,
        Place
    }
}