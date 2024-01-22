using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEditor;
using UnityEngine.UI;
using Unity.VisualScripting;

public class InventoryKeybids : MonoBehaviour
{
    public static InventoryKeybids Instance;

    [SerializeField] PlayerInput _playerInput;
    public PlayerInput PlayerInput => _playerInput;

    [SerializeField] MachinePlacement machinePlacement;

    [SerializeField] PlayerStats _playerStats;

    #region Interactions
    [SerializeField] Transform _playerCam;
    [SerializeField] float _camDistance;

    [SerializeField] float _gatherTime;

    [SerializeField] float _toolRange;
    [SerializeField] RaycastHit _toolHit;
    Transform _lastHitResource;
    Transform _lastHitEnemy;

    bool _doingDamage;

    [SerializeField] float _rayRange;
    [SerializeField] float _rayRadius;

    [Header("Interaction")]
    [SerializeField] GameObject _InteractPanel;
    [SerializeField] TMP_Text _interactableTxt;

    [SerializeField] RaycastHit _interactableHit;
    [SerializeField] LayerMask _interactableMask;

    [SerializeField] bool _didUiInteraction;
    public bool DidUiInteraction { get { return _didUiInteraction; } private set { _didUiInteraction = value; } }

    [Header("Placing")]
    [SerializeField] RaycastHit _placeableHit;
    [SerializeField] LayerMask _placeableMask;


    #endregion

    [Header("Inputs")]
    [SerializeField] bool _onInventory;
    [SerializeField] bool _onInteract;
    [SerializeField] bool _onShoot;
    [SerializeField] bool _onScroll;


    bool _canToggleInventory;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

    }

    private void OnEnable()
    {
        _playerInput.actions.FindAction("Inventory").started += OnInventory;
        _playerInput.actions.FindAction("Inventory").performed += OnInventory;
        _playerInput.actions.FindAction("Inventory").canceled += OnInventory;

        _playerInput.actions.FindAction("Interact").started += OnInteract;
        _playerInput.actions.FindAction("Interact").performed += OnInteract;
        _playerInput.actions.FindAction("Interact").canceled += OnInteract;

        _playerInput.actions.FindAction("Use").started += OnShoot;
        _playerInput.actions.FindAction("Use").performed += OnShoot;
        _playerInput.actions.FindAction("Use").canceled += OnShoot;

        _playerInput.actions.FindAction("Hotbar").started += OnScroll;
        _playerInput.actions.FindAction("Hotbar").performed += OnScroll;
        _playerInput.actions.FindAction("Hotbar").canceled += OnScroll;
    }



    // private void OnDisable()
    // {
    //     _playerInput.actions.FindAction("Inventory").started -= OnInventory;
    //     _playerInput.actions.FindAction("Inventory").performed -= OnInventory;
    //     _playerInput.actions.FindAction("Inventory").canceled -= OnInventory;

    //     _playerInput.actions.FindAction("Interact").started -= OnInteract;
    //     _playerInput.actions.FindAction("Interact").performed -= OnInteract;
    //     _playerInput.actions.FindAction("Interact").canceled -= OnInteract;

    //     _playerInput.actions.FindAction("Use").started -= OnShoot;
    //     _playerInput.actions.FindAction("Use").performed -= OnShoot;
    //     _playerInput.actions.FindAction("Use").canceled -= OnShoot;

    // }

    private void Start()
    {
        _playerStats = GetComponent<PlayerStats>();
    }

    private void Update()
    {
        #region Inputs
        if (_onInventory)
        {
            if (_canToggleInventory)
            {
                if (MiningPanelManager.Instance.panelActive)
                {
                    MiningPanelManager.Instance.ToggleMiningPanel(null);
                }
                else if (SmeltingPanelManager.Instance.panelActive)
                {
                    SmeltingPanelManager.Instance.ToggleSmeltingPanel(null);
                }
                else
                {
                    // if (InGameUIManager.Instance.inventoryShown)
                    InGameUIManager.Instance.ToggleInventory();

                    InventorySubscribe();
                }
                _canToggleInventory = false;

            }
        }
        else
        {
            _canToggleInventory = true;
        }

        CheckInteractable();

        #region On Shoot
        if (_onShoot)
        {
            CheckUsable();
        }
        else
        {
            if (InventoryManager.Instance == null || InventoryManager.Instance.GetSelectedItem() == null)
            {
                if (_lastHitResource != null)
                {
                    _lastHitResource.GetComponent<Outline>().enabled = false;
                    _lastHitResource = null;
                }
                if (_lastHitEnemy != null)
                {
                    // Debug.LogWarning("CancelInvoke(LaserEnemy)");
                    CancelInvoke("LaserEnemy");
                    _doingDamage = false;
                    _lastHitEnemy = null;
                }
                if (machinePlacement.selectedPrefab != null)
                {
                    machinePlacement.PickMachine(null, _interactableHit);
                }
                return;
            }
            else if (InventoryManager.Instance.GetSelectedItem().name == "Pickaxe")
            {
                if (Physics.Raycast(_playerCam.position, _playerCam.forward, out _toolHit, _toolRange + _camDistance))
                {
                    if (_toolHit.transform.gameObject.layer == LayerMask.NameToLayer("Resource"))
                    {
                        _lastHitResource = _toolHit.transform;
                        _lastHitResource.GetComponent<Outline>().enabled = true;
                        return;
                    }
                    else
                    {
                        if (_lastHitResource != null)
                        {
                            _lastHitResource.GetComponent<Outline>().enabled = false;
                            _lastHitResource = null;
                        }
                    }

                    if (_toolHit.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                    {
                        _lastHitEnemy = _toolHit.transform;
                        return;
                    }
                    else
                    {
                        if (_lastHitEnemy != null)
                        {
                            // Debug.LogWarning("CancelInvoke(LaserEnemy)");
                            _doingDamage = false;
                            CancelInvoke("LaserEnemy");
                            _lastHitEnemy = null;
                        }
                    }
                }
            }
            else if (InventoryManager.Instance.GetSelectedItem().name == "Smelter" || InventoryManager.Instance.GetSelectedItem().name == "Miner")
            {
                CheckPlaceable();
            }
        }
        #endregion

        if (_onScroll)
        {
            ScrollSelect();
        }

        #endregion
    }

    private void CheckUsable()
    {
        if (InventoryManager.Instance == null || InventoryManager.Instance.GetSelectedItem() == null)
        {
            // Debug.LogError("No selected Item or InventoryManager");
            return;
        }

        if (InventoryManager.Instance.GetSelectedItem().isTool)
        {
            CheckTool();
        }
        else if (InventoryManager.Instance.GetSelectedItem().isPlacable)
        {
            Place();
        }


        _camDistance = Vector3.Distance(transform.position, _playerCam.position);
    }

    #region Tools

    private void CheckTool()
    {
        if (Physics.Raycast(_playerCam.position, _playerCam.forward, out _toolHit, _toolRange + _camDistance))
        {
            if (_toolHit.transform.gameObject.layer == LayerMask.NameToLayer("Resource"))
            {
                GatherTool();
                _lastHitResource = _toolHit.transform;
                _lastHitResource.GetComponent<Outline>().enabled = true;
            }
            else if (_toolHit.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                // Debug.Log("Hitting Enemy");
                _lastHitEnemy = _toolHit.transform;
                WeaponTool();
            }
            else
            {
                if (_lastHitResource != null)
                {
                    _lastHitResource.GetComponent<Outline>().enabled = false;
                    _lastHitResource = null;
                }
            }
        }
        else
        {
            Debug.Log("Raycast Missed");
        }
    }

    private void GatherTool()
    {
        if (_gatherTime == -1)
        {
            _gatherTime = _toolHit.transform.GetComponent<ResourceVein>().Resource.mineDuration;
        }
        else
        {
            _gatherTime -= Time.deltaTime;

            if (_gatherTime <= 0)
            {
                InventoryManager.Instance.AddItem(_toolHit.transform.GetComponent<ResourceVein>().Resource.item.itemID, _toolHit.transform.GetComponent<ResourceVein>().Resource.recourceAmount);
                _gatherTime = _toolHit.transform.GetComponent<ResourceVein>().Resource.mineDuration;
            }
        }
    }

    private void WeaponTool()
    {
        // Debug.LogWarning("InvokeRepeating(LaserEnemy)");
        if (!_doingDamage)
        {
            InvokeRepeating("LaserEnemy", 0.01f, _playerStats.AttackSpeed);
            _doingDamage = true;
        }
    }


    void LaserEnemy()
    {
        if (_onShoot)
        {
            _lastHitEnemy.GetComponent<Enemy>().TakeDamage(_playerStats.AttackDamage);
            Debug.Log($"Enemy Health: {_lastHitEnemy.GetComponent<Enemy>().Health}");
        }
        else
        {
            // Debug.LogWarning("CancelInvoke(LaserEnemy)");
            _doingDamage = false;
            CancelInvoke("LaserEnemy");
        }

    }

    #endregion

    private void Place()
    {
        if (_interactableHit.point != null)
        {
            Debug.Log(_interactableHit);
            machinePlacement.PlaceMachine(_interactableHit);
        }
        else
        {
            Debug.LogError($"Tried To Place Machine But _interactibleHit was Null");
        }
    }

    private void CheckInteractable()
    {
        if (Physics.SphereCast(_playerCam.transform.position, _rayRadius, _playerCam.transform.forward, out _interactableHit, _rayRange + _camDistance))
        {
            if (_interactableHit.transform.GetComponent<DroppedItem>())
            {
                DroppedItem droppedItem = _interactableHit.transform.GetComponent<DroppedItem>();
                _interactableTxt.text = $"Press ({_playerInput.actions.FindAction("Interact").GetBindingDisplayString()}) to pick up {droppedItem.amount} {droppedItem.item.name}";
                _InteractPanel.SetActive(true);

                if (_onInteract)
                {
                    if (droppedItem.amount > droppedItem.item.maxStack)
                    {
                        if (InventoryManager.Instance.HasSpace(droppedItem.item.itemID, droppedItem.amount))
                        {
                            InventoryManager.Instance.AddItem(droppedItem.item.itemID, droppedItem.amount);
                            droppedItem.amount -= droppedItem.amount;
                        }
                    }
                    else if (InventoryManager.Instance.HasSpace(droppedItem.item.itemID, droppedItem.amount))
                    {
                        InventoryManager.Instance.AddItem(droppedItem.item.itemID, droppedItem.amount);
                        Destroy(droppedItem.gameObject);
                        Debug.Log($"Pressed {_playerInput.actions.FindAction("Interact").GetBindingDisplayString()} To Pick Up {droppedItem.amount} {droppedItem.item}");
                    }
                    else
                    {
                        Debug.Log($"[NO SPACE] Pressed {_playerInput.actions.FindAction("Interact").GetBindingDisplayString()} To Pick Up {droppedItem.amount} {droppedItem.item}, but had no room in inventory");
                    }

                }
            }
            else if (_interactableHit.transform.GetComponent<DiggingMachine>())
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    MiningPanelManager.Instance.ToggleMiningPanel(_interactableHit.transform.GetComponent<DiggingMachine>());
                    _InteractPanel.SetActive(_InteractPanel.activeSelf);
                    _didUiInteraction = !_didUiInteraction;
                    Debug.Log($"Pressed {_playerInput.actions.FindAction("Interact").GetBindingDisplayString()} To Open Mining Panel");
                }
                if (!_didUiInteraction)
                {
                    _interactableTxt.text = $"Press {_playerInput.actions.FindAction("Interact").GetBindingDisplayString()} to open miner";
                    _InteractPanel.SetActive(true);

                }
                else
                {
                    _interactableTxt.text = "";
                    _InteractPanel.SetActive(false);

                }
            }
            else if (_interactableHit.transform.GetComponent<SmeltingMachine>())
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    SmeltingPanelManager.Instance.ToggleSmeltingPanel(_interactableHit.transform.GetComponent<SmeltingMachine>());
                    _InteractPanel.SetActive(_InteractPanel.activeSelf);
                    _didUiInteraction = !_didUiInteraction;
                    Debug.Log($"Pressed {_playerInput.actions.FindAction("Interact").GetBindingDisplayString()} To Open Smelting Panel");
                }
                if (!_didUiInteraction)
                {
                    _interactableTxt.text = $"Press {_playerInput.actions.FindAction("Interact").GetBindingDisplayString()} to open smelter";
                    _InteractPanel.SetActive(true);

                }
                else
                {
                    _interactableTxt.text = "";
                    _InteractPanel.SetActive(false);


                }
            }
            else if (LayerMask.LayerToName(_interactableHit.transform.gameObject.layer) == "CraftingTable")
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    InGameUIManager.Instance.ToggleCrafting();
                    _InteractPanel.SetActive(!_InteractPanel.activeSelf);
                    _didUiInteraction = !_didUiInteraction;
                    Debug.Log($"Pressed {_playerInput.actions.FindAction("Interact").GetBindingDisplayString()} To Open Crafting Panel");
                }
                if (!_didUiInteraction)
                {
                    _interactableTxt.text = $"Press {_playerInput.actions.FindAction("Interact").GetBindingDisplayString()} to open the crafting table";
                    _InteractPanel.SetActive(true);

                }
                else
                {
                    _interactableTxt.text = "";
                    _InteractPanel.SetActive(false);

                }
            }
            else
            {
                _interactableTxt.text = "";
                _InteractPanel.SetActive(false);
            }
        }
        else
        {
            if (_InteractPanel != null)
            {
                _InteractPanel.SetActive(false);
            }
        }
    }

    private void CheckPlaceable()
    {
        if (Physics.SphereCast(_playerCam.transform.position, _rayRadius, _playerCam.transform.forward, out _placeableHit, _rayRange + _camDistance))
        {
            print("PlacingMachine");
            machinePlacement.PickMachine(InventoryManager.Instance.GetSelectedItem(), _placeableHit);
        }
        else
        {
            machinePlacement.PickMachine(null, _placeableHit);
        }
    }

    float scrollValue;

    void ScrollSelect()
    {
        print("Scroll Value: " + scrollValue);
    }

    #region Inputs

    void OnInventory(InputAction.CallbackContext context)
    {
        _onInventory = context.ReadValueAsButton();
    }

    void OnInteract(InputAction.CallbackContext context)
    {
        _onInteract = context.ReadValueAsButton();
    }

    void OnShoot(InputAction.CallbackContext context)
    {
        _onShoot = context.ReadValueAsButton();
    }

    void OnScroll(InputAction.CallbackContext context)
    {
        _onScroll = context.ReadValueAsButton();
        scrollValue = context.ReadValue<float>();
    }

    #endregion

    public void InventorySubscribe()
    {
        _playerInput.actions.FindAction("Inventory").started += OnInventory;
        _playerInput.actions.FindAction("Inventory").canceled += OnInventory;
    }
}