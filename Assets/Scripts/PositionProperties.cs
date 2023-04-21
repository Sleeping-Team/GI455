using System;
using UnityEngine;

[Serializable]
public class PositionProperties
{
    public Transform Location => _location;
    public bool IsOccupied => _isOccupied;
    
    [SerializeField] private Transform _location;
    private bool _isOccupied = false;

    public void SetOccupied(bool status)
    {
        _isOccupied = status;
    }
    
    public static int FindEmpty(PositionProperties[] focus)
    {
        if (focus.Length == 0) return 0;
        
        for (int i = 0; i < focus.Length; i++)
        { 
            if (!focus[i]._isOccupied) return i;
        }
        
        return -1;
    }

   
}
