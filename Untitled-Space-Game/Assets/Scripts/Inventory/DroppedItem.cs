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

    bool _canPickup;

    Transform _child;

    GameObject _lastCollidedObject;

    private void Start()
    {
        _child = GetComponentInChildren<Transform>();
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
    }

    private void OnTriggerEnter(Collider other)
    {
        _lastCollidedObject = other.gameObject;
        print("On Enter Triggered With " + other.gameObject.name);
        if (other.GetComponentInParent<CharStateMachine>() && _canPickup)
        {
            if (InventoryManager.Instance.HasSpace(_item.itemID, _amount))
            {
                InventoryManager.Instance.AddItem(_item.itemID, _amount);
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Player Has No Space!");
            }
        }
        else if (other.GetComponent<DroppedItem>())
        {
            if (other.GetComponent<DroppedItem>()._item == _item)
            {
                _amount += other.GetComponent<DroppedItem>()._amount;
                Destroy(other.gameObject);
                Debug.Log("Merged Two Dropped Items Together");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _lastCollidedObject = null;
    }
}
