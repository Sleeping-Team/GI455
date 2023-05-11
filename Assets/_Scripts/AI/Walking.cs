using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Walking : State
{
    public Walking(Customer character, NavMeshAgent agent, Animator anim)
        : base(character, agent, anim)
    {
        Name = STATE.WALKING;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        if (Agent.remainingDistance < .5f)
        {
            Agent.isStopped = true;
            
            Character.Table.AssignSeat(Character.transform);

            NextState = new Idle(Character, Agent, Anim);
            Stage = EVENT.EXIT;
        }
        
        //base.Update();
    }

    public override void Exit()
    {
        Agent.ResetPath();
        base.Exit();
    }
}
