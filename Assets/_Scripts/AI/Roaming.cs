using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Roaming : State
{
    public Roaming(Character character, NavMeshAgent agent, Animator anim)
        : base(character, agent, anim)
    {
        Name = STATE.ROAMING;
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
