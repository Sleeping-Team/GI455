using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.AI;

public class Customer : NetworkBehaviour, IInteractable
{
    public CustomerState State => _customerState;
    public int Quantity => _quantity;
    public TablePosition Table => _table;

    public bool IsWalk => _isWalk;
    public bool IsSit => _isSit;
    
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private int _quantity = 1;
    [SerializeField] private TablePosition _table;
    [SerializeField] private Animator _animator;

    private State _currentState;
    CustomerState _customerState = CustomerState.WaitingTable;

    private bool _isWalk;
    private bool _isSit;
    
    public enum CustomerState
    {
        WaitingTable, //Stand at entrance waiting for table
        OnTable, // Standby at Table
        Roaming
    }

    private void Awake()
    {
        if (transform.childCount > 2) _quantity = transform.childCount - 1;

        _currentState = new Idle(this, _agent, _animator);
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
        
        _customerState = CustomerState.OnTable;

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
        focus.GetComponent<Customer>().SetState(CustomerState.OnTable);
    }

    [ClientRpc]
    public void TableSetupClientRpc(string target, string customer)
    {
        TableOrder focus = GameObject.Find(target).GetComponent<TableOrder>();
        focus.ChangeState(TableOrder.TableState.Ordering);
        focus.SetStatus(true);
        focus.AssignCustomer(GameObject.Find(customer).GetComponent<Customer>());
        
        _isWalk = true;
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

    public void OnEnter()
    {
        Image interactionIcon = GetComponentInChildren<Image>();
        interactionIcon.transform.DOLocalMoveZ(-1.03f, 1f);
        interactionIcon.DOFade(1f, 1f);
    }

    public void OnExit()
    {
        Image interactionIcon = GetComponentInChildren<Image>();
        interactionIcon.transform.DOLocalMoveZ(-0.74f, 1f);
        interactionIcon.DOFade(0f, 0.5f);
    }
}

