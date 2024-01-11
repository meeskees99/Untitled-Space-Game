using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public Item _item;
    public int _amount;

    [SerializeField] float _pickUpDelay = 1.5f;
    [SerializeField] float _rotationSpeed;
    [SerializeField] float _floatAmount;

    [SerializeField] float _mergeDelay = 3f;

    bool _canPickup;

    Transform _child;

    GameObject _lastCollidedObject;

    float timer;

    private void Start()
    {
        _child = GetComponentInChildren<Transform>();

        if (_item.name == "Clay")
        {
            timer = _item.smeltTime * 2;
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
                InventoryManager.Instance.AddItem(_item.itemID, _amount);
                Destroy(gameObject);
            }
        }
        if (_mergeDelay > 0)
        {
            _mergeDelay -= Time.deltaTime;
        }
        if (_item.name == "Clay")
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                InventoryManager.Instance.DropItem(_item.itemToGetAfterSmelt.itemID, _amount, transform);
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
            if (other.GetComponent<DroppedItem>()._item == _item)
            {
                if (_mergeDelay <= 0)
                {
                    if (_amount + other.GetComponent<DroppedItem>()._amount <= _item.maxStack)
                    {
                        _amount += other.GetComponent<DroppedItem>()._amount;
                        Destroy(other.gameObject);
                    }
                    else
                    {
                        int extra = _amount + other.GetComponent<DroppedItem>()._amount - _item.maxStack;
                        _amount = _item.maxStack;
                        other.GetComponent<DroppedItem>()._amount = extra;
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
