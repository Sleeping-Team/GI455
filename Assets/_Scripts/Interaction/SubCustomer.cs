using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SubCustomer : CharacterProperties, IDestination, ICharacter
{
    public Customer Master => _master;
    
    [SerializeField] private Customer _master;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Animator _animator;
    [SerializeField] private bool _isFollowMaster = false;

    private State _currentState;

    private void Awake()
    {
        _currentState = new Idle(this, _agent, _animator);
    }

    private void FixedUpdate()
    {
        _currentState = _currentState.Process();
    }

    public void HaveParent(bool withMaster)
    {
        if (withMaster)
        {
            transform.SetParent(_master.transform);
            transform.SetSiblingIndex(1);
        }
        else
        {
            transform.SetParent(null);
        }
    }

    public void SetDestination(Transform waypoint)
    {
        _agent.enabled = true;
        _agent.SetDestination(waypoint.position);
    }

    public void Follow(bool isFollow)
    {
        _isFollowMaster = isFollow;

        if (_isFollowMaster)
        {
            _agent.SetDestination(_master.transform.position);
        }
        else
        {
            _agent.enabled = true;
            _agent.ResetPath();
            _agent.enabled = false;
        }
    }

    public void Sit()
    {
        if(!_chair) return;

        _agent.enabled = false;
        transform.position = _chair.position;
        transform.rotation = _chair.rotation;
    }
    
    public void SetChair(Transform c)
    {
        _chair = c;
    }
}
