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
                if (_hit.transform.gameObject.layer == _placementMask)
                {
                    // TODO - make the thing turn blue
                    _spawnedBlueprint.transform.position = _hit.transform.position;
                    _spawnedBlueprint.transform.GetComponentInChildren<Material>().color = _cantPlaceColor;
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
                    _spawnedBlueprint.transform.GetComponentInChildren<Material>().color = _canPlaceColor;
                }
            }
            else
            {
                Destroy(_spawnedBlueprint);
            }

        }
    }

    public void PickMachine(int machineIndex)
    {
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
        Instantiate(_machinePrefabs[_selectedIndex], placePos);

        _selectedPrefab = null;
        _selectedIndex = -1;
    }
}
