using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Transform _playerTransform;

    [SerializeField] private float _visionRange = 15f;
    [SerializeField] private float _searchTime = 5f;
    [SerializeField] private Transform[] _patrolPoints;
    private int _patrolIndex;
    private float _searchTimer;

    public enum EnemyState { Patrolling, Searching, Chasing }
    public EnemyState currentState;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _playerTransform = GameObject.FindWithTag("Player").transform;
    }

    void Start()
    {
        currentState = EnemyState.Patrolling;
        SetPatrolPoint();
    }

    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Patrolling:
                Patrol();
                break;
            case EnemyState.Searching:
                Search();
                break;
            case EnemyState.Chasing:
                Chase();
                break;
        }
    }

    void SetPatrolPoint()
    {
        _agent.destination = _patrolPoints[_patrolIndex].position;
        _patrolIndex = (_patrolIndex + 1) % _patrolPoints.Length;
    }

    void Patrol()
    {
        if (InRange(_visionRange))
        {
            currentState = EnemyState.Chasing;
            return;
        }

        if (_agent.remainingDistance < 0.5f)
        {
            SetPatrolPoint();
        }
    }


    void Chase()
    {
        if (!InRange(_visionRange))
        {
           currentState = EnemyState.Searching;
            _searchTimer = _searchTime;
            return;
        }
        
        _agent.destination = _playerTransform.position;
    }

    void Search()
    {
        _searchTimer -= Time.deltaTime;
        if (_searchTimer <= 0)
        {
            currentState = EnemyState.Patrolling;
        }
    }

    bool InRange(float range)
    {
        return Vector3.Distance(transform.position, _playerTransform.position) < range;
    }
}
