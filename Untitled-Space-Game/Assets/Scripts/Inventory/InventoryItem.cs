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

    [HideInInspector] public Item item;
    public int count = 1;
    [HideInInspector] public Transform parentAfterDrag;

    public bool isDragging;

    InventoryManager inventoryManager;

    private void Start()
    {
        inventoryManager = InventoryManager.Instance;
    }

    public void InitializeItem(Item newItem)
    {
        item = newItem;
        image.sprite = newItem.image;
        RefreshCount();
        GetComponentInParent<InventorySlot>().itemInThisSlot = this;
    }

    public void RefreshCount()
    {
        countText.text = count.ToString();
        bool textActive = count > 1;
        countText.gameObject.SetActive(textActive);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        InventorySlot slot = transform.GetComponentInParent<InventorySlot>();
        slot.itemInThisSlot = null;
        image.raycastTarget = false;
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.position = new Vector3(transform.position.x, transform.position.y, 5);
        isDragging = true;
        InventoryManager.Instance.heldItem = eventData.pointerDrag.GetComponent<InventoryItem>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 5);
        InventoryManager.Instance.heldItem = eventData.pointerDrag.GetComponent<InventoryItem>();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        image.raycastTarget = true;
        transform.SetParent(parentAfterDrag);
        transform.position = parentAfterDrag.position;
        isDragging = false;
        InventoryManager.Instance.heldItem = null;
    }

    public void SetItemParent(Transform parent)
    {
        transform.SetParent(parent);
    }


    private void Update()
    {

        if (isDragging)
        {
            Debug.DrawRay(transform.position, Vector3.back, Color.red);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(Input.mousePosition, Vector3.forward, Color.green);
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                Debug.Log("Mouse 1");
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100))
                {
                    Debug.Log("Dropping Item In Slot...");
                    InventorySlot slot;
                    hit.transform.TryGetComponent<InventorySlot>(out slot);
                    if (slot != null)
                    {
                        slot.AddItemToSlot(this);
                        Debug.Log($"Dropped Item In Slot {slot.name}");
                    }
                    else
                    {
                        Debug.Log("No Slot Found!");
                    }
                }
            }
        }
    }
}
