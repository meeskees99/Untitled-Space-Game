using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    NavMeshAgent _agent;

    [Header("Movement Options")]
    [SerializeField] LayerMask _walkabeLayer;
    float _movementSpeed = 5f;
    float _stopDistance = 1f;
    float _walkRadius;
    [SerializeField] float _minWaitTime = 2f, _maxWaitTime = 6f;

    [Header("EnemyOptions")]
    [SerializeField] EnemyStats _enemyStats;

    [SerializeField] bool _stopPatrolling;

    // Start is called before the first frame update
    void Start()
    {
        if (_enemyStats != null)
        {
            _movementSpeed = _enemyStats.movementSpeed;
            _stopDistance = _enemyStats.stopDistance;
            _walkRadius = _enemyStats.movementRadius;
        }
        else
        {
            Debug.LogError("You did not select any stats for " + gameObject.name);
        }

        _agent = GetComponent<NavMeshAgent>();
        _agent.autoBraking = false;
        _agent.stoppingDistance = _stopDistance;
        if (_enemyStats.canJump)
            _agent.areaMask += LayerMask.NameToLayer("Jump");
    }

    // Update is called once per frame
    void Update()
    {
        PatrolToNextPoint();
    }

    float timer;

    void PatrolToNextPoint()
    {
        if (_stopPatrolling)
        {
            return;
        }

        _agent.speed = _movementSpeed;
        if (!_agent.pathPending && _agent.remainingDistance < 0.5f)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                //print("Going to next point");
                timer = Random.Range(_minWaitTime, _maxWaitTime);

                Vector3 randomDirection = Random.insideUnitSphere * _walkRadius;

                randomDirection += transform.position;
                NavMeshHit hit;
                NavMesh.SamplePosition(randomDirection, out hit, _walkRadius, 1);
                Vector3 finalPosition = hit.position;

                _agent.destination = finalPosition;
            }
        }
    }
}
