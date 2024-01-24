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
    public string ProfileId
    {
        get { return _profileId; }
        set { _profileId = value; }
    }

    [Header("Content")]
    [SerializeField] Button _saveFileButton;
    public Button SaveFileButton
    {
        get { return _saveFileButton; }
    }
    [SerializeField] Button _deleteFileButton;
    public Button DeleteFileButton
    {
        get { return _deleteFileButton; }
    }
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
            _saveFileNameTxt.text = _profileId;
            _saveFileLastPlayedTxt.text = DateTime.FromBinary(data.lastUpdated).ToString();
        }
    }

    // public void SetInteractable(bool interactable)
    // {
    //     _saveFileButton.interactable = interactable;
    // }
}
