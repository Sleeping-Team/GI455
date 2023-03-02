using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class FloorPlan : Singletor<FloorPlan>
{
    public Dictionary<GameObject, bool> TableStatus => _tableStatus;
    public bool TableIsAvailable { get; private set; }

    private Dictionary<GameObject, bool> _tableStatus = new Dictionary<GameObject, bool>();

    private void Awake()
    {
        TableIsAvailable = true;
        
        GameObject[] tables = GameObject.FindGameObjectsWithTag("Table");

        foreach (GameObject table in tables)
        {
            _tableStatus.Add(table, true);
        }
    }

    public void SetTableStatus(GameObject key, bool value)
    {
        _tableStatus[key] = value;

        TableIsAvailable = _tableStatus.ContainsValue(true);
    }
}
