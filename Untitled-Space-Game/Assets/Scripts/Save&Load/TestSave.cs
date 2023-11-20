using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class TestSave : MonoBehaviour
{
    [SerializeField] TMP_Text _saveTxt;

    [SerializeField] TMP_InputField _testSaveInputField;

    [SerializeField] GameObject _testCube;

    [SerializeField] float _testFloatX;
    [SerializeField] float _testFloatY;
    [SerializeField] float _testFloatZ;


    private void OnEnable()
    {
        Load();
    }

    public void SaveBtn()
    {
        string path = Path.Combine(Application.persistentDataPath, "test.map");
        using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
        {
            writer.Write((string)_testCube.transform.position.x.ToString());
            writer.Write((string)_testCube.transform.position.y.ToString());
            writer.Write((string)_testCube.transform.position.z.ToString());
        }
    }

    public void Load()
    {
        string path = Path.Combine(Application.persistentDataPath, "test.map");
        using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
        {
            float.TryParse(reader.ReadString(), out _testFloatX);
            float.TryParse(reader.ReadString(), out _testFloatY);
            float.TryParse(reader.ReadString(), out _testFloatZ);

            _testCube.transform.position = new Vector3(_testFloatX, _testFloatY, _testFloatZ);
        }
    }
}
// _testCube.transform.position = new Vector3(reader.ReadInt32(), _testCube.transform.position.y, _testCube.transform.position.z);