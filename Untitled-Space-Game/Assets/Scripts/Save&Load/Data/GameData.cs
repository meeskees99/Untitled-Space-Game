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
    public List<int> diggerFuel = new();
    public List<int> diggerOutput = new();

    public List<int> smelterIndex = new();
    public List<Vector3> smelterPositions = new();
    public List<Quaternion> smelterRotations = new();

    public List<int> smelterFuelId = new();
    public List<int> smelterResourceId = new();
    public List<int> smelterOutputId = new();

    public List<int> smelterFuelAmount = new();
    public List<int> smelterResourceAmount = new();
    public List<int> smelterOutputAmount = new();

    public List<float> smelterFuelLeft = new();
    public List<float> smelterProgressAmount = new();

    // public int[] resourceId;
    // public List<int> itemAmounts = new();

    // public int[] fuelId;
    // public List<int> fuelAmounts = new();
    #endregion

    public int currentQuestId;

    public int gameDifficulty;



    public GameData()
    {
        this.gameDifficulty = -1;
        this.currentQuestId = 0;

        for (int i = 0; i < itemId.Length; i++)
        {
            this.itemId[i] = -1;
            this.itemAmount[i] = -1;
        }

        this.resourceIndex = new List<int>();

        this.resourcePositions = new List<Vector3>();
        this.resourceRotations = new List<Vector3>();

        this.diggerVeinIndex = new();

        this.diggerFuel = new();
        this.diggerOutput = new();

        this.smelterIndex = new();

        this.smelterPositions = new();
        this.smelterRotations = new();

        this.smelterFuelId = new();
        this.smelterResourceId = new();
        this.smelterOutputId = new();

        this.smelterFuelAmount = new();
        this.smelterResourceAmount = new();
        this.smelterOutputAmount = new();

        this.smelterFuelLeft = new();
        this.smelterProgressAmount = new();
    }
}
