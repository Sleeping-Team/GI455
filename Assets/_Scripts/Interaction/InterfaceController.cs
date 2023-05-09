using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class InterfaceController : NetworkBehaviour
{
    public static InterfaceController Instance;

    [SerializeField] private Transform _queueHolder;
    [SerializeField] private OrderQueue _indicatorPrefab;

    private NetworkVariable<int> orderQuantity = new NetworkVariable<int>();

    private void Awake()
    {
        Instance = this;

        orderQuantity.Value = 0;
    }

    public void Display()
    {
        AddQueueServerRpc();
        
        OrderQueue indicator = Instantiate(_indicatorPrefab, _queueHolder);
        indicator.Setup(orderQuantity.Value);
        indicator.Sequence();
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddQueueServerRpc()
    {
        orderQuantity.Value++;
    }
}
