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
    [SerializeField] TMP_Text resourceText;
    [SerializeField] Image itemImage;

    void Start()
    {
        itemImage.sprite = recipe.itemToCraft.image;
        resourceText.text = "";
        for (int i = 0; i < recipe.itemsNeeded.Length; i++)
        {
            resourceText.text += $"{recipe.itemsNeeded[i].amount} {recipe.itemsNeeded[i].item.name}\n";
        }
    }

    public void SelectRecipe()
    {
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
        itemImage.sprite = recipe.itemToCraft.image;
        resourceText.text = "";
        for (int i = 0; i < recipe.itemsNeeded.Length; i++)
        {
            resourceText.text += $"{recipe.itemsNeeded[i].amount} {recipe.itemsNeeded[i].item.name}\n";
        }
    }
}
