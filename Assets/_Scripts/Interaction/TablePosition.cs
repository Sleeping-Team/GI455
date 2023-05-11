using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class TablePosition : MonoBehaviour
{
    public PositionProperties[] ChairPosition => _chairPosition;
    public PositionProperties[] PlatePosition => _platePosition;
    
    [SerializeField] private PositionProperties[] _chairPosition;
    [SerializeField] private PositionProperties[] _platePosition;

    public enum ObjectOnFocus
    {
        Chair,
        Plate
    }

    public void ResetAll()
    {
        foreach (PositionProperties properties in _chairPosition)
        {
            properties.SetOccupied(false);
        }

        foreach (PositionProperties properties in _platePosition)
        {
            properties.SetOccupied(false);
        }
    }
    
    public void AssignObject(ObjectOnFocus focus, Transform thing)
    {
        int index;
        switch (focus)
        {
            case ObjectOnFocus.Chair:
                
                bool pass = thing.TryGetComponent(out Customer agent);
                if (pass)
                {
                    agent.SetDestination(transform);

                    if (agent.Quantity > 1)
                    {
                        foreach (SubCustomer sc in agent.SubCustomer)
                        {
                            sc.SetDestination(thing);
                        }
                    }
                }

                break;
            case ObjectOnFocus.Plate:
                index = PositionProperties.FindEmpty(_platePosition);
                break;
        }
    }

    /// <summary>
    /// Assign Seat to a customer or group of customers
    /// </summary>
    /// <param name="thing">Customer</param>
    public void AssignSeat(Transform thing)
    {
        bool isCustomer = thing.TryGetComponent(out Customer customer);

        if (isCustomer)
        {
            if (customer.Quantity > 1)
            {
                for (int i = 0; i < customer.Quantity; i++)
                {
                    Transform selectedCustomer = customer.transform.GetChild(i);
                    selectedCustomer.position = _chairPosition[i].Location.position;
                    selectedCustomer.rotation = _chairPosition[i].Location.rotation;
                    _chairPosition[i].SetOccupied(true);
                }
            }
            else
            {
                int index = PositionProperties.FindEmpty(_chairPosition);
                thing.position = _chairPosition[index].Location.position;
                thing.rotation = _chairPosition[index].Location.rotation;
                _chairPosition[index].SetOccupied(true);
            }
        }
    }
}