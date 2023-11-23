using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftButton : MonoBehaviour
{
    public Recipe recipe;

    bool isSelected;
    [SerializeField] GameObject selectedObj;
    [SerializeField] TMP_Text itemNameText;
    [SerializeField] Image itemImage;

    // [SerializeField] GameObject imagePrefab;

    void Start()
    {
        if (recipe != null)
        {
            itemImage.sprite = recipe.itemToCraft.image;
            itemNameText.text = recipe.itemToCraft.name;
        }
    }

    public void SelectRecipe()
    {
        UpdateRecipeUI();
        if (!isSelected)
        {
            selectedObj.SetActive(true);
            isSelected = true;
            CraftingManager.Instance.SelectCraftingRecipe(recipe);
        }
        else
        {
            selectedObj.SetActive(false);
            isSelected = false;
            CraftingManager.Instance.SelectCraftingRecipe(null);
        }
    }

    public void UpdateRecipeUI()
    {
        if (recipe == null)
        {
            Debug.Log("Could Not Update RecipeUI As There Is No Recipe Attatched To " + gameObject.name);
            return;
        }
        itemImage.sprite = recipe.itemToCraft.image;
        itemNameText.text = recipe.itemToCraft.name;
        // for (int i = 0; i < recipe.itemsNeeded.Length; i++)
        // {
        //     // resourceText.text += $"{recipe.itemsNeeded[i].amount} {recipe.itemsNeeded[i].item.name}\n";
        //     // GameObject newImage = Instantiate(imagePrefab, transform);
        //     // newImage.GetComponent<Image>().sprite = recipe.itemsNeeded[i].item.image;
        // }
    }
}
