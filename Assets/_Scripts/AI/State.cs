using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class State
{
    public enum STATE
    {
        IDLE,
        WALKING,
        LEAVING,
    }

    public enum EVENT
    {
        ENTER,
        UPDATE,
        EXIT
    }

    public STATE Name;

    protected EVENT Stage;
    protected GameObject Character;
    protected NavMeshAgent Agent;
    protected Animator Anim;
    protected Transform Destination;
    protected State NextState;

    public State(GameObject character, NavMeshAgent agent, Animator anim, Transform destination)
    {
        Character = character;
        Agent = agent;
        Anim = anim;
        Destination = destination;
        Stage = EVENT.ENTER;
    }

    public virtual void Enter()
    {
        Stage = EVENT.UPDATE;
    }

    public virtual void Update()
    {
        Stage = EVENT.UPDATE;
    }

    public virtual void Exit()
    {
        Stage = EVENT.UPDATE;
    }

    public State Process()
    {
        if (Stage == EVENT.ENTER)
        {
            Enter();
        }

        if (Stage == EVENT.UPDATE)
        {
            Update();
        }

        if (Stage == EVENT.EXIT)
        {
            Exit();
            return NextState;
        }

        return this;
    }
}
