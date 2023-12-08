using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class ResourceSpawner : MonoBehaviour, IDataPersistence
{
    [Header("Settings")]
    [SerializeField] GameObject[] _resourcePrefabs;
    [SerializeField] Mesh[] floorMesh;

    [Header("Save & Load")]
    [SerializeField] List<Quaternion> _resourceRotations = new();
    [SerializeField] List<Vector3> _resourcePositions = new();
    [SerializeField] List<int> _resourceIndex = new();

    [SerializeField] float _minResourceDistance;

    [SerializeField] int _amountToSpawn;

    [SerializeField] NavMeshHit _navMeshHit;

    [SerializeField] Vector3 _randomPos;
    [SerializeField] Quaternion _randomRot;
    [SerializeField] int _randomResourceIndex;

    bool _hasLoadData;

    private void Start()
    {
        for (int i = 0; i < _amountToSpawn; i++)
        {
            CheckSpawnResource();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            CheckSpawnResource();
            NavMeshManager.Instance.UpdateNavMesh();
        }
    }

    void CheckSpawnResource()
    {
        if (_hasLoadData)
        {
            for (int i = 0; i < _resourcePositions.Count; i++)
            {
                SpawnResource(_resourcePositions[i], _resourceRotations[i], _resourceIndex[i]);
                Debug.Log($"Spawned {_resourcePrefabs[i].name} at {_resourcePositions[i]} from save");
            }
            return;
        }
        else
        {

            Debug.Log("Get random pos");
            _randomPos = GetRandomPosition();



            Debug.Log("Get random rot");
            _randomRot = GetRandomRotation();



            Debug.Log("Get random slop");
            _randomResourceIndex = GetRandomResource();
            Debug.Log(_randomResourceIndex);

            if (NavMesh.SamplePosition(_randomPos, out _navMeshHit, 10f, NavMesh.AllAreas))
            {
                Debug.Log($"Prefab array length == {_resourcePrefabs.Length}");
                Debug.Log("Hit navmesh " + "with resource " + _resourcePrefabs[_randomResourceIndex]);
                SpawnResource(_navMeshHit.position, _randomRot, _randomResourceIndex);
            }
            else
            {
                if (NavMesh.FindClosestEdge(_navMeshHit.position, out _navMeshHit, NavMesh.AllAreas))
                {
                    SpawnResource(_navMeshHit.position, _randomRot, _randomResourceIndex);
                }
            }
        }
    }

    Vector3 GetRandomPosition()
    {
        float xPos = Random.Range(-100, 100);
        float yPos = 0;
        float zPos = Random.Range(-100, 100);

        Vector3 randomPos = new Vector3(xPos, yPos, zPos);

        for (int i = 0; i < _resourcePositions.Count; i++)
        {
            if (Vector3.Distance(randomPos, _resourcePositions[i]) < _minResourceDistance)
            {
                GetRandomPosition();
            }
        }

        return randomPos;
    }

    Quaternion GetRandomRotation()
    {
        Quaternion randomRot = Quaternion.identity;
        randomRot.y = Random.Range(0, 361);
        return randomRot;
    }

    int GetRandomResource()
    {
        int randomResourceIndex = Random.Range(0, _resourcePrefabs.Length);
        return randomResourceIndex;
    }

    void SpawnResource(Vector3 position, Quaternion rotation, int resourceIndex)
    {
        GameObject spawnedResource = Instantiate(_resourcePrefabs[resourceIndex], position, rotation);
        _resourceIndex.Add(resourceIndex);

        spawnedResource.transform.position = position;
        _resourcePositions.Add(position);

        spawnedResource.transform.rotation = rotation;
        _resourceRotations.Add(rotation);

        NavMeshManager.Instance.UpdateNavMesh();

        Debug.Log($"Spawned a {_resourcePrefabs[resourceIndex]} resource at {position}, with a rotation of {rotation}");

    }

    public void LoadData(GameData data)
    {
        _resourcePositions = data.resourcePositions;
        _resourceRotations = data.resourceRotations;
        _resourceIndex = data.resourceIndex;

        if (data.resourcePositions.Count == 0)
        {
            _hasLoadData = true;
        }
    }

    public void SaveData(GameData data)
    {
        data.resourcePositions = _resourcePositions;
        data.resourceRotations = _resourceRotations;
        data.resourceIndex = _resourceIndex;
    }
}