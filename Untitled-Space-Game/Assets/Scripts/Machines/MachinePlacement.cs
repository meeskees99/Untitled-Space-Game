using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MachinePlacement : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] GameObject[] _machinePrefabs;
    [SerializeField] GameObject[] _machineBlueprintPrefabs;

    [Header("Settings")]
    [SerializeField] float _placementRange;
    [SerializeField] Vector3 _placementOffset;
    [SerializeField] LayerMask _placementMask;
    [SerializeField] Color _canPlaceColor;
    [SerializeField] Color _cantPlaceColor;

    [SerializeField] Transform _shootPos;

    RaycastHit _hit;

    GameObject _selectedPrefab;

    int _selectedIndex = -1;

    GameObject _spawnedBlueprint;

    private void Start()
    {
        _selectedIndex = -1;
    }
    void Update()
    {
        if (_selectedIndex >= 0)
        {
            if (Physics.Raycast(_shootPos.position, _shootPos.forward, out _hit, _placementRange))
            {
                if (_spawnedBlueprint == null)
                {
                    _spawnedBlueprint = Instantiate(_machineBlueprintPrefabs[_selectedIndex]);
                }
                if (_hit.transform.gameObject.layer == LayerMask.NameToLayer("Resource"))
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
                    _spawnedBlueprint.transform.position = _hit.point;
                    _spawnedBlueprint.transform.GetComponentInChildren<MeshRenderer>().material.color = _cantPlaceColor;
                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        Destroy(_spawnedBlueprint);
                    }
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
            return;
        }
        if (machineIndex < 0)
        {
            _selectedIndex = -1;
        }
        else
        {
            _selectedIndex = machineIndex;
        }
    }

    void PlaceMachine(Transform placePos)
    {
        GameObject spawnedMachine = Instantiate(_machinePrefabs[_selectedIndex], placePos);
        spawnedMachine.transform.position = _placementOffset;

        _selectedPrefab = null;
        _selectedIndex = -1;
    }
}
