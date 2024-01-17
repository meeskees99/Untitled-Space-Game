using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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
    [SerializeField] float _placementRange;
    [SerializeField] Vector3 _placementOffset;
    [SerializeField] string _placementLayerName;
    [SerializeField] Color _canPlaceColor;
    [SerializeField] Color _cantPlaceColor;

    [SerializeField] List<DiggingMachine> _placedDiggers = new();
    [SerializeField] List<SmeltingMachine> _placedSmelters = new();
    [SerializeField] List<int> diggerVeinIndex = new();
    [SerializeField] List<int> smelterIndex = new();

    [SerializeField] Transform _shootPos;
    bool _hasLoadData;

    RaycastHit _hit;

    GameObject _selectedPrefab;

    int _selectedIndex = -1;

    GameObject _spawnedBlueprint;

    public bool _machineQuest;

    private void Start()
    {
        _selectedIndex = -1;

        if (_hasLoadData)
        {
            for (int i = 0; i < diggerVeinIndex.Count; i++)
            {
                SpawnMachines(diggerVeinIndex[i]);
            }
        }
    }
    void Update()
    {
        // if (_selectedPrefab != null)
        // {
        //     if (Physics.Raycast(_shootPos.position, _shootPos.forward, out _hit, _placementRange))
        //     {
        //         if (_spawnedBlueprint == null)
        //         {
        //             _spawnedBlueprint = Instantiate(_machineBlueprintPrefabs[_selectedIndex]);
        //         }
        //         if (_hit.transform.gameObject.layer == LayerMask.NameToLayer(_placementLayerName))
        //         {
        //             // TODO - make the thing turn blue
        //             _spawnedBlueprint.transform.position = _hit.transform.position;
        //             _spawnedBlueprint.transform.GetComponentInChildren<MeshRenderer>().material.color = _canPlaceColor;
        //             if (Input.GetKeyDown(KeyCode.Mouse0))
        //             {
        //                 Destroy(_spawnedBlueprint);
        //                 PlaceMachine(_hit.transform);
        //             }
        //         }
        //         else
        //         {
        //             // TODO - make the thing turn red
        //             _spawnedBlueprint.transform.position = _hit.point + _placementOffset;
        //             _spawnedBlueprint.transform.GetComponentInChildren<MeshRenderer>().material.color = _cantPlaceColor;

        //             //UNCOMMENT IF YOU WANT TO STOP PLACING WHEN YOU MISS
        //             // if (Input.GetKeyDown(KeyCode.Mouse0))
        //             // {
        //             //     _selectedPrefab = null;
        //             //     _selectedIndex = -1;
        //             //     Destroy(_spawnedBlueprint);
        //             // }
        //         }
        //     }
        //     else
        //     {
        //         Destroy(_spawnedBlueprint);
        //     }

        // }
        // else
        // {
        //     if (_spawnedBlueprint != null)
        //     {
        //         Destroy(_spawnedBlueprint);
        //     }
        // }
    }

    public void PickMachine(Item machineItem, RaycastHit raycastHit)
    {
        if (machineItem == null)
        {
            _selectedPrefab = null;
            if (_spawnedBlueprint != null)
            {
                Debug.Log("Destroyed Blueprint");
                Destroy(_spawnedBlueprint);
            }
            return;
        }
        else
        {
            if (_spawnedBlueprint == null && _selectedPrefab != null)
            {
                _spawnedBlueprint = Instantiate(machineItem.machineBlueprint, raycastHit.point, Quaternion.identity);
            }
            else if (_spawnedBlueprint != null && _selectedPrefab != null)
            {
                _spawnedBlueprint.transform.position = raycastHit.point + _placementOffset;
                // _spawnedBlueprint.transform.rotation = raycastHit.transform.rotation;
                if (LayerMask.LayerToName(raycastHit.transform.gameObject.layer) == "Ground")
                {
                    _spawnedBlueprint.transform.GetComponentInChildren<MeshRenderer>().material.color = _canPlaceColor;
                }
                else
                {
                    _spawnedBlueprint.transform.GetComponentInChildren<MeshRenderer>().material.color = _cantPlaceColor;
                }

            }
        }
        if (machineItem.isPlacable)
        {
            _selectedPrefab = machineItem.machinePrefab;
        }


    }

    public void PlaceMachine(RaycastHit placePos)
    {
        Debug.Log("place");
        if (_selectedPrefab == null)
        {
            return;
        }

        if (_selectedPrefab.transform.GetComponent<SmeltingMachine>())
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
        GameObject spawnedMachine = Instantiate(_selectedPrefab, placePos);
        spawnedMachine.transform.localPosition = _placementOffset;

        _placedDiggers.Add(spawnedMachine.GetComponent<DiggingMachine>());

        diggerVeinIndex.Add(placePos.GetComponent<ResourceVein>().resourceIndex);

        _selectedPrefab = null;

        if (_machineQuest)
        {
            QuestManager.Instance.CheckPlace();
        }
    }

    void PlaceSmelter(RaycastHit placePos)
    {
        Destroy(_spawnedBlueprint);
        GameObject spawnedMachine = Instantiate(_selectedPrefab, placePos.point, Quaternion.identity);
        // spawnedMachine.transform.localPosition = _placementOffset;
        InventoryManager.Instance.UseItem(InventoryManager.Instance.GetSelectedItem().itemID, 1);

        _placedSmelters.Add(spawnedMachine.GetComponent<SmeltingMachine>());

        smelterIndex.Add(_placedSmelters.Count);
        spawnedMachine.GetComponent<SmeltingMachine>().smelterIndex = _placedSmelters.Count;

        _selectedPrefab = null;

        if (_machineQuest)
        {
            Debug.Log(QuestManager.Instance.gameObject.name);
            QuestManager.Instance.CheckPlace();
        }
    }


    void SpawnMachines(int spawnIndex)
    {
        GameObject spawnedMachine = Instantiate(_machinePrefabs[0], _resourceSpawner._resourceGameObjects[spawnIndex].transform);
        spawnedMachine.transform.localPosition = _placementOffset;
    }

    public void LoadData(GameData data)
    {
        diggerVeinIndex = data.diggerVeinIndex;
        smelterIndex = data.smelterIndex;

        if (diggerVeinIndex.Count > 0 || smelterIndex.Count > 0)
        {
            _hasLoadData = true;
        }
    }

    public void SaveData(GameData data)
    {
        data.diggerVeinIndex = diggerVeinIndex;
        data.smelterIndex = smelterIndex;
    }
}
