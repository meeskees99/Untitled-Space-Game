using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public long lastUpdated;

    #region Resources
    public List<Vector3> resourcePositions;
    public List<Vector3> resourceRotations;
    public List<int> resourceIndex;
    #endregion

    #region Inventory
    public int[] itemId = new int[23];
    public int[] itemAmount = new int[23];
    #endregion

    #region Machines
    public List<int> diggerVeinIndex = new();
    // public int[] resourceId;
    // public List<int> itemAmounts = new();


    // public int[] fuelId;
    // public List<int> fuelAmounts = new();
    #endregion
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
        this.resourceRotations = new List<Vector3>();

        this.diggerVeinIndex = new();
    }
}
