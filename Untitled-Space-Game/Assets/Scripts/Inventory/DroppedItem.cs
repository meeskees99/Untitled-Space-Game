using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public List<Item> item = new();
    public List<int> amount = new();

    [SerializeField] float _pickUpDelay = 1.5f;
    [SerializeField] float _rotationSpeed;
    [SerializeField] float _floatAmount;

    [SerializeField] float _mergeDelay = 1f;


    GameObject _lastCollidedObject;

    float timer;

    private void Start()
    {
        if (item.Count == 1)
        {
            if (item[0].name == "Clay")
            {
                timer = item[0].smeltTime * 2;
            }
        }
    }

    private void Update()
    {
        if (item.Count == 1)
        {
            if (_pickUpDelay > 0)
            {
                _pickUpDelay -= Time.deltaTime;
            }
            else
            {
                if (_lastCollidedObject != null && _lastCollidedObject.GetComponent<CharStateMachine>())
                {
                    InventoryManager.Instance.AddItem(item[0].itemID, amount[0]);
                    Destroy(gameObject);
                }
            }
            if (_mergeDelay > 0)
            {
                _mergeDelay -= Time.deltaTime;
            }

            if (item[0].name == "Clay")
            {
                if (timer > 0)
                {
                    timer -= Time.deltaTime;
                }
                else
                {
                    InventoryManager.Instance.DropItem(item[0].itemToGetAfterSmelt.itemID, amount[0], transform);
                    Destroy(gameObject);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        _lastCollidedObject = other.gameObject;
        print("On Enter Triggered With " + other.gameObject.name);

        if (other.GetComponent<DroppedItem>() && amount.Count == 1)
        {
            if (other.GetComponent<DroppedItem>().item == item)
            {
                if (_mergeDelay <= 0)
                {
                    if (amount[0] + other.GetComponent<DroppedItem>().amount[0] <= item[0].maxStack)
                    {
                        amount[0] += other.GetComponent<DroppedItem>().amount[0];
                        Destroy(other.gameObject);
                    }
                    else
                    {
                        int extra = amount[0] + other.GetComponent<DroppedItem>().amount[0] - item[0].maxStack;
                        amount[0] = item[0].maxStack;
                        other.GetComponent<DroppedItem>().amount[0] = extra;
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
