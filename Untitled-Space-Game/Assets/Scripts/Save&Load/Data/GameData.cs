using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public long lastUpdated;

    public List<Vector3> resourcePositions;
    public List<Quaternion> resourceRotations;
    public List<int> resourceIndex;

    public int[] itemId = new int[23];
    public int[] itemAmount = new int[23];

    public int gameDifficulty;

    public GameData()
    {
        this.gameDifficulty = -1;

        for (int i = 0; i < itemId.Length; i++)
        {
            this.itemId[i] = -1;
            this.itemAmount[i] = -1;
        }

        this.resourceIndex = new List<int>();
        this.resourcePositions = new List<Vector3>();
        this.resourceRotations = new List<Quaternion>();
    }
}
