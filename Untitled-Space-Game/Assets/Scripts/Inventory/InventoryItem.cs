using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image image;
    public TMP_Text countText;

    public Item item;
    public int count = 1;
    [HideInInspector] public Transform parentAfterDrag;

    public bool isDragging;

    InventoryManager inventoryManager;

    bool dropOnDrop;

    private void Start()
    {
        inventoryManager = InventoryManager.Instance;
    }

    public void InitializeItem(Item newItem)
    {
        if (newItem == null)
        {
            Debug.LogError("No Item To Initialize!");
            return;
        }
        item = newItem;
        image.sprite = newItem.image;
        RefreshCount();
        GetComponentInParent<InventorySlot>().SetInventoryItem(this);
    }

    public void RefreshCount()
    {
        if (count == 0)
        {
            Destroy(gameObject);
        }
        countText.text = count.ToString();
        bool textActive = count > 1;
        countText.gameObject.SetActive(textActive);
        Debug.Log("Refreshed Count Of " + item);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (dropOnDrop)
        {
            InventoryManager.Instance.DropItem(item.itemID, count);
            Destroy(gameObject);
        }
        InventorySlot slot = transform.GetComponentInParent<InventorySlot>();
        slot.SetInventoryItem(null);
        image.raycastTarget = false;
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.position = new Vector3(transform.position.x, transform.position.y, -15);
        isDragging = true;
        InventoryManager.Instance.heldItem = eventData.pointerDrag.GetComponent<InventoryItem>();
        InventoryManager.Instance.UpdateItemsInfoList();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -15);
        InventoryManager.Instance.heldItem = eventData.pointerDrag.GetComponent<InventoryItem>();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dropOnDrop)
        {
            InventoryManager.Instance.DropItem(item.itemID, count);
            Destroy(gameObject);
        }
        else if (parentAfterDrag.childCount == 0)
        {
            image.raycastTarget = true;
            transform.SetParent(parentAfterDrag);
            transform.position = parentAfterDrag.position;
            isDragging = false;
            InventoryManager.Instance.heldItem = null;
            GetComponentInParent<InventorySlot>().SetInventoryItem(this);
            InventoryManager.Instance.UpdateItemsInfoList();
            MiningPanelManager.Instance.currentDigger.InitializeFuelType();
        }
        else
        {
            Debug.Log($"{parentAfterDrag.name} already had a child!");
            if (parentAfterDrag.GetComponent<InventorySlot>().GetInventoryItem().count + count <= item.maxStack)
            {
                parentAfterDrag.GetComponent<InventorySlot>().GetInventoryItem().count += count;
            }
            else
            {
                int overflow = parentAfterDrag.GetComponent<InventorySlot>().GetInventoryItem().count + count - parentAfterDrag.GetComponent<InventorySlot>().GetInventoryItem().item.maxStack;
                parentAfterDrag.GetComponent<InventorySlot>().GetInventoryItem().count = item.maxStack;
                InventoryManager.Instance.AddItem(item.itemID, overflow);
            }
            parentAfterDrag.GetComponent<InventorySlot>().GetInventoryItem().RefreshCount();
            InventoryManager.Instance.UpdateItemsInfoList();
            Destroy(gameObject);
        }
    }

    public void SetItemParent(Transform parent)
    {
        transform.SetParent(parent);
    }

    [Header("UI Raycasting")]
    [SerializeField] GraphicRaycaster _raycaster;
    PointerEventData _pointerEventData;
    [SerializeField] EventSystem _eventSystem;
    [SerializeField] RectTransform _canvasRect;

    private void Update()
    {
        if (isDragging)
        {
            if (_raycaster == null)
            {
                _raycaster = FindObjectOfType<GraphicRaycaster>();
            }
            if (_eventSystem == null)
            {
                _eventSystem = FindObjectOfType<EventSystem>();
            }
            if (_canvasRect == null)
            {
                _canvasRect = GameObject.Find("PlayerCanvas").GetComponent<RectTransform>();
            }
            List<RaycastResult> results = new();
            _pointerEventData = new PointerEventData(_eventSystem);
            _pointerEventData.position = transform.position;
            _raycaster.Raycast(_pointerEventData, results);

            if (results[0].gameObject.transform.GetComponent<Button>())
            {
                Debug.Log("DropOnDrop True");
                dropOnDrop = true;
                return;
            }
            dropOnDrop = false;
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                Debug.Log($"Hit {results[0].gameObject.name}");
                InventorySlot slot;
                results[0].gameObject.transform.TryGetComponent<InventorySlot>(out slot);
                if (slot != null)
                {
                    slot.AddItemToSlot(this);
                    Debug.Log($"Dropped Item In Slot {slot.name}");
                }
                else
                {
                    InventoryItem item;
                    results[0].gameObject.transform.TryGetComponent<InventoryItem>(out item);
                    if (item != null)
                    {
                        if (item.count < item.item.maxStack)
                        {
                            item.count++;
                            item.RefreshCount();
                        }
                        return;
                    }
                    Debug.Log("No Slot Found!");
                }
            }
        }
    }
}
