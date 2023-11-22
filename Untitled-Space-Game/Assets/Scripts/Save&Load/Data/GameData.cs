using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public Vector3 playerPosition;
    public Quaternion playerRotation;

    public int[] itemId = new int[23];
    public int[] itemAmount = new int[23];

    public GameData()
    {
        this.playerPosition = Vector3.zero;
        this.playerRotation = Quaternion.identity;

        for (int i = 0; i < itemId.Length; i++)
        {
            this.itemId[i] = -1;
            this.itemAmount[i] = -1;

        }
    }
}
