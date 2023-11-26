using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using System;
public class SaveSlot : MonoBehaviour
{
    [Header("Profile")]
    [SerializeField] string _profileId = "";

    [Header("Content")]
    [SerializeField] Button _saveFileButton;
    [SerializeField] TMP_Text _saveFileNameTxt;
    [SerializeField] TMP_Text _saveFileLastPlayedTxt;

    public void SetData(GameData data)
    {
        if (data == null)
        {
            _saveFileNameTxt.text = "Save file name A";
            _saveFileLastPlayedTxt.text = "Last played: A";
        }
        else
        {
            _saveFileNameTxt.text = data.saveFileName;
            _saveFileLastPlayedTxt.text = "Last played: " + DateTime.FromBinary(data.lastUpdated).ToString();
        }
    }

    public string GetProfileId()
    {
        return this._profileId;
    }

    public void SetInteractable(bool interactable)
    {
        _saveFileButton.interactable = interactable;
    }
}
