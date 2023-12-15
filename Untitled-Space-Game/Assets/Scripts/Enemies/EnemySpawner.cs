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

    [Header("Spawn Settings")]
    [SerializeField] float _minSpawnDistanceFromPlayer = 30f;
    [SerializeField] float _maxSpawnDistanceFromPlayer = 100f;
    [SerializeField] int preferedEnemyCount;
    [SerializeField] int _minSpawnAmountOnStart, _maxSpawnAmountOnStart;
    [SerializeField] GameObject[] _enemyTypes;

    int _spawnAttempts;
    // Start is called before the first frame update
    void Start()
    {
        // NavMeshManager.Instance.UpdateNavMesh();
        if (player == null)
            player = FindObjectOfType<PlayerStats>().gameObject;

        int randomSpawnAmount = Random.Range(_minSpawnAmountOnStart, _maxSpawnAmountOnStart);
        for (int i = 0; i < randomSpawnAmount; i++)
        {
            GetRandomPosition();
        }

        Debug.Log($"Spawned {enemiesInScene.Count} enemies on start!");
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
        float xpos = Random.Range(player.transform.position.x - _maxSpawnDistanceFromPlayer, _maxSpawnDistanceFromPlayer + player.transform.position.x);
        float ypos = 0;
        float zpos = Random.Range(player.transform.position.x - _maxSpawnDistanceFromPlayer, _maxSpawnDistanceFromPlayer + player.transform.position.x);

        Vector3 randomPos = new Vector3(xpos, ypos, zpos);

        if (NavMesh.SamplePosition(randomPos, out _navMeshHit, 10f, NavMesh.AllAreas) &&
        Vector3.Distance(randomPos, player.transform.position) > _minSpawnDistanceFromPlayer)
        {
            _spawnAttempts = 0;
            SpawnNewEnemy(_navMeshHit.position);
        }
        else
        {
            _spawnAttempts++;
            if (_spawnAttempts < 20)
                GetRandomPosition();
        }
    }

}
