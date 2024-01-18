using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public Item item;
    public int amount;

    [SerializeField] float _pickUpDelay = 1.5f;
    [SerializeField] float _rotationSpeed;
    [SerializeField] float _floatAmount;

    [SerializeField] float _mergeDelay = 1f;

    bool _canPickup;

    Transform _child;

    GameObject _lastCollidedObject;

    float timer;

    private void Start()
    {
        _child = GetComponentInChildren<Transform>();

        if (item.name == "Clay")
        {
            timer = item.smeltTime * 2;
        }
    }

    private void Update()
    {
        if (_pickUpDelay > 0)
        {
            _pickUpDelay -= Time.deltaTime;
        }
        else
        {
            _canPickup = true;
            if (_lastCollidedObject != null && _lastCollidedObject.GetComponent<CharStateMachine>())
            {
                InventoryManager.Instance.AddItem(item.itemID, amount);
                Destroy(gameObject);
            }
        }
        if (_mergeDelay > 0)
        {
            _mergeDelay -= Time.deltaTime;
        }
        if (item.name == "Clay")
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                InventoryManager.Instance.DropItem(item.itemToGetAfterSmelt.itemID, amount, transform);
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        _lastCollidedObject = other.gameObject;
        print("On Enter Triggered With " + other.gameObject.name);

        if (other.GetComponent<DroppedItem>())
        {
            if (other.GetComponent<DroppedItem>().item == item)
            {
                if (_mergeDelay <= 0)
                {
                    if (amount + other.GetComponent<DroppedItem>().amount <= item.maxStack)
                    {
                        amount += other.GetComponent<DroppedItem>().amount;
                        Destroy(other.gameObject);
                    }
                    else
                    {
                        int extra = amount + other.GetComponent<DroppedItem>().amount - item.maxStack;
                        amount = item.maxStack;
                        other.GetComponent<DroppedItem>().amount = extra;
                    }

                    Debug.Log("Merged Two Dropped Items Together");
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _lastCollidedObject = null;
    }
}
