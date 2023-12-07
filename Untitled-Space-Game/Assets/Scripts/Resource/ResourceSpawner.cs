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

    private void Start()
    {
        for (int i = 0; i < _amountToSpawn; i++)
        {
            CheckSpawnResource();
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.X))
        {
            NavMeshManager.Instance.UpdateNavMesh();
        }
        // floorMesh.
    }

    void CheckSpawnResource()
    {
        Vector3 randomPos = Vector3.zero;
        Quaternion randomRot = Quaternion.identity;
        GameObject randomResource = null;


        if (_resourcePositions.Count <= 0)
        {
            randomPos = GetRandomPosition();
        }
        if (_resourceRotations.Count <= 0)
        {
            randomRot = GetRandomRotation();
        }
        if (_resourceIndex.Count <= 0)
        {
            randomResource = GetRandomResource();
        }



        if (NavMesh.SamplePosition(randomPos, out NavMeshHit navMeshHit, 10f, NavMesh.AllAreas))
        {
            SpawnResource(navMeshHit.position, randomRot, randomResource);
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
        Quaternion kaas = Quaternion.identity;
        kaas.y = Random.Range(0, 361);

        return kaas;
    }

    GameObject GetRandomResource()
    {
        int i = Random.Range(0, _resourcePrefabs.Length);

        _resourceIndex.Add(i);

        return _resourcePrefabs[i];
    }

    void SpawnResource(Vector3 position, Quaternion rotation, GameObject resourcePrefabToSpawn)
    {
        Instantiate(resourcePrefabToSpawn, position, rotation);
        _resourcePositions.Add(position);
        _resourceRotations.Add(rotation);
    }

    public void LoadData(GameData data)
    {
        _resourcePositions = data.resourcePositions;
        _resourceRotations = data.resourceRotations;
        _resourceIndex = data.resourceIndex;
    }

    public void SaveData(GameData data)
    {
        data.resourcePositions = _resourcePositions;
        data.resourceRotations = _resourceRotations;
        data.resourceIndex = _resourceIndex;
    }
}