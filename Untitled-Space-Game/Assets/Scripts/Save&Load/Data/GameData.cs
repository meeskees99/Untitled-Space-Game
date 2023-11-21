using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public Vector3 cubePosition;
    public Quaternion cubeRotation;
    public Vector3 cubeScale;

    public GameData()
    {
        this.cubePosition = Vector3.zero;
        this.cubeRotation = Quaternion.identity;
        this.cubeScale = new Vector3(1, 1, 1);
    }
}
