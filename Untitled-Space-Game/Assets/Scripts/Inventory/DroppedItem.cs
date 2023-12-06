using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public Item item;
    public int amount;

    [SerializeField] float pickUpDelay = 1.5f;

    bool canPickup;

    private void Update()
    {
        if (pickUpDelay > 0)
        {
            pickUpDelay -= Time.deltaTime;
        }
        else
        {
            canPickup = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        print("On Enter Collided With " + other.gameObject.name);
        if (other.GetComponentInParent<CharStateMachine>() && canPickup)
        {
            if (InventoryManager.Instance.HasSpace(item.itemID, amount))
            {
                InventoryManager.Instance.AddItem(item.itemID, amount);
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Player Has No Space!");
            }
        }
        else if (other.GetComponent<DroppedItem>())
        {
            if (other.GetComponent<DroppedItem>().item == item)
            {
                amount += other.GetComponent<DroppedItem>().amount;
                Destroy(other.gameObject);
                Debug.Log("Merged Two Dropped Items Together");
            }
        }
    }
}
