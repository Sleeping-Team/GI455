using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

[DefaultExecutionOrder(0)]
public class Kitchen : NetworkBehaviour
{
    public static Kitchen Instance;

    public List<MenuProperties> Menus => _menus;
    public float OrderProcessDelay => _orderProcessDelay;

    public int DishesOnMenu { get; private set; }
    public bool HaveOnSpace { get; private set; }

    [SerializeField] private float _orderProcessDelay = 3f;
    [SerializeField] private List<MenuProperties> _menus;
    [SerializeField] private Transform[] _countersPosition;

    private CounterProperties[] _counters;

    private List<Coroutine> _orderProcess = new List<Coroutine>();

    private void Awake()
    {
        Instance = this;
        
        DishesOnMenu = _menus.Count;

        int counterQuantity = _countersPosition.Length;
        _counters = new CounterProperties[counterQuantity];
        for (int i = 0; i < counterQuantity; i++)
        {
            _counters[i] = new CounterProperties(i, _countersPosition[i]);
        }

        TableOrder.OnCustomerOrder += Order;
    }

    private void Order(List<MenuProperties> orders)
    {
        Debug.Log("Receive Order");

        Debug.LogWarning($"Before execution: {_menus.Count}");
        
        for (int i = 0; i < orders.Count; i++)
        {
            _orderProcess.Add(StartCoroutine(ProcessOrder(i, orders[i])));
        }
    }

    IEnumerator ProcessOrder(int counterIndex, MenuProperties order)
    {
        Debug.LogWarning($"Enter Function: {_menus.Count}");
        Debug.Log($"Start Order: {order.name}");
        yield return new WaitForSeconds(_orderProcessDelay);
        Debug.Log($"Cooked Order: {order.name}");

        GameObject dish = Instantiate(order.prefab, _counters[counterIndex].Position);
        dish.GetComponent<NetworkObject>().Spawn();

        _counters[counterIndex].IsOccupied = true;
        Debug.LogWarning($"Exit Function: {_menus.Count}");
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