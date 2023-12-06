using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [Header("Info")]
    public static int CurrentEnemyCount;
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
            bool result = SpawnNewEnemy();
            if (result)
            {
                Debug.Log("Succesfully Spawned An Enemy");
            }
            else
            {
                result = SpawnNewEnemy();
            }
        }

        Debug.Log($"Spawned {randomSpawnAmount} enemies on start!");
    }



    bool SpawnNewEnemy()
    {
        NavMesh.SamplePosition(transform.position, out _navMeshHit, Mathf.Infinity, _spawnLayer);
        if (Vector3.Distance(_navMeshHit.position, player.transform.position) < _minSpawnDistanceFromPlayer)
        {
            return false;
        }

        int i = Random.Range(0, _enemyTypes.Length);

        GameObject spawnedEnemy = Instantiate(_enemyTypes[i], _navMeshHit.position, Quaternion.identity);
        enemiesInScene.Add(spawnedEnemy);
        CurrentEnemyCount++;
        return true;
    }

    public void RemoveEnemy()
    {
        CurrentEnemyCount--;
    }
}
