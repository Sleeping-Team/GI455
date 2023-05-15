using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.AI;

public class Customer : CharacterProperties, IInteractable, IDestination, ICharacter
{
    public CustomerState State => _customerState;
    public State CurrentState => _currentState;
    
    public SubCustomer[] SubCustomer => _subCustomer;
    public NavMeshAgent NavAgent => _agent;
    
    public int Quantity => _quantity;
    
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private int _quantity = 1;
    [SerializeField] private Animator _animator;
    [SerializeField] private SubCustomer[] _subCustomer;
    [SerializeField] private float _waitTime = 10.0f;

    private Canvas _interactingCanvas;
    private State _currentState;
    CustomerState _customerState = CustomerState.WaitingTable;

    private bool _iconIsDisabled = false;
    
    public enum CustomerState
    {
        WaitingTable, //Stand at entrance waiting for table
        GetService, // Standby at Table
        Leaving
    }

    private void Awake()
    {
        if (transform.childCount > 2) _quantity = transform.childCount - 1;

        _currentState = new Idle(this, _agent, _animator);
        
        _interactingCanvas = GetComponentInChildren<Canvas>();
    }

    private void FixedUpdate()
    {
        _currentState = _currentState.Process();
    }

    private void OnEnable()
    {
        _customerState = CustomerState.WaitingTable;
    }
    
    public void SetState(CustomerState state)
    {
        _customerState = state;
    }

    public void AssignTable()
    {
        AssignTableServerRpc();
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void AssignTableServerRpc(ServerRpcParams serverRpcParams = default)
    {
        Debug.Log("Assign Table");
        
        _customerState = CustomerState.GetService;

        if (_table == null)
        {
            _table = FloorPlan.Instance.SearchVacantTable(Quantity);
        }
        
        TablePosition tableDetail = _table.GetComponent<TablePosition>();

        TableSetupClientRpc(_table.name, name);

        transform.SetParent(_table.transform);
        tableDetail.AssignObject(TablePosition.ObjectOnFocus.Chair, this.transform);

        gameObject.GetComponent<CapsuleCollider>().enabled = false;
        
        AssignTableClientRpc(name);
    }

    [ClientRpc]
    public void AssignTableClientRpc(string target)
    {
        GameObject focus = GameObject.Find(target);
        focus.GetComponent<Customer>().SetState(CustomerState.GetService);
    }

    [ClientRpc]
    public void TableSetupClientRpc(string target, string customer)
    {
        TableOrder focus = GameObject.Find(target).GetComponent<TableOrder>();
        focus.ChangeState(TableOrder.TableState.Ordering);
        focus.SetStatus(true);
        focus.AssignCustomer(GameObject.Find(customer).GetComponent<Customer>());
        
        //_isWalk = true;
    }

    public override void OnNetworkObjectParentChanged(NetworkObject parentNetworkObject)
    {
        if (_table == null & transform.parent != null) _table = transform.parent.GetComponent<TablePosition>();
        
        name = $"{name}_{NetworkObject.NetworkObjectId}";

        base.OnNetworkObjectParentChanged(parentNetworkObject);
    }

    IEnumerator EatCoroutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
    }

    public IEnumerator PatientTime()
    {
        yield return new WaitForSeconds(_waitTime);
        Debug.Log("have wait");
    }

    public void OnEnter()
    {
        if(_iconIsDisabled) return;
        
        Image interactionIcon = GetComponentInChildren<Image>();
        interactionIcon.transform.DOLocalMoveZ(-1.03f, 1f);
        interactionIcon.DOFade(1f, 1f);
    }

    public void OnExit()
    {
        if(_iconIsDisabled) return;
        
        Image interactionIcon = GetComponentInChildren<Image>();
        interactionIcon.transform.DOLocalMoveZ(-0.74f, 1f);
        interactionIcon.DOFade(0f, 0.5f);
    }

    public void DisableIcon()
    {
        _iconIsDisabled = true;
        _interactingCanvas.gameObject.SetActive(false);
    }

    public void EnableIcon()
    {
        _iconIsDisabled = false;
        _interactingCanvas.gameObject.SetActive(true);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ClearCustomerServerRpc()
    {
        foreach (SubCustomer sub in _subCustomer)
        {
            sub.HaveParent(true);
        }
        NetworkObject.Despawn();
    }

    public void ClearCustomer()
    {
        ClearCustomerServerRpc();
    }

    public void SetDestination(Transform waypoint)
    {
        _agent.enabled = true;
        _agent.SetDestination(waypoint.position);
        foreach (SubCustomer sc in _subCustomer)
        {
            sc.SetDestination(transform);
        }
    }
    
    public void SetChair(Transform c)
    {
        _chair = c;
    }
}

