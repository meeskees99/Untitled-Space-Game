using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MachinePlacement : MonoBehaviour, IDataPersistence
{
    [SerializeField] ResourceSpawner _resourceSpawner;
    [Header("Prefabs")]
    [SerializeField] GameObject[] _machinePrefabs;
    [SerializeField] GameObject[] _machineBlueprintPrefabs;

    [Header("Settings")]
    [SerializeField] Transform _player;
    [SerializeField] float _placementRange;
    [SerializeField] Vector3 _placementOffset;
    [SerializeField] string _placementLayerName;
    [SerializeField] Color _canPlaceColor;
    [SerializeField] Color _cantPlaceColor;
    [Header("Save & Loading")]
    [SerializeField] List<DiggingMachine> _placedDiggers = new();
    [SerializeField] List<SmeltingMachine> _placedSmelters = new();

    [SerializeField] List<int> diggerVeinIndex = new();
    [SerializeField] List<int> smelterIndex = new();

    [SerializeField] List<Vector3> smelterPositions = new();
    [SerializeField] List<Quaternion> smelterRotations = new();

    [SerializeField] List<int> smelterFuelId = new();
    [SerializeField] List<int> smelterResourceId = new();
    [SerializeField] List<int> smelterOutputId = new();

    [SerializeField] List<int> smelterFuelAmount = new();
    [SerializeField] List<int> smelterResourceAmount = new();
    [SerializeField] List<int> smelterOutputAmount = new();

    [SerializeField] List<float> smelterFuelLeft = new();
    [SerializeField] List<float> smelterProgressAmount = new();

    [SerializeField] Transform _shootPos;
    bool _hasLoadData;


    public GameObject selectedPrefab;

    GameObject _spawnedBlueprint;

    public bool _machineQuest;

    bool _canPlace;

    private void Start()
    {
        if (_hasLoadData)
        {
            SpawnMiner();
            SpawnSmelter();
        }
    }

    public void PickMachine(Item machineItem, RaycastHit raycastHit)
    {
        if (machineItem == null)
        {
            selectedPrefab = null;
            if (_spawnedBlueprint != null)
            {
                Debug.Log("Destroyed Blueprint");
                Destroy(_spawnedBlueprint);
            }
            return;
        }
        else
        {
            if (_spawnedBlueprint == null && selectedPrefab != null)
            {
                _spawnedBlueprint = Instantiate(machineItem.machineBlueprint, raycastHit.point, Quaternion.identity);
            }
            else if (_spawnedBlueprint != null && selectedPrefab != null)
            {
                _spawnedBlueprint.transform.position = raycastHit.point + _placementOffset;

                _spawnedBlueprint.transform.rotation = _player.transform.rotation;

                if (LayerMask.LayerToName(raycastHit.transform.gameObject.layer) == "Ground")
                {
                    _canPlace = true;
                    _spawnedBlueprint.transform.GetComponentInChildren<MeshRenderer>().material.color = _canPlaceColor;
                }
                else
                {
                    _canPlace = false;
                    _spawnedBlueprint.transform.GetComponentInChildren<MeshRenderer>().material.color = _cantPlaceColor;
                }

            }
        }
        if (machineItem.isPlacable)
        {
            selectedPrefab = machineItem.machinePrefab;
        }


    }

    public void PlaceMachine(RaycastHit placePos)
    {
        Debug.Log("place");
        if (!_canPlace)
        {
            return;
        }
        if (selectedPrefab == null)
        {
            return;
        }

        if (selectedPrefab.transform.GetComponent<SmeltingMachine>())
        {
            PlaceSmelter(placePos);
        }
        else
        {
            PlaceMiner(placePos.transform);
        }
    }

    public void PlaceMiner(Transform placePos)
    {
        Destroy(_spawnedBlueprint);
        GameObject spawnedMachine = Instantiate(selectedPrefab, placePos);
        spawnedMachine.transform.localPosition = _placementOffset;

        // spawnedMachine.transform.rotation = Quaternion.Euler(0, , 0);

        _placedDiggers.Add(spawnedMachine.GetComponent<DiggingMachine>());

        diggerVeinIndex.Add(placePos.GetComponent<ResourceVein>().resourceIndex);

        selectedPrefab = null;

        if (_machineQuest)
        {
            QuestManager.Instance.CheckPlace();
        }
    }

    void PlaceSmelter(RaycastHit placePos)
    {
        Destroy(_spawnedBlueprint);
        GameObject spawnedMachine = Instantiate(selectedPrefab, placePos.point, _player.rotation);
        // spawnedMachine.transform.localPosition = _placementOffset;
        InventoryManager.Instance.UseItem(InventoryManager.Instance.GetSelectedItem().itemID, 1);

        _placedSmelters.Add(spawnedMachine.GetComponent<SmeltingMachine>());

        smelterIndex.Add(_placedSmelters.Count - 1);

        smelterPositions.Add(spawnedMachine.transform.position);
        smelterRotations.Add(spawnedMachine.transform.rotation);

        spawnedMachine.GetComponent<SmeltingMachine>().smelterIndex = _placedSmelters.Count - 1;

        selectedPrefab = null;

        if (_machineQuest)
        {
            Debug.Log(QuestManager.Instance.gameObject.name);
            QuestManager.Instance.CheckPlace();
        }
    }


    void SpawnMiner()
    {
        for (int i = 0; i < diggerVeinIndex.Count; i++)
        {
            GameObject spawnedMachine = Instantiate(_machinePrefabs[0], _resourceSpawner._resourceGameObjects[i].transform);
            spawnedMachine.transform.localPosition = _placementOffset;
            _placedDiggers.Add(spawnedMachine.GetComponent<DiggingMachine>());
        }
    }

    void SpawnSmelter()
    {
        bool hasFuel = false;
        bool hasResource = false;
        bool hasOutput = false;

        for (int i = 0; i < smelterIndex.Count; i++)
        {
            Debug.Log($"Count == {smelterIndex.Count}  i == {i}");
            GameObject spawnedSmelter = Instantiate(_machinePrefabs[1], smelterPositions[i], Quaternion.identity);
            spawnedSmelter.GetComponent<SmeltingMachine>().smelterIndex = smelterIndex[i];
            _placedSmelters.Add(spawnedSmelter.GetComponent<SmeltingMachine>());

            if (smelterFuelId[i] == -1)
            {
                Debug.Log($"No Fuel Found In Smelter {_placedSmelters[i].smelterIndex}");
            }
            else
            {
                hasFuel = true;
                spawnedSmelter.GetComponent<SmeltingMachine>().FuelAmount = smelterFuelAmount[i];
            }
            if (smelterResourceId[i] == -1)
            {
                Debug.Log($"No Resource Found In Smelter {_placedSmelters[i].smelterIndex}");
            }
            else
            {
                hasResource = true;
                spawnedSmelter.GetComponent<SmeltingMachine>().ResourceAmount = smelterResourceAmount[i];
            }
            if (smelterOutputId[i] == -1)
            {
                Debug.Log($"No Output Found In Smelter {_placedSmelters[i].smelterIndex}");
            }
            else
            {
                hasOutput = true;
                spawnedSmelter.GetComponent<SmeltingMachine>().OutputAmount = smelterOutputAmount[i];
            }

            for (int y = 0; y < InventoryManager.Instance.allItems.Length; y++)
            {
                if (hasFuel && InventoryManager.Instance.allItems[y].itemID == smelterFuelId[i])
                {
                    spawnedSmelter.GetComponent<SmeltingMachine>().FuelType = InventoryManager.Instance.allItems[y];
                    Debug.Log("Set Fuel Type As " + InventoryManager.Instance.allItems[y].name);
                }
                if (hasResource && InventoryManager.Instance.allItems[y].itemID == smelterResourceId[i])
                {
                    spawnedSmelter.GetComponent<SmeltingMachine>().ResourceType = InventoryManager.Instance.allItems[y];
                    Debug.Log("Set Resource Type As " + InventoryManager.Instance.allItems[y].name);
                }
                if (hasOutput && InventoryManager.Instance.allItems[y].itemID == smelterOutputId[i])
                {
                    spawnedSmelter.GetComponent<SmeltingMachine>().OutputType = InventoryManager.Instance.allItems[y];
                    Debug.Log("Set Output Type As " + InventoryManager.Instance.allItems[y].name);
                }
            }

            _placedSmelters[i].FuelLeft = smelterFuelLeft[i];
            _placedSmelters[i].SmeltProgression = smelterProgressAmount[i];

        }
    }

    public void LoadData(GameData data)
    {
        diggerVeinIndex = data.diggerVeinIndex;
        smelterIndex = data.smelterIndex;
        smelterPositions = data.smelterPositions;
        smelterRotations = data.smelterRotations;

        if (diggerVeinIndex.Count > 0 || smelterIndex.Count > 0)
        {
            _hasLoadData = true;
        }
        smelterFuelId = data.smelterFuelId;
        smelterFuelAmount = data.smelterFuelAmount;

        smelterOutputId = data.smelterOutputId;
        smelterOutputAmount = data.smelterOutputAmount;

        smelterResourceId = data.smelterResourceId;
        smelterResourceAmount = data.smelterResourceAmount;

        smelterFuelLeft = data.smelterFuelLeft;
        smelterProgressAmount = data.smelterProgressAmount;
    }

    public void SaveData(GameData data)
    {
        data.diggerVeinIndex = diggerVeinIndex;
        data.smelterIndex = smelterIndex;
        data.smelterPositions = smelterPositions;

        smelterFuelId.Clear();
        smelterFuelAmount.Clear();
        smelterResourceId.Clear();
        smelterResourceAmount.Clear();
        smelterOutputId.Clear();
        smelterOutputAmount.Clear();

        for (int i = 0; i < _placedSmelters.Count; i++)
        {
            if (_placedSmelters[i].FuelType != null)
            {
                smelterFuelId.Add(_placedSmelters[i].FuelType.itemID);
                smelterFuelAmount.Add(_placedSmelters[i].FuelAmount);
            }
            else
            {
                smelterFuelId.Add(-1);
                smelterFuelAmount.Add(0);
            }
            if (_placedSmelters[i].ResourceType != null)
            {
                smelterResourceId.Add(_placedSmelters[i].ResourceType.itemID);
                smelterResourceAmount.Add(_placedSmelters[i].ResourceAmount);
            }
            else
            {
                smelterResourceId.Add(-1);
                smelterResourceAmount.Add(0);
            }
            if (_placedSmelters[i].OutputType != null)
            {
                smelterOutputId.Add(_placedSmelters[i].OutputType.itemID);
                smelterOutputAmount.Add(_placedSmelters[i].OutputAmount);
            }
            else
            {
                smelterOutputId.Add(-1);
                smelterOutputAmount.Add(0);
            }
            smelterFuelLeft.Add(_placedSmelters[i].FuelLeft);
            smelterProgressAmount.Add(_placedSmelters[i].SmeltProgression);

        }

        data.smelterFuelId = smelterFuelId;
        data.smelterFuelAmount = smelterFuelAmount;

        data.smelterResourceId = smelterResourceId;
        data.smelterResourceAmount = smelterResourceAmount;

        data.smelterOutputId = smelterOutputId;
        data.smelterOutputAmount = smelterOutputAmount;

        data.smelterFuelLeft = smelterFuelLeft;
        data.smelterProgressAmount = smelterProgressAmount;
    }
}
