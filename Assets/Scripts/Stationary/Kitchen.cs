using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[DefaultExecutionOrder(0)]
public class Kitchen : Singletor<Kitchen>
{
    public List<MenuProperties> Menus => _menus;
    
    public int DishesOnMenu { get; private set; }
    public bool HaveOnSpace { get; private set; }

    [SerializeField] private float _orderProcessDelay = 3f;
    [SerializeField] private List<MenuProperties> _menus;
    [SerializeField] private Transform[] _countersPosition;

    private CounterProperties[] _counters;

    private List<Coroutine> _orderProcess = new List<Coroutine>();
    
    private void Awake()
    {
        DishesOnMenu = _menus.Count;

        TableOrder.OnCustomerOrder += Order;

        int counterQuantity = _countersPosition.Length;
        _counters = new CounterProperties[counterQuantity];
        for (int i = 0; i < counterQuantity; i++)
        {
            _counters[i] = new CounterProperties(i, _countersPosition[i]);
        }
    }

    private void Order(List<MenuProperties> orders)
    {
        Debug.Log("Receive Order");
        
        for (int i = 0; i < orders.Count; i++)
        {
            _orderProcess.Add(StartCoroutine(ProcessOrder(i, orders[i])));
        }
    }

    IEnumerator ProcessOrder(int counterIndex, MenuProperties order)
    {
        Debug.Log($"Start Order: {order.name}");
        yield return new WaitForSeconds(_orderProcessDelay);
        Debug.Log($"Cooked Order: {order.name}");
        
        GameObject dish = Instantiate(order.prefab, _counters[counterIndex].Position);
        dish.name = order.name;

        _counters[counterIndex].IsOccupied = true;
    }
}

[Serializable]
public class CounterProperties
{
    public int Index;
    public Transform Position;
    public bool IsOccupied;

    public CounterProperties(int index, Transform position)
    {
        Index = index;
        Position = position;
        IsOccupied = false;
    }
}
