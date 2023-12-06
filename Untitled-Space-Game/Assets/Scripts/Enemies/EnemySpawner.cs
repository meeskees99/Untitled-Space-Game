using System.Collections;
using System.Collections.Generic;
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
            // bool result = 

            SpawnNewEnemy();

            // if (result)
            // {
            //     Debug.Log("Succesfully Spawned An Enemy");
            // }
            // else
            // {
            //     result = SpawnNewEnemy();
            // }
        }

        Debug.Log($"Spawned {randomSpawnAmount} enemies on start!");
    }



    void SpawnNewEnemy()
    {
        bool canSpawn = false;
        while (!canSpawn)
        {
            if (NavMesh.SamplePosition(player.transform.position, out _navMeshHit, 200f, NavMesh.AllAreas))
            {
                if (Vector3.Distance(_navMeshHit.position, player.transform.position) < _minSpawnDistanceFromPlayer)
                {
                    print("Couldn't spawn enemy here as it was too close to the player");
                    return;
                }
                else
                {
                    canSpawn = true;
                }
            }
        }

        int i = Random.Range(0, _enemyTypes.Length);
        GameObject enemyToSpawn = _enemyTypes[i];
        GameObject spawnedEnemy = Instantiate(enemyToSpawn);
        Vector3 spawnPos = _navMeshHit.position;
        Debug.Log($"Spawnpos: {spawnPos}");
        spawnedEnemy.transform.position = spawnPos;
        enemiesInScene.Add(spawnedEnemy);
        // spawnedEnemy.GetComponent<NavMeshAgent>().enabled = true;
        currentEnemyCount++;
    }

    public void RemoveEnemy()
    {
        currentEnemyCount--;
    }
}
