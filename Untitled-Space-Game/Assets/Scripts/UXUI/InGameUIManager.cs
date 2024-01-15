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
    [SerializeField] GameObject _inventoryCanvas;

    [Header("Panels")]
    [SerializeField] GameObject _craftingPanel;
    [SerializeField] GameObject _miningPanel;
    [SerializeField] Slider _fuelLeftSlider;


    public Animator animator;

    public GameObject deathPanel;

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

    public void ToggleInventory()
    {
        if (_craftingShown)
        {
            _craftingPanel.SetActive(false);
            _craftingShown = false;
        }
        if (inventoryShown)
        {
            FindObjectOfType<PlayerInput>().SwitchCurrentActionMap("Game");
            inventoryShown = false;
            _inventoryCanvas.GetComponent<Canvas>().enabled = false;
            _inventoryCanvas.GetComponent<GraphicRaycaster>().enabled = false;
        }
        else
        {
            FindObjectOfType<PlayerInput>().SwitchCurrentActionMap("Menu");
            inventoryShown = true;

            _inventoryCanvas.GetComponent<Canvas>().enabled = true;
            _inventoryCanvas.GetComponent<GraphicRaycaster>().enabled = true;
        }
    }

    public void ToggleCrafting()
    {
        if (inventoryShown)
        {
            inventoryShown = false;
            _inventoryCanvas.GetComponent<Canvas>().enabled = false;
            _inventoryCanvas.GetComponent<GraphicRaycaster>().enabled = false;
        }
        if (_craftingShown)
        {
            FindObjectOfType<PlayerInput>().SwitchCurrentActionMap("Game");
            _craftingPanel.SetActive(false);
            _craftingShown = false;
        }
        else
        {
            FindObjectOfType<PlayerInput>().SwitchCurrentActionMap("Menu");
            _craftingPanel.SetActive(true);
            _craftingShown = true;
        }
    }
}
