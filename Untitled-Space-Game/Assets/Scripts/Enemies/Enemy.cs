using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    NavMeshAgent _agent;
    [Header("Debugging")]
    [SerializeField] float _currentTravelTime;
    [SerializeField] float _maxTravelTime = 15f;

    [Header("UI")]
    [SerializeField] float _healthShowTime = 3f;
    float _currentShowTime;

    [SerializeField] float _canvasFollowRange = 7f;
    [SerializeField] GameObject _canvasObject;
    [SerializeField] Slider _healthSlider;

    [Header("Enemy Stats [DO NOT CHANGE(Recieves from Enemy Stats)]")]
    [SerializeField] string _enemyName;
    [SerializeField] float _health;
    [SerializeField] int _attackDamage;
    [SerializeField] float _attackRange;
    [SerializeField] float _attackRate;
    [SerializeField] float _chaseRadius;
    [SerializeField] float _chaseTime;

    public float Health { get { return _health; } private set { } }

    [Header("Movement Stats [DO NOT CHANGE(Recieves from Enemy Stats)]")]
    [SerializeField] float _movementSpeed;
    [SerializeField] float _stopDistance;
    [SerializeField] float _walkRadius;

    [Header("EnemyOptions")]
    [SerializeField] EnemyStats _enemyStats;
    [SerializeField] LayerMask _walkabeLayer;
    [Tooltip("Time Between Picking New Patrol Points")]
    [SerializeField] float _minWaitTime = 2f, _maxWaitTime = 6f;

    [Tooltip("Override Movement")]
    [SerializeField] bool _stopPatrolling;

    GameObject _player;

    EnemySpawner _enemySpawner;

    float spawnDelay = 3f;

    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        if (_enemyStats != null)
        {
            _enemyName = _enemyStats.enemyName;
            _health = _enemyStats.health;
            _attackDamage = _enemyStats.attackDamage;
            _attackRate = _enemyStats.attackRate;
            _chaseRadius = _enemyStats.chaseRadius;
            _chaseTime = _enemyStats.chaseTime;

            _movementSpeed = _enemyStats.movementSpeed;
            _stopDistance = _enemyStats.stopDistance;
            _walkRadius = _enemyStats.movementRadius;
            if (_agent != null)
            {
                _agent.angularSpeed = _enemyStats.angularSpeed;
                _agent.stoppingDistance = _stopDistance;
                _agent.autoBraking = false;
            }

            _healthSlider.maxValue = _enemyStats.health;

            if (!_enemyStats.canJump)
                _agent.areaMask = 1;
        }
        else
        {
            Debug.LogError("You did not select any stats for " + _enemyName);
        }

        _enemySpawner = FindObjectOfType<EnemySpawner>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_agent == null)
        {
            return;
        }
        else if (GetComponent<NavMeshAgent>().enabled == false)
        {
            return;
        }
        PatrolToNextPoint();

        NavMeshHit hit;
        if (NavMesh.FindClosestEdge(transform.position, out hit, NavMesh.AllAreas))
        {
            if (Vector3.Distance(transform.position, hit.position) < 1.5f && _agent.remainingDistance < 2f)
            {
                GoToNextPoint();
            }
            DrawCircle(transform.position, hit.distance, Color.red);
            Debug.DrawRay(hit.position, Vector3.up, Color.red);
        }

        if (_player != null)
        {
            Debug.Log("_currentShowTime: " + _currentShowTime);
            if (_currentShowTime < _healthShowTime)
            {
                _canvasObject.GetComponent<Canvas>().enabled = true;

                if (Vector3.Distance(_player.transform.position, transform.position) > _canvasFollowRange)
                {
                    _canvasObject.GetComponent<Canvas>().enabled = false;
                }
                else
                {
                    _canvasObject.GetComponent<Canvas>().enabled = true;
                    _canvasObject.transform.LookAt(_player.transform);
                }

            }
            else
            {
                _canvasObject.GetComponent<Canvas>().enabled = false;
            }
        }
        else
        {
            _player = FindObjectOfType<PlayerStats>().gameObject;
        }
    }

    #region Pathfinding

    float waitTimer;
    float chaseTimer;
    float attackTimer;

    void PatrolToNextPoint()
    {
        if (_stopPatrolling)
        {
            _currentTravelTime = 0;
            return;
        }

        _agent.speed = _movementSpeed;

        if (CheckPlayerInRange() != null)
        {
            if (chaseTimer <= 0)
                chaseTimer = _chaseTime;

            if (_agent.remainingDistance < _attackRange)
            {
                _currentTravelTime = 0;
                if (attackTimer <= 0)
                {
                    attackTimer = _attackRate;
                    _player.GetComponent<PlayerStats>().TakeDamage(_attackDamage);
                }
                else
                {
                    attackTimer -= Time.deltaTime;
                }
            }
            else
            {
                _agent.destination = _player.transform.position;

                // if (_agent.remainingDistance > 0.5f)
                // {
                //     _currentTravelTime += Time.deltaTime;
                // }
                // if (_currentTravelTime > _maxTravelTime)
                // {
                //     GoToNextPoint();
                // }
            }
            return;
        }
        else
        {
            if (spawnDelay > 0)
            {
                spawnDelay -= Time.deltaTime;
                return;
            }
            if (chaseTimer <= 0)
            {
                if (_agent.remainingDistance > 0.5f)
                {
                    _currentTravelTime += Time.deltaTime;
                }
                if (_currentTravelTime > _maxTravelTime)
                {
                    GoToNextPoint();
                }
                if (!_agent.pathPending && _agent.remainingDistance < 0.5f)
                {
                    waitTimer -= Time.deltaTime;
                    if (waitTimer <= 0)
                    {
                        GoToNextPoint();
                    }
                }
            }
            else
            {
                _agent.destination = _player.transform.position;
                chaseTimer -= Time.deltaTime;
            }
        }
    }

    void DrawCircle(Vector3 center, float radius, Color color)
    {
        Vector3 prevPos = center + new Vector3(radius, 0, 0);
        for (int i = 0; i < 30; i++)
        {
            float angle = (float)(i + 1) / 30.0f * Mathf.PI * 2.0f;
            Vector3 newPos = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            Debug.DrawLine(prevPos, newPos, color);
            prevPos = newPos;
        }
    }

    void GoToNextPoint()
    {
        waitTimer = Random.Range(_minWaitTime, _maxWaitTime);

        Vector3 randomDirection = Random.insideUnitSphere * _walkRadius;

        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, _walkRadius, 1);
        Vector3 finalPosition = hit.position;

        _agent.destination = finalPosition;
        _currentTravelTime = 0;
    }


    public Collider CheckPlayerInRange()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _chaseRadius);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].GetComponentInParent<CharStateMachine>())
            {
                if (PlayerStats.Health > 0)
                {
                    _player = colliders[i].GetComponentInParent<CharStateMachine>().gameObject;
                    return colliders[i];
                }
                else
                {
                    Debug.Log("Found Dead Player");
                }
            }
        }
        return null;
    }
    #endregion

    public void TakeDamage(float amount)
    {
        if (amount >= _health)
        {
            Die();
        }
        else
        {
            Debug.LogError("Damage Taken: " + amount);
            _health -= amount;
            _healthSlider.value = _health;
            _currentShowTime = 0;
        }
    }

    void Die()
    {
        DropLoot();

        Destroy(gameObject);
    }

    void DropLoot()
    {
        Debug.Log("Dropped Loot");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _walkRadius);
    }

    private void OnDestroy()
    {
        // Debug.Log($"{_enemyName} was destroyed");
        _enemySpawner.RemoveEnemy();
        _enemySpawner.enemiesInScene.Remove(gameObject);
    }
}
