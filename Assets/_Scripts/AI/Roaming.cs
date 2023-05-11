using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Roaming : State
{
    private List<Transform> _poiRef = new List<Transform>();
    private List<Transform> _poiOrder = new List<Transform>();
    
    private int _pointToGo;
    private int _focusPointIndex;

    private SubCustomer _sCustomer;

    public Roaming(CharacterProperties character, NavMeshAgent agent, Animator anim)
        : base(character, agent, anim)
    {
        Name = STATE.ROAMING;
    }

    public override void Enter()
    {
        _focusPointIndex = 0;
        
        _poiRef = new List<Transform>(FloorPlan.Instance.PointsOfInterest);

        _pointToGo = Random.Range(1, _poiRef.Count);

        for (int i = 0; i < _pointToGo; i++)
        {
            int index = Random.Range(0, _poiRef.Count);
            _poiOrder.Add(_poiRef[index]);
            _poiRef.RemoveAt(index);
        }

        _sCustomer = Character.GetComponent<SubCustomer>();
        _sCustomer.SetDestination(_poiOrder[_focusPointIndex]);
        
        base.Enter();
    }

    public override void Update()
    {
        if (_sCustomer.Master.State == Customer.CustomerState.Leaving)
        {
            NextState = new Walking(Character, Agent, Anim);
            _sCustomer.Follow(true);
            Stage = EVENT.EXIT;
        }
        
        if (Agent.remainingDistance < .5f)
        {
            _focusPointIndex++;
            if(_focusPointIndex < _pointToGo) _sCustomer.SetDestination(_poiOrder[_focusPointIndex]);
            else
            {
                NextState = new Walking(Character, Agent, Anim);
                _sCustomer.SetDestination(Character.Chair);
                Stage = EVENT.EXIT;
            }
        }

        //base.Update();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
