using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class State
{
    public event Action OnStateChanged;
    
    public enum STATE
    {
        IDLE,
        WALKING,
        ROAMING,
    }

    public enum EVENT
    {
        ENTER,
        UPDATE,
        EXIT
    }

    public STATE Name;

    protected EVENT Stage;
    protected CharacterProperties Character;
    protected NavMeshAgent Agent;
    protected Animator Anim;
    protected State NextState;

    public State(CharacterProperties character, NavMeshAgent agent, Animator anim)
    {
        Character = character;
        Agent = agent;
        Anim = anim;
        Stage = EVENT.ENTER;
    }

    /// <summary>
    /// What to do on Enter this stage
    /// </summary>
    public virtual void Enter()
    {
        Debug.Log($"{Character.name} now entering Stage : {Name.ToString()}");
        Stage = EVENT.UPDATE;
    }

    /// <summary>
    /// What to do when is in this stage. Including condition to change stage.
    /// *You need to Implement "Stage = EVENT.EXIT" on Change Stage by your self*
    /// </summary>
    public virtual void Update()
    {
        Stage = EVENT.UPDATE;
    }

    /// <summary>
    /// What to do on Exit this stage (Wrap up)
    /// </summary>
    public virtual void Exit()
    {
        Debug.Log($"{Character.name} now exiting Stage : {Name.ToString()}");
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
            OnStateChanged?.Invoke();
            return NextState;
        }

        return this;
    }
}
