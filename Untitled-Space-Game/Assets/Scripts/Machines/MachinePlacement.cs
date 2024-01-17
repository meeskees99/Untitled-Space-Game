using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] List<DiggingMachine> _placedDiggers = new();
    [SerializeField] List<SmeltingMachine> _placedSmelters = new();
    [SerializeField] List<int> diggerVeinIndex = new();
    [SerializeField] List<int> smelterIndex = new();
    [SerializeField] List<Vector3> smelterPositions = new();
    [SerializeField] List<Quaternion> smelterRotations = new();


    [SerializeField] Transform _shootPos;
    bool _hasLoadData;

    RaycastHit _hit;

    public GameObject selectedPrefab;

    int _selectedIndex = -1;

    GameObject _spawnedBlueprint;

    public bool _machineQuest;

    bool _canPlace;

    private void Start()
    {
        _selectedIndex = -1;

        if (_hasLoadData)
        {
            for (int i = 0; i < diggerVeinIndex.Count; i++)
            {
                SpawnMiner(diggerVeinIndex[i]);
            }
            SpawnSmelter();
        }
    }

    public void PickMachine(Item machineItem, RaycastHit raycastHit)
    {
        // Debug.Log("Picking machine Item " + machineItem.name);
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
        GameObject spawnedMachine = Instantiate(selectedPrefab, placePos.point, Quaternion.identity);
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


    void SpawnMiner(int spawnIndex)
    {
        GameObject spawnedMachine = Instantiate(_machinePrefabs[0], _resourceSpawner._resourceGameObjects[spawnIndex].transform);
        spawnedMachine.transform.localPosition = _placementOffset;
    }

    void SpawnSmelter()
    {
        for (int i = 0; i < smelterIndex.Count; i++)
        {
            GameObject spawnedSmelter = Instantiate(_machinePrefabs[1], smelterPositions[i], Quaternion.identity);
            spawnedSmelter.GetComponent<SmeltingMachine>().smelterIndex = smelterIndex[i];
            _placedSmelters.Add(spawnedSmelter.GetComponent<SmeltingMachine>());
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
    }

    public void SaveData(GameData data)
    {
        data.diggerVeinIndex = diggerVeinIndex;
        data.smelterIndex = smelterIndex;
        data.smelterPositions = smelterPositions;
    }
}
