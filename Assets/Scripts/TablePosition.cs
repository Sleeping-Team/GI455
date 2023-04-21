using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                index = PositionProperties.FindEmpty(_chairPosition);
                thing.position = _chairPosition[index].Location.position;
                _chairPosition[index].SetOccupied(true);
                break;
            case ObjectOnFocus.Plate:
                index = PositionProperties.FindEmpty(_platePosition);
                break;
        }
    }
}