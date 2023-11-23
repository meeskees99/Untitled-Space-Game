using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    public static CraftingManager Instance;

    public Recipe selectedRecipeToCraft;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SelectCraftingRecipe(Recipe recipe)
    {
        if (recipe == null)
        {
            Debug.Log("No Recipe Selected");
            return;
        }
        selectedRecipeToCraft = recipe;
        Debug.Log($"Selected Recipe {recipe}");
    }

    public void CraftItem()
    {
        if (selectedRecipeToCraft != null)
        {
            for (int i = 0; i < selectedRecipeToCraft.itemsNeeded.Length; i++)
            {
                for (int y = 0; y < InventoryManager.Instance.itemsInInventory.Count; y++)
                {
                    if (InventoryManager.Instance.itemsInInventory[y].item == selectedRecipeToCraft.itemsNeeded[i].item)
                    {
                        if (InventoryManager.Instance.itemsInInventory[y].amount < selectedRecipeToCraft.itemsNeeded[i].amount)
                        {
                            Debug.Log("You don't have the resources to craft this item!");
                            return;
                        }
                    }
                }
            }
            InventoryManager.Instance.AddItem(selectedRecipeToCraft.itemToCraft.itemID, selectedRecipeToCraft.amountToCraft);
        }
        else
        {
            Debug.Log("No Recipe Selected");
        }
    }
}
