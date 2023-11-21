using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemInfo
{
    public Item item;
    public int amount;

    public ItemInfo(Item item, int amount)
    {
        this.item = item;
        this.amount = amount;
    }
}
