using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[DefaultExecutionOrder(3)]
public class Customer : MonoBehaviour
{
    public static event Action<List<MenuProperties>> OnCustomerOrder;
    
    public CustomerState State => _customerState;
    
    /// <summary>
    /// No Table should be -1
    /// </summary>
    [SerializeField] int _maximumOrder;

    public List<MenuProperties> Orders = new List<MenuProperties>();

    [SerializeField] CustomerState _customerState = CustomerState.Entering;

    private int _dishesOnMenu;
    List<MenuProperties> _tempMenus = new List<MenuProperties>();

    public Dictionary<string, bool> OrderStatus => _orderStatus;
    private Dictionary<string, bool> _orderStatus = new Dictionary<string, bool>();

    private void Start()
    {
        _dishesOnMenu = Kitchen.Instance.DishesOnMenu;
    }

    public enum CustomerState
    {
        Entering, //Enter the restaurant
        WaitingTable, //Stand at entrance waiting for table
        Deciding, //Choosing what to eat
        Ordering, //Order a menu
        Waiting, //Waiting for food/drinks
        Eating, //Served enjoy meals
        Leaving, //Done Eating
    }

    private void OnEnable()
    {
        _customerState = CustomerState.Entering;
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

    public void SetState(CustomerState state)
    {
        _customerState = state;
    }

    public void SetOrderStatus(string key, bool value)
    {
        _orderStatus[key] = value;
    }

    [ContextMenu("Reset")]
    public void Reset()
    {
        Orders.Clear();
        _customerState = CustomerState.Entering;
    }
}
