using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCubeData : MonoBehaviour, IDataPersistence
{

    public void LoadData(GameData data)
    {
        this.transform.position = data.cubePosition;
    }

    public void SaveData(ref GameData data)
    {
        data.cubePosition = this.transform.position;
    }
}
