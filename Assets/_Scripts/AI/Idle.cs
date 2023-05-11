using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Idle : State
{
    public Idle(GameObject character, NavMeshAgent agent, Animator anim, Transform destination)
        : base(character, agent, anim, destination)
    {
        Name = STATE.IDLE;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
