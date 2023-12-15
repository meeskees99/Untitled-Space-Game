using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    NavMeshAgent _agent;
    [Header("Debugging")]
    [SerializeField] float _currentTravelTime;
    [SerializeField] float _maxTravelTime = 15f;

    [Header("Enemy Stats [DO NOT CHANGE(Recieves from Enemy Stats)]")]
    [SerializeField] string _enemyName;
    [SerializeField] int _health;
    [SerializeField] int _attackDamage;
    [SerializeField] float _attackRange;
    [SerializeField] float _attackRate;
    [SerializeField] float _chaseRadius;
    [SerializeField] float _chaseTime;

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

    GameObject player;

    EnemySpawner _enemySpawner;

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
    }

    float timer;
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
                    player.GetComponent<PlayerStats>().TakeDamage(_attackDamage);
                }
                else
                {
                    attackTimer -= Time.deltaTime;
                }
            }
            else
            {
                _agent.destination = player.transform.position;
                if (_agent.remainingDistance > 0.5f)
                    _currentTravelTime += Time.deltaTime;
            }
            return;
        }
        else
        {
            if (chaseTimer <= 0)
            {
                if (_agent.remainingDistance > 0.5f)
                    _currentTravelTime += Time.deltaTime;
                if (!_agent.pathPending && _agent.remainingDistance < 0.5f || _currentTravelTime > _maxTravelTime)
                {
                    timer -= Time.deltaTime;
                    if (timer <= 0)
                    {
                        GoToNextPoint();
                    }
                }
            }
            else
            {
                _agent.destination = player.transform.position;
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
        timer = Random.Range(_minWaitTime, _maxWaitTime);

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
        Collider[] colliders = Physics.OverlapSphere(transform.position, _enemyStats.chaseRadius);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].GetComponentInParent<CharStateMachine>())
            {
                if (PlayerStats.Health > 0)
                {
                    player = colliders[i].GetComponentInParent<CharStateMachine>().gameObject;
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
