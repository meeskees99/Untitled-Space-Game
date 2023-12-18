using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEditor;

public class InventoryKeybids : MonoBehaviour
{
    [SerializeField] PlayerInput _playerInput;
    public PlayerInput PlayerInput => _playerInput;

    #region Interactions
    [SerializeField] Transform _playerCam;
    [SerializeField] float _camDistance;

    [SerializeField] float _gatherTime;

    [SerializeField] float _toolRange;
    [SerializeField] RaycastHit _toolHit;
    [SerializeField] LayerMask _gatherMask;

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


    bool _canToggleInventory;

    private void OnEnable()
    {
        _playerInput.actions.FindAction("Inventory").started += OnInventory;
        // _playerInput.actions.FindAction("Inventory").performed += OnInventory;
        _playerInput.actions.FindAction("Inventory").canceled += OnInventory;

        _playerInput.actions.FindAction("Interact").started += OnInteract;
        _playerInput.actions.FindAction("Interact").performed += OnInteract;
        _playerInput.actions.FindAction("Interact").canceled += OnInteract;

        _playerInput.actions.FindAction("Use").started += OnShoot;
        _playerInput.actions.FindAction("Use").performed += OnShoot;
        _playerInput.actions.FindAction("Use").canceled += OnShoot;
    }

    private void OnDisable()
    {
        _playerInput.actions.FindAction("Inventory").started -= OnInventory;
        // _playerInput.actions.FindAction("Inventory").performed -= OnInventory;
        _playerInput.actions.FindAction("Inventory").canceled -= OnInventory;

        _playerInput.actions.FindAction("Interact").started -= OnInteract;
        _playerInput.actions.FindAction("Interact").performed -= OnInteract;
        _playerInput.actions.FindAction("Interact").canceled -= OnInteract;

        _playerInput.actions.FindAction("Use").started -= OnShoot;
        _playerInput.actions.FindAction("Use").performed -= OnShoot;
        _playerInput.actions.FindAction("Use").canceled -= OnShoot;

    }

    private void Start()
    {

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
            }
        }
        else
        {
            _canToggleInventory = true;
        }
        if (_onInteract)
        {
            CheckInteractable();
        }
        if (_onShoot)
        {
            CheckTool();
        }

        #endregion
    }

    #region Tools
    private void CheckTool()
    {
        if (InventoryManager.Instance == null || InventoryManager.Instance.GetSelectedItem() == null)
        {
            return;
        }

        _camDistance = Vector3.Distance(transform.position, _playerCam.position);

        if (InventoryManager.Instance.GetSelectedItem().name == "Pickaxe")
        {
            if (Physics.Raycast(_playerCam.position, Vector3.forward, out _toolHit, _toolRange + _camDistance))
            {
                if (_toolHit.transform.gameObject.layer == LayerMask.NameToLayer("Resource"))
                {
                    GatherTool();
                }
                else if (_toolHit.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    WeaponTool();
                }
            }
        }
    }

    private void GatherTool()
    {
        if (Physics.Raycast(_playerCam.position, Vector3.forward, out _toolHit, _toolRange + _camDistance, _gatherMask))
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
    }

    private void WeaponTool()
    {
        // TODO - make weapon variables
        // float rayDistance = _toolRange += Vector3.Distance(_playerCam.transform.position, _playerObj.transform.position);
        // if (Physics.Raycast(_playerCam.position, Vector3.forward, out _toolHit, rayDistance, _gatherMask))
        // {

        // }
    }
    #endregion

    private void CheckInteractable()
    {
        if (Physics.SphereCast(_playerCam.transform.position, _interactableRadius, _playerCam.transform.forward, out _interactableHit, _interactableRange + _camDistance, _interactableMask))
        {
            if (_interactableHit.transform.GetComponent<DroppedItem>())
            {
                DroppedItem droppedItem = _interactableHit.transform.GetComponent<DroppedItem>();
                _interactableTxt.text = "Press (E) to pick up " + droppedItem._amount + " " +
                droppedItem._item.name;
                _InteractPanel.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (InventoryManager.Instance.HasSpace(droppedItem._item.itemID, droppedItem._amount))
                    {
                        InventoryManager.Instance.AddItem(droppedItem._item.itemID, droppedItem._amount);
                        Destroy(droppedItem.gameObject);
                        Debug.Log($"Pressed E To Pick Up {droppedItem._amount} {droppedItem._item}");
                    }
                    else
                    {
                        Debug.Log($"[NO SPACE] Pressed E To Pick Up {droppedItem._amount} {droppedItem._item}, but had no room in inventory");
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
    #endregion
}