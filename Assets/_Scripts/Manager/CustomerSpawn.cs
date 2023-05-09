using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class CustomerSpawn : SingletonNetwork<CustomerSpawn>
{
    [SerializeField] private PositionProperties[] _waitPosition;
    [SerializeField] private Customer[] _customerPrefabs;
    [SerializeField] private float _spawnDelay;

    private bool gameover = false;
    private int _customersQuantity;
    
    private void Start()
    {
        _customersQuantity = _customerPrefabs.Length;

        if (IsHost)
        {
            StartCoroutine(DoSpawn());
        }
    }

    IEnumerator DoSpawn()
    {
        while (!gameover)
        {
            int index = PositionProperties.FindEmpty(_waitPosition);
            Debug.Log(index);
            if (index >= 0)
            {
                _waitPosition[index].SetOccupied(true);
                Customer customer = Instantiate(_customerPrefabs[Random.Range(0, _customersQuantity)], _waitPosition[index].Location);
                customer.transform.localPosition = Vector3.zero;

                NetworkObject customerNetwork = customer.GetComponent<NetworkObject>();
                if(!customerNetwork.IsSpawned) customerNetwork.Spawn();
            }
            else
            {
                Debug.Log("Location is full");
            }

            yield return new WaitForSeconds(_spawnDelay);
        }
    }
}
