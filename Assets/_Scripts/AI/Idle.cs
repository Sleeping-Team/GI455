using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Idle : State
{
    public Idle(Customer character, NavMeshAgent agent, Animator anim)
        : base(character, agent, anim)
    {
        Name = STATE.IDLE;
    }

    public override void Enter()
    {
        Character.IsSit = false;
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
        //base.Update();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
