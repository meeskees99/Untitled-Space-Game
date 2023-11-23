using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [Header("Things To Disable When Not Opened")]
    [SerializeField] Image inventoryBackgroundImage;
    [SerializeField] Button inventoryBackgroundButton;
    [SerializeField] Image inventoryImage;
    [SerializeField] Image[] inventorySlotImages;

    [SerializeField] GameObject craftingPanel;

    [Header("Settings")]
    bool inventoryShown;
    bool craftingShown;


    bool initializedUI;


    // Update is called once per frame
    void Update()
    {
        if (!initializedUI)
        {
            initializedUI = true;
            for (int i = 0; i < inventorySlotImages.Length; i++)
            {
                if (inventorySlotImages[i].transform.childCount > 0)
                {
                    Debug.Log("Found A Child");
                    inventorySlotImages[i].transform.GetChild(0).GetComponent<Image>().enabled = false;
                    inventorySlotImages[i].transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                }
            }
        }
    }


    public void ToggleInventory()
    {
        if (craftingShown)
        {
            craftingPanel.SetActive(false);
            craftingShown = false;
        }
        if (inventoryShown)
        {
            inventoryShown = false;
            inventoryBackgroundButton.enabled = false;
            inventoryBackgroundImage.enabled = false;
            inventoryImage.enabled = false;
            for (int i = 0; i < inventorySlotImages.Length; i++)
            {
                inventorySlotImages[i].enabled = false;
                if (inventorySlotImages[i].transform.childCount > 0)
                {
                    Debug.Log("Found A Child");
                    inventorySlotImages[i].transform.GetChild(0).GetComponent<Image>().enabled = false;
                    inventorySlotImages[i].transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                }
            }
        }
        else
        {
            inventoryShown = true;
            inventoryBackgroundButton.enabled = true;
            inventoryBackgroundImage.enabled = true;
            inventoryImage.enabled = true;
            for (int i = 0; i < inventorySlotImages.Length; i++)
            {
                inventorySlotImages[i].enabled = true;
                if (inventorySlotImages[i].transform.childCount > 0)
                {
                    Debug.Log("Found A Child");
                    inventorySlotImages[i].transform.GetChild(0).GetComponent<Image>().enabled = true;
                    if (inventorySlotImages[i].GetComponent<InventorySlot>().itemInThisSlot.count > 1)
                        inventorySlotImages[i].transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                }
            }
        }
    }

    public void ToggleCrafting()
    {
        if (inventoryShown)
        {
            inventoryShown = false;
            inventoryBackgroundButton.enabled = false;
            inventoryBackgroundImage.enabled = false;
            inventoryImage.enabled = false;
            for (int i = 0; i < inventorySlotImages.Length; i++)
            {
                inventorySlotImages[i].enabled = false;
                if (inventorySlotImages[i].transform.childCount > 0)
                {
                    Debug.Log("Found A Child");
                    inventorySlotImages[i].transform.GetChild(0).GetComponent<Image>().enabled = false;
                    inventorySlotImages[i].transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                }
            }
        }
        if (craftingShown)
        {
            craftingPanel.SetActive(false);
            craftingShown = false;
        }
        else
        {
            craftingPanel.SetActive(true);
            craftingShown = true;
        }
    }
}
