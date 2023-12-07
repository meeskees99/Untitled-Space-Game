using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [Header("Info")]
    public int currentEnemyCount;
    public List<GameObject> enemiesInScene = new();
    [SerializeField] GameObject player;

    [Header("NavMesh")]
    NavMeshHit _navMeshHit;
    [SerializeField] LayerMask _spawnLayer;

    [Header("Enemy")]
    [SerializeField] float _minSpawnDistanceFromPlayer = 30f;
    [SerializeField] int preferedEnemyCount;
    [SerializeField] int _minSpawnAmountOnStart, _maxSpawnAmountOnStart;
    [SerializeField] GameObject[] _enemyTypes;


    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerStats>().gameObject;

        int randomSpawnAmount = Random.Range(_minSpawnAmountOnStart, _maxSpawnAmountOnStart);
        for (int i = 0; i < randomSpawnAmount; i++)
        {
            GetRandomPosition();
        }

        Debug.Log($"Spawned {randomSpawnAmount} enemies on start!");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            GetRandomPosition();
        }
    }

    void SpawnNewEnemy(Vector3 spawnPosition)
    {
        int i = Random.Range(0, _enemyTypes.Length);
        GameObject enemyToSpawn = _enemyTypes[i];
        GameObject spawnedEnemy = Instantiate(enemyToSpawn);
        Debug.Log($"Spawnpos: {spawnPosition}");
        spawnedEnemy.transform.GetComponent<Rigidbody>().position = spawnPosition;
        enemiesInScene.Add(spawnedEnemy);
        spawnedEnemy.GetComponent<NavMeshAgent>().enabled = true;
        currentEnemyCount++;
    }

    public void RemoveEnemy()
    {
        currentEnemyCount--;
    }

    void GetRandomPosition()
    {
        NavMeshTriangulation navMeshTriangulation = NavMesh.CalculateTriangulation();

        int vertexIndex = Random.Range(0, navMeshTriangulation.vertices.Length);

        if (NavMesh.SamplePosition(navMeshTriangulation.vertices[vertexIndex], out _navMeshHit, 2f, NavMesh.AllAreas) &&
        Vector3.Distance(navMeshTriangulation.vertices[vertexIndex], player.transform.position) > _minSpawnDistanceFromPlayer)
        {
            SpawnNewEnemy(_navMeshHit.position);
        }
        else
        {
            Debug.Log("Navmesh missed");
            GetRandomPosition();
        }
    }
}
