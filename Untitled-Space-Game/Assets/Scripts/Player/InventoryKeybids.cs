using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEditor;
using UnityEngine.UI;

public class InventoryKeybids : MonoBehaviour
{
    [SerializeField] PlayerInput _playerInput;
    public PlayerInput PlayerInput => _playerInput;

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

    [Header("Interaction")]
    [SerializeField] GameObject _InteractPanel;
    [SerializeField] TMP_Text _interactableTxt;

    [SerializeField] float _interactableRadius;
    [SerializeField] RaycastHit _interactableHit;
    [SerializeField] float _interactableRange;
    [SerializeField] LayerMask _interactableMask;

    [SerializeField] bool _didUiInteraction;
    public bool DidUiInteraction { get { return _didUiInteraction; } private set { _didUiInteraction = value; } }

    #endregion

    [Header("Inputs")]
    [SerializeField] bool _onInventory;
    [SerializeField] bool _onInteract;
    [SerializeField] bool _onShoot;
    [SerializeField] bool _onScroll;


    bool _canToggleInventory;

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
                _canToggleInventory = false;
                InGameUIManager.Instance.ToggleInventory();
                _playerInput.actions.FindAction("Inventory").started += OnInventory;
                _playerInput.actions.FindAction("Inventory").canceled += OnInventory;
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
            CheckTool();
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
        }
        #endregion

        if (_onScroll)
        {
            ScrollSelect();
        }

        #endregion
    }

    #region Tools
    private void CheckTool()
    {
        if (InventoryManager.Instance == null || InventoryManager.Instance.GetSelectedItem() == null)
        {
            // Debug.LogError("No selected Item or InventoryManager");
            return;
        }

        // Debug.LogWarning("Checking Tool");

        _camDistance = Vector3.Distance(transform.position, _playerCam.position);

        if (InventoryManager.Instance.GetSelectedItem().name == "Pickaxe")
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

    private void CheckInteractable()
    {
        if (Physics.SphereCast(_playerCam.transform.position, _interactableRadius, _playerCam.transform.forward, out _interactableHit, _interactableRange + _camDistance, _interactableMask))
        {
            if (_interactableHit.transform.GetComponent<DroppedItem>())
            {
                DroppedItem droppedItem = _interactableHit.transform.GetComponent<DroppedItem>();
                _interactableTxt.text = $"Press ({_playerInput.actions.FindAction("Interact").GetBindingDisplayString()}) to pick up " + droppedItem._amount + " " +
                droppedItem._item.name;
                _InteractPanel.SetActive(true);

                if (_onInteract)
                {
                    if (InventoryManager.Instance.HasSpace(droppedItem._item.itemID, droppedItem._amount))
                    {
                        InventoryManager.Instance.AddItem(droppedItem._item.itemID, droppedItem._amount);
                        Destroy(droppedItem.gameObject);
                        Debug.Log($"Pressed {_playerInput.actions.FindAction("Interact").GetBindingDisplayString()} To Pick Up {droppedItem._amount} {droppedItem._item}");
                    }
                    else
                    {
                        Debug.Log($"[NO SPACE] Pressed {_playerInput.actions.FindAction("Interact").GetBindingDisplayString()} To Pick Up {droppedItem._amount} {droppedItem._item}, but had no room in inventory");
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
                    Debug.Log($"Pressed E To Open Mining Panel");
                }
                if (!_didUiInteraction)
                {
                    _interactableTxt.text = "Press (E) to open miner";
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
                    Debug.Log($"Pressed E To Open Mining Panel");
                }
                if (!_didUiInteraction)
                {
                    _interactableTxt.text = "Press (E) to open miner";
                    _InteractPanel.SetActive(true);

                }
                else
                {
                    _interactableTxt.text = "";
                    _InteractPanel.SetActive(false);

                }
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
}