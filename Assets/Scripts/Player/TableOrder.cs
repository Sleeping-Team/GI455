using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[DefaultExecutionOrder(3)]
public class TableOrder : MonoBehaviour
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
    
    public enum TableState
    {
        Vacant, // No Person
        Thinking, // Delay to think what to eat
        Ordering, // Player need to go and pick up the order
        Waiting, // Wait for food
        Eating, // Customer Eating
        Dirty
    }

    private void Start()
    {
        _dishesOnMenu = Kitchen.Instance.DishesOnMenu;
    }
    
    public void RandomOrder()
    {
        int orderQuantity;
        _tempMenus = Kitchen.Instance.Menus;

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

        foreach (MenuProperties order in Orders)
        {
            _orderStatus.Add(order.name, false);
        }
    }
    
    public void SetOrderStatus(string key, bool value)
    {
        _orderStatus[key] = value;
    }

    public void ChangeState(TableState state)
    {
        _tableState = state;
    }

    public void SetStatus(bool isOccupied)
    {
        _isOccupied = isOccupied;
    }

    [ContextMenu("Reset")]
    public void Reset()
    {
        Orders.Clear();
    }

    public void AssignCustomer(Customer person)
    {
        _customer = person;
    }
}
