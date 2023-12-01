using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public Item item;
    public int amount;

    public GameObject itemInfoUI;
    public TMP_Text pickupItemText;

    Transform player;

    private void OnEnable()
    {
        pickupItemText.text = $"Press (E) To Pick Up {amount} {item.name}.";
        // if (player == null)
        // {
        //     player = FindObjectOfType<CharStateMachine>().transform;
        // }
    }

    public void SetItemInfoUI(bool value)
    {
        itemInfoUI.SetActive(value);
    }

    private void Update()
    {
        pickupItemText.text = $"Press (E) To Pick Up {amount} {item.name}.";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CharStateMachine>())
        {
            if (InventoryManager.Instance.HasSpace(item.itemID, amount))
            {
                InventoryManager.Instance.AddItem(item.itemID, amount);
            }
            else
            {
                Debug.Log("Player Has No Space!");
            }
        }
    }
}
