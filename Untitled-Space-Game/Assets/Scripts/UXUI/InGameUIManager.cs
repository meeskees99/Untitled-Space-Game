using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class InGameUIManager : MonoBehaviour
{
    public static InGameUIManager Instance;

    [Header("Panels")]
    [SerializeField] GameObject _inventoryCanvas;
    [SerializeField] GameObject _craftingPanel;
    [SerializeField] GameObject _shipRepairPanel;
    [SerializeField] GameObject _miningPanel;
    [SerializeField] Slider _fuelLeftSlider;
    [SerializeField] GameObject _deathPanel;
    [SerializeField] Button _respawnBtn;

    [Header("Settings")]
    [SerializeField] string _sceneToLoad;



    public Animator inventoryAnimator;


    [Header("Settings")]
    bool _craftingShown;
    bool _shipRepairShown;
    public bool inventoryShown { get; private set; }

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

    private void Start()
    {
        FindObjectOfType<PlayerInput>().SwitchCurrentActionMap("Game");
        Cursor.lockState = CursorLockMode.Locked;
        mouseLocked = true;
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

            Cursor.lockState = CursorLockMode.Locked;
            mouseLocked = true;
        }
        else
        {
            FindObjectOfType<PlayerInput>().SwitchCurrentActionMap("Menu");
            inventoryShown = true;

            _inventoryCanvas.GetComponent<Canvas>().enabled = true;
            _inventoryCanvas.GetComponent<GraphicRaycaster>().enabled = true;

            Cursor.lockState = CursorLockMode.None;
            mouseLocked = false;
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

            Cursor.lockState = CursorLockMode.Locked;
            mouseLocked = true;
        }
        else
        {
            FindObjectOfType<PlayerInput>().SwitchCurrentActionMap("Menu");
            _craftingPanel.SetActive(true);
            _craftingShown = true;

            Cursor.lockState = CursorLockMode.None;
            mouseLocked = false;
        }
    }

    public void ToggleShipRepair()
    {
        inventoryAnimator.SetTrigger("SwitchInventoryType");
        if (_shipRepairShown)
        {
            Cursor.lockState = CursorLockMode.Locked;
            mouseLocked = true;

            if (inventoryShown)
            {
                ToggleInventory();
            }
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
            mouseLocked = false;

            FindObjectOfType<PlayerInput>().SwitchCurrentActionMap("Menu");
            _shipRepairShown = true;
            _shipRepairPanel.GetComponent<Canvas>().enabled = true;
            _shipRepairPanel.GetComponent<GraphicRaycaster>().enabled = true;
        }
    }

    public void Die(int gameDifficulty = -1)
    {

        _deathPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        mouseLocked = false;
        if (gameDifficulty == 2)
        {
            _respawnBtn.interactable = false;
        }
        FindObjectOfType<PlayerInput>().SwitchCurrentActionMap("Menu");
    }

    public void Respawn()
    {
        FindObjectOfType<PlayerStats>().ResetStats();
        FindObjectOfType<PlayerInput>().SwitchCurrentActionMap("Game");
        Cursor.lockState = CursorLockMode.Locked;
        mouseLocked = true;
    }


    public void MainMenu()
    {
        DataPersistenceManager.instance.SaveGame();

        SceneManager.LoadSceneAsync(_sceneToLoad);
    }

}
