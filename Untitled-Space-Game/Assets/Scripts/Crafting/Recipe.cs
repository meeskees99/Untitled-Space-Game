using UnityEngine;

[CreateAssetMenu(menuName = "Items/Recipe")]
public class Recipe : ScriptableObject
{
    public Item itemToCraft;
    public ItemInfo[] itemsNeeded;
    public int amountToCraft;
}
