using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    public static InGameUIManager Instance;

    public Animator inventoryAnimator;

    [Header("Panels")]
    [SerializeField] GameObject _inventoryCanvas;
    [SerializeField] GameObject _craftingPanel;
    [SerializeField] GameObject _shipRepairPanel;
    [SerializeField] GameObject _miningPanel;
    [SerializeField] GameObject _deathPanel;
    [SerializeField] GameObject _pausePanel;

    [Header("UI")]
    [SerializeField] TMP_Text _diedTxt;
    [SerializeField] Button _respawnButton;

    [Header("Settings")]
    bool _craftingShown;
    bool _shipRepairShown;
    public bool inventoryShown { get; private set; }

    public bool mouseLocked;

    bool _gamePaused;

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
        mouseLocked = Cursor.lockState == CursorLockMode.Locked;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!_gamePaused)
            {
                Time.timeScale = 0;
                _pausePanel.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                FindObjectOfType<PlayerInput>().SwitchCurrentActionMap("Menu");
            }
            else
            {
                Time.timeScale = 1;
                _pausePanel.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                FindObjectOfType<PlayerInput>().SwitchCurrentActionMap("Game");
            }
            _gamePaused = !_gamePaused;
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
            Cursor.lockState = CursorLockMode.Locked;
            mouseLocked = true;
            FindObjectOfType<PlayerInput>().SwitchCurrentActionMap("Game");
            inventoryShown = false;
            _inventoryCanvas.GetComponent<Canvas>().enabled = false;
            _inventoryCanvas.GetComponent<GraphicRaycaster>().enabled = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            mouseLocked = false;
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
            Cursor.lockState = CursorLockMode.Locked;
            FindObjectOfType<PlayerInput>().SwitchCurrentActionMap("Game");
            _craftingPanel.SetActive(false);
            _craftingShown = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            FindObjectOfType<PlayerInput>().SwitchCurrentActionMap("Menu");
            _craftingPanel.SetActive(true);
            _craftingShown = true;
        }
    }

    public void ToggleShipRepair()
    {
        inventoryAnimator.SetTrigger("SwitchInventoryType");
        if (_shipRepairShown)
        {
            if (inventoryShown)
            {
                ToggleInventory();
            }
            Cursor.lockState = CursorLockMode.Locked;
            FindObjectOfType<PlayerInput>().SwitchCurrentActionMap("Game");
            _shipRepairShown = false;
            _shipRepairPanel.GetComponent<Canvas>().enabled = false;
            _shipRepairPanel.GetComponent<GraphicRaycaster>().enabled = false;
        }
        else
        {
            if (!inventoryShown)
            {
                ToggleInventory();
            }
            Cursor.lockState = CursorLockMode.None;
            FindObjectOfType<PlayerInput>().SwitchCurrentActionMap("Menu");
            _shipRepairShown = true;
            _shipRepairPanel.GetComponent<Canvas>().enabled = true;
            _shipRepairPanel.GetComponent<GraphicRaycaster>().enabled = true;
        }
    }

    public void Die(int difficulty = -1)
    {
        Cursor.lockState = CursorLockMode.None;
        FindObjectOfType<PlayerInput>().SwitchCurrentActionMap("Menu");

        _deathPanel.SetActive(true);
        if (difficulty == 2)
        {
            _respawnButton.interactable = false;
        }
    }

    public void Respawn()
    {
        FindObjectOfType<PlayerStats>().ResetStats();
    }

    public void MainMenu()
    {

    }
}
