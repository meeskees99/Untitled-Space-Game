using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
    [SerializeField] List<int> diggerVeinIndex = new();

    [SerializeField] Transform _shootPos;
    bool _hasLoadData;

    RaycastHit _hit;

    GameObject _selectedPrefab;

    int _selectedIndex = -1;

    GameObject _spawnedBlueprint;

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
        if (_selectedPrefab != null)
        {
            if (Physics.Raycast(_shootPos.position, _shootPos.forward, out _hit, _placementRange))
            {
                if (_spawnedBlueprint == null)
                {
                    _spawnedBlueprint = Instantiate(_machineBlueprintPrefabs[_selectedIndex]);
                }
                if (_hit.transform.gameObject.layer == LayerMask.NameToLayer(_placementLayerName))
                {
                    // TODO - make the thing turn blue
                    _spawnedBlueprint.transform.position = _hit.transform.position;
                    _spawnedBlueprint.transform.GetComponentInChildren<MeshRenderer>().material.color = _canPlaceColor;
                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        Destroy(_spawnedBlueprint);
                        PlaceMachine(_hit.transform);
                    }
                }
                else
                {
                    // TODO - make the thing turn red
                    _spawnedBlueprint.transform.position = _hit.point + _placementOffset;
                    _spawnedBlueprint.transform.GetComponentInChildren<MeshRenderer>().material.color = _cantPlaceColor;

                    //UNCOMMENT IF YOU WANT TO STOP PLACING WHEN YOU MISS
                    // if (Input.GetKeyDown(KeyCode.Mouse0))
                    // {
                    //     _selectedPrefab = null;
                    //     _selectedIndex = -1;
                    //     Destroy(_spawnedBlueprint);
                    // }
                }
            }
            else
            {
                Destroy(_spawnedBlueprint);
            }

        }
        else
        {
            if (_spawnedBlueprint != null)
            {
                Destroy(_spawnedBlueprint);
            }
        }
    }

    public void PickMachine(int machineIndex)
    {
        if (_selectedIndex == machineIndex)
        {
            _selectedIndex = -1;
            _selectedPrefab = null;
            return;
        }
        if (machineIndex < 0)
        {
            _selectedIndex = -1;
            _selectedPrefab = null;
        }
        else
        {
            _selectedIndex = machineIndex;
            _selectedPrefab = _machinePrefabs[_selectedIndex];
        }
    }

    void PlaceMachine(Transform placePos)
    {
        GameObject spawnedMachine = Instantiate(_machinePrefabs[_selectedIndex], placePos);
        spawnedMachine.transform.localPosition = _placementOffset;

        _placedDiggers.Add(spawnedMachine.GetComponent<DiggingMachine>());

        diggerVeinIndex.Add(placePos.GetComponent<ResourceVein>().resourceIndex);

        _selectedPrefab = null;
        _selectedIndex = -1;

        QuestManager.Instance.CheckPlace(spawnedMachine);
    }

    void SpawnMachines(int spawnIndex)
    {
        GameObject spawnedMachine = Instantiate(_machinePrefabs[0], _resourceSpawner._resourceGameObjects[spawnIndex].transform);
        spawnedMachine.transform.localPosition = _placementOffset;
    }

    public void LoadData(GameData data)
    {
        diggerVeinIndex = data.diggerVeinIndex;

        if (diggerVeinIndex.Count > 0)
        {
            _hasLoadData = true;
        }
    }

    public void SaveData(GameData data)
    {
        data.diggerVeinIndex = diggerVeinIndex;
    }
}
