using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftButton : MonoBehaviour
{
    public Recipe recipe;

    bool isSelected;
    [SerializeField] GameObject selectedObj;

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
}
