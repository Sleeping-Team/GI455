using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Idle : State
{
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
        base.Enter();
    }

    public override void Update()
    {
        if (Agent.hasPath)
        {
            NextState = new Walking(Character, Agent, Anim);
            Stage = EVENT.EXIT;
        }

        if (Character.Table)
        {
            if (Random.Range(0, 100) < 10)
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
