using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftButton : MonoBehaviour
{
    public Recipe recipe;

    bool isSelected;
    [SerializeField] GameObject _selectedObj;
    [SerializeField] TMP_Text _itemNameText;
    [SerializeField] Image _itemImage;

    void Start()
    {
        if (recipe != null)
        {
            _itemImage.sprite = recipe.itemToCraft.image;
            _itemNameText.text = recipe.itemToCraft.name;
        }
    }

    private void Update()
    {
        isSelected = CraftingManager.Instance.selectedButtonObject == this;
        _selectedObj.SetActive(isSelected);
    }

    public void SelectRecipe()
    {
        UpdateRecipeUI();
        if (CraftingManager.Instance.selectedButtonObject != this)
        {
            CraftingManager.Instance.SelectCraftingRecipe(recipe, this);
        }
        else
        {
            CraftingManager.Instance.SelectCraftingRecipe(null, null);
        }
    }

    public void UpdateRecipeUI()
    {
        if (recipe == null)
        {
            Debug.Log("Could Not Update RecipeUI As There Is No Recipe Attatched To " + gameObject.name);
            return;
        }
        _itemImage.sprite = recipe.itemToCraft.image;
        _itemNameText.text = recipe.itemToCraft.name;
    }
}
