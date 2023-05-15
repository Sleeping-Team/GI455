using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Idle : State
{
    private bool _isSubCustomer;
    
    public Idle(CharacterProperties character, NavMeshAgent agent, Animator anim)
        : base(character, agent, anim)
    {
        Name = STATE.IDLE;
    }

    public override void Enter()
    {
        if (Character.Table) Character.IsSit = true;
        else Character.IsSit = false;
        Character.IsWalk = false;

        _isSubCustomer = Character.TryGetComponent(out SubCustomer sc);
        
        base.Enter();
    }

    public override void Update()
    {
        if (Agent.hasPath)
        {
            NextState = new Walking(Character, Agent, Anim);
            Stage = EVENT.EXIT;
        }

        if(!_isSubCustomer) return;
        
        if (Character.Chair)
        {
            Character.IsSit = true;
            if (Random.Range(0, 1000000) < 100)
            {
                NextState = new Roaming(Character, Agent, Anim);
                Stage = EVENT.EXIT;
            }
        }
        
        //base.Update();
    }

    public override void Exit()
    {
        Character.IsSit = false;
        base.Exit();
    }
}
