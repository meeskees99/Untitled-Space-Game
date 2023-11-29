using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingManager : MonoBehaviour
{
    public static CraftingManager Instance;

    [SerializeField] Recipe[] allRecipes;
    public Recipe selectedRecipeToCraft;

    public Transform recipeListTransform;
    public GameObject itemPrefab;

    [Header("Recipe Selection")]
    [SerializeField] GameObject imagePrefab;
    [SerializeField] Transform imageListTransform;

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
        for (int i = imageListTransform.childCount; i > 0; i--)
        {
            Destroy(imageListTransform.GetChild(i - 1).gameObject);
        }
        if (recipe == null)
        {
            Debug.Log("No Recipe Selected");
            return;
        }
        selectedRecipeToCraft = recipe;
        Debug.Log($"Selected Recipe {recipe}");

        for (int i = 0; i < recipe.itemsNeeded.Length; i++)
        {
            GameObject spawnedImage = Instantiate(imagePrefab, imageListTransform);
            spawnedImage.GetComponent<Image>().sprite = recipe.itemsNeeded[i].item.image;
            spawnedImage.transform.GetChild(0).GetComponent<TMP_Text>().text = recipe.itemsNeeded[i].amount.ToString();
        }
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
                            Debug.Log($"You don't have enough {selectedRecipeToCraft.itemsNeeded[i].item.name} to craft this item!");
                            return;
                        }
                    }
                }
            }
            for (int i = 0; i < selectedRecipeToCraft.itemsNeeded.Length; i++)
            {
                InventoryManager.Instance.UseItem(selectedRecipeToCraft.itemsNeeded[i].item.itemID, selectedRecipeToCraft.itemsNeeded[i].amount);
                Debug.Log("Harold IK EET NU ITEMS");
            }
            InventoryManager.Instance.AddItem(selectedRecipeToCraft.itemToCraft.itemID, selectedRecipeToCraft.amountToCraft);
            InventoryManager.Instance.UpdateItemsInfoList();
        }
        else
        {
            Debug.Log("No Recipe Selected");
        }
    }

    public void RecipeButton(int index)
    {
        AddRecipe(allRecipes[index]);
    }

    void AddRecipe(Recipe recipe)
    {
        GameObject spawnedRecipe = Instantiate(itemPrefab, recipeListTransform);
        spawnedRecipe.GetComponent<CraftButton>().recipe = recipe;
        spawnedRecipe.GetComponent<CraftButton>().UpdateRecipeUI();
    }
}
