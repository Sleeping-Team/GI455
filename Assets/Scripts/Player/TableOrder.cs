using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

[DefaultExecutionOrder(3)]
public class TableOrder : NetworkBehaviour
{
    public static event Action<List<MenuProperties>> OnCustomerOrder;

    public TableState State => _tableState;
    public bool IsOccupied => _isOccupied;
    public Customer Customers => _customer;
    
    /// <summary>
    /// No Table should be -1
    /// </summary>
    [SerializeField] int _maximumOrder;

    public List<MenuProperties> Orders = new List<MenuProperties>();

    
    private int _dishesOnMenu;
    List<MenuProperties> _tempMenus = new List<MenuProperties>();

    public Dictionary<string, bool> OrderStatus => _orderStatus;
    private Dictionary<string, bool> _orderStatus = new Dictionary<string, bool>();

    [SerializeField] private TableState _tableState = TableState.Vacant;

    private bool _isOccupied = false;
    private Customer _customer;

    public List<string> TempOrder = new List<string>();
    
    public enum TableState
    {
        Vacant, // No Person
        //Thinking, // Delay to think what to eat
        Ordering, // Player need to go and pick up the order
        Waiting, // Wait for food
        //Eating, // Customer Eating
        Dirty
    }

    public override void OnNetworkSpawn()
    {
        _dishesOnMenu = Kitchen.Instance.DishesOnMenu;
        
        base.OnNetworkSpawn();
    }

    public void RandomOrder()
    {
        Debug.Log("Run Random Order");
        RandomOrderServerRpc();
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void RandomOrderServerRpc()
    {
        Debug.Log("In Random Order Server RPC");
        
        int orderQuantity;
        _tempMenus = new List<MenuProperties>(Kitchen.Instance.Menus);

        if(_maximumOrder > _dishesOnMenu) orderQuantity = Random.Range(1, _dishesOnMenu + 1);
        else orderQuantity = Random.Range(1, _maximumOrder + 1);

        for (int i = 0; i < orderQuantity; i++)
        {
            int randomOrder = Random.Range(0, _tempMenus.Count);
            
            Orders.Add(_tempMenus[randomOrder]);

            _tempMenus.Remove(_tempMenus[randomOrder]);
            
            Debug.Log($"Randomize Order #{i+1}");
        }
        
        OnCustomerOrder?.Invoke(Orders);

        ClearOrderClientRpc(name);
        
        foreach (MenuProperties order in Orders)
        {
            _orderStatus.Add(order.name, false);
            ListOrderClientRpc(name, order.name);
        }

        WrapUpClientRpc(name);
    }

    [ClientRpc]
    public void ListOrderClientRpc(string name,string key)
    {
        TableOrder focus = FloorPlan.Instance.TablesDatabase[name].GetComponent<TableOrder>();
        focus.TempOrder.Add(key);
    }

    [ClientRpc]
    public void WrapUpClientRpc(string name)
    {
        TableOrder focus = FloorPlan.Instance.TablesDatabase[name].GetComponent<TableOrder>();
        _orderStatus = new Dictionary<string, bool>();
        foreach (string order in focus.TempOrder)
        {
            _orderStatus.Add(order, false);
        }
    }
    
    [ClientRpc]
    public void ClearOrderClientRpc(string name)
    {
        FloorPlan.Instance.TablesDatabase[name].GetComponent<TableOrder>().TempOrder = new List<string>();
    }

    [ServerRpc]
    public void ClearCustomerServerRpc()
    {
        Customers.NetworkObject.Despawn();
    }

    public void ClearCustomer()
    {
        ClearCustomerServerRpc();
    }

    public void MapOrder(Dictionary<string, bool> data)
    {
        _orderStatus = data;
    }
    
    public void SetOrderStatus(string key, bool value)
    {
        _orderStatus[key] = value;
    }
    
    public void ChangeState(TableState state)
    {
        _tableState = state;

        Debug.Log($"Change {name}'s state to {state.ToString()}");
    }

    public void NextState()
    {
        if (_tableState == TableState.Dirty)
        {
            _tableState = TableState.Vacant;
        }
        else
        {
            _tableState++;
        }
    }

    [ClientRpc]
    public void NextStateClientRpc(string name)
    {
        TableOrder focus = GameObject.Find(name).GetComponent<TableOrder>();
        focus.NextState();
    }

    [ServerRpc(RequireOwnership = false)]
    public void NextStateServerRpc()
    {
        NextStateClientRpc(name);
    }
    
    public void SetStatus(bool isOccupied)
    {
        _isOccupied = isOccupied;
    }
    
    [ContextMenu("Reset")]
    public void Reset()
    {
        Orders.Clear();
        Debug.Log("is now clear oder");
    }
    
    public void AssignCustomer(Customer person)
    {
        _customer = person;
    }

    public void Serve(string order)
    {
        ServeServerRpc(order);
    }

    [ServerRpc]
    public void ServeServerRpc(string order)
    {
        ServeClientRpc(order);
    }

    [ClientRpc]
    public void ServeClientRpc(string order)
    {
        TableOrder focus = GameObject.Find(name).GetComponent<TableOrder>();
        Served(order);
    }

    public void Served(string order)
    {
        _orderStatus[order] = true;
    }
}
