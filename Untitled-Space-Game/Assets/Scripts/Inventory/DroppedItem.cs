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
    }

    public void SetItemInfoUI(bool value)
    {
        itemInfoUI.SetActive(value);
    }

    private void Update()
    {
        if (itemInfoUI.activeSelf)
        {
            itemInfoUI.transform.LookAt(player.position);
        }
    }
}
