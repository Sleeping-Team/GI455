using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Walking : State
{
    private Customer _customer;
    private SubCustomer _subCustomer;
    private bool _isCustomer;
    private bool _isSubCustomer;

    public Walking(CharacterProperties character, NavMeshAgent agent, Animator anim)
        : base(character, agent, anim)
    {
        Name = STATE.WALKING;
    }

    public override void Enter()
    {
        _isCustomer = Character.TryGetComponent(out _customer);
        if (!_isCustomer)
        {
            _isSubCustomer = Character.TryGetComponent(out _subCustomer);
            if (_isSubCustomer)
            {
                _subCustomer.Master.CurrentState.OnStateChanged += () => ChangeToIdleState();
                _subCustomer.HaveParent(false);
                
                if (_subCustomer.Master.State == Customer.CustomerState.Leaving)
                {
                    _subCustomer.Follow(true);
                }
            }
        }

        Transform characterTransform = Character.transform;
        
        characterTransform.GetChild(0).position = characterTransform.position;
        characterTransform.GetChild(0).rotation = characterTransform.rotation;
        
        Character.IsWalk = true;
        base.Enter();
    }

    public override void Update()
    {
        if (_isCustomer)
        {
            if (Agent.remainingDistance < .5f)
            {
                if (_customer.State == Customer.CustomerState.Leaving)
                {
                    _customer.ClearCustomer();
                }
                else
                {
                    Agent.isStopped = true;

                    if (_customer.Quantity > 1)
                    {
                        foreach (SubCustomer little in _customer.SubCustomer)
                        {
                            little.HaveParent(true);
                            little.Follow(false);
                        }
                    }

                    _customer.Table.AssignSeat(Character.transform);

                    ChangeToIdleState();
                }
                
            }
        }
        else
        {
            if (!Character.Chair)
            {
                _subCustomer.Follow(true);
            }
            else
            {
                if (Agent.remainingDistance < .5f)
                {
                    _subCustomer.Sit();
                    ChangeToIdleState();
                }
            }
        }

        //base.Update();
    }

    public override void Exit()
    {
        if(_isCustomer) Agent.ResetPath();
        else
        {
            _subCustomer.Follow(false);
            _subCustomer.Master.CurrentState.OnStateChanged -= () => ChangeToIdleState();
        }
        Agent.enabled = false;
        
        Character.IsWalk = false;
        base.Exit();
    }

    private void ChangeToIdleState()
    {
        NextState = new Idle(Character, Agent, Anim);
        Stage = EVENT.EXIT;
    }
}
