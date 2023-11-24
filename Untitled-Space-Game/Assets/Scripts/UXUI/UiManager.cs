using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [Header("Things To Disable When Not Opened")]
    [SerializeField] Image _inventoryBackgroundImage;
    [SerializeField] Button _inventoryBackgroundButton;
    [SerializeField] Image _inventoryImage;
    [SerializeField] Image[] _inventorySlotImages;

    [SerializeField] GameObject _craftingPanel;

    [Header("Settings")]
    bool _craftingShown;
    public bool inventoryShown { get; private set; }


    bool _initializedUI;


    // Update is called once per frame
    void Update()
    {
        if (!_initializedUI)
        {
            _initializedUI = true;
            for (int i = 0; i < _inventorySlotImages.Length; i++)
            {
                if (_inventorySlotImages[i].transform.childCount > 0)
                {
                    Debug.Log("Found A Child");
                    _inventorySlotImages[i].transform.GetChild(0).GetComponent<Image>().enabled = false;
                    _inventorySlotImages[i].transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                }
            }
        }
    }


    public void ToggleInventory()
    {
        if (_craftingShown)
        {
            _craftingPanel.SetActive(false);
            _craftingShown = false;
        }
        if (inventoryShown)
        {
            inventoryShown = false;
            _inventoryBackgroundButton.enabled = false;
            _inventoryBackgroundImage.enabled = false;
            _inventoryImage.enabled = false;
            for (int i = 0; i < _inventorySlotImages.Length; i++)
            {
                _inventorySlotImages[i].enabled = false;
                if (_inventorySlotImages[i].transform.childCount > 0)
                {
                    Debug.Log("Found A Child");
                    _inventorySlotImages[i].transform.GetChild(0).GetComponent<Image>().enabled = false;
                    _inventorySlotImages[i].transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                }
            }
        }
        else
        {
            inventoryShown = true;
            _inventoryBackgroundButton.enabled = true;
            _inventoryBackgroundImage.enabled = true;
            _inventoryImage.enabled = true;
            for (int i = 0; i < _inventorySlotImages.Length; i++)
            {
                _inventorySlotImages[i].enabled = true;
                if (_inventorySlotImages[i].transform.childCount > 0)
                {
                    Debug.Log("Found A Child");
                    _inventorySlotImages[i].transform.GetChild(0).GetComponent<Image>().enabled = true;
                    if (_inventorySlotImages[i].GetComponent<InventorySlot>().itemInThisSlot.count > 1)
                        _inventorySlotImages[i].transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                }
            }
        }
    }

    public void ToggleCrafting()
    {
        if (inventoryShown)
        {
            inventoryShown = false;
            _inventoryBackgroundButton.enabled = false;
            _inventoryBackgroundImage.enabled = false;
            _inventoryImage.enabled = false;
            for (int i = 0; i < _inventorySlotImages.Length; i++)
            {
                _inventorySlotImages[i].enabled = false;
                if (_inventorySlotImages[i].transform.childCount > 0)
                {
                    Debug.Log("Found A Child");
                    _inventorySlotImages[i].transform.GetChild(0).GetComponent<Image>().enabled = false;
                    _inventorySlotImages[i].transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                }
            }
        }
        if (_craftingShown)
        {
            _craftingPanel.SetActive(false);
            _craftingShown = false;
        }
        else
        {
            _craftingPanel.SetActive(true);
            _craftingShown = true;
        }
    }
}
