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
                    Transform selectedCustomer = thing.transform.GetChild(i);
                    selectedCustomer.position = _chairPosition[i].Location.position;
                    selectedCustomer.rotation = _chairPosition[i].Location.rotation;

                    if (i > 0)
                    {
                        ICharacter ic = selectedCustomer.GetComponent(typeof(ICharacter)) as ICharacter;
                        ic.SetChair(_chairPosition[i].Location);
                    }
                    else
                    {
                        customer.SetChair(_chairPosition[i].Location);
                    }
                    
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