using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    public static InGameUIManager Instance;
    [Header("Things To Disable When Not Opened")]
    [SerializeField] Image _inventoryBackgroundImage;
    [SerializeField] Button _inventoryBackgroundButton;
    [SerializeField] Image _inventoryImage;
    [SerializeField] Image[] _inventorySlotImages;

    [Header("Panels")]
    [SerializeField] GameObject _craftingPanel;
    [SerializeField] GameObject _miningPanel;
    [SerializeField] Slider _fuelLeftSlider;

    [Header("Settings")]
    bool _craftingShown;
    public bool inventoryShown { get; private set; }


    bool _initializedUI;

    public bool mouseLocked;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            this.enabled = false;
            Debug.LogError($"Found An Extra InGameUIManager! The Component Has Been Disabled.");
        }
    }

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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (mouseLocked)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            mouseLocked = Cursor.lockState == CursorLockMode.Locked;
        }
    }

    public void SetMinerUIInfo()
    {
        if (_miningPanel.gameObject.activeInHierarchy)
        {
            DiggingMachine[] diggingMachines = FindObjectsOfType<DiggingMachine>();
            foreach (var digger in diggingMachines)
            {
                if (digger.fuelLeftSlider == null)
                    digger.fuelLeftSlider = _fuelLeftSlider;
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
                    if (_inventorySlotImages[i].GetComponent<InventorySlot>().GetInventoryItem().count > 1)
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
