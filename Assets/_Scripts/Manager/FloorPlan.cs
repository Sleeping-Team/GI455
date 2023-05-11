using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class FloorPlan : NetworkBehaviour
{
    public static FloorPlan Instance;

    public Dictionary<int, List<TableStatus>> TablesStatus => _tablesStatus;
    public Dictionary<string, GameObject> TablesDatabase => _tablesObjects;
    public bool TableIsAvailable { get; private set; }
    public Transform Exit => _exit;
    public Transform[] PointsOfInterest => _pointOfInterest;

    [SerializeField] private Transform _exit;
    [SerializeField] private Transform[] _pointOfInterest;
    private Dictionary<int, List<TableStatus>> _tablesStatus = new Dictionary<int, List<TableStatus>>();
    private Dictionary<string, GameObject> _tablesObjects = new Dictionary<string, GameObject>();

    private void Awake()
    {
        Instance = this;
    
        _tablesStatus.Add(2, new List<TableStatus>());
        _tablesStatus.Add(4, new List<TableStatus>());
        Debug.Log("Created Dictionary");
        
        GameObject[] tables = GameObject.FindGameObjectsWithTag("Table");
        Debug.Log($"Found {tables.Length} tables");

        foreach (GameObject table in tables)
        {
            if (table.TryGetComponent(out TablePosition tablePosition))
            {
                if (tablePosition.ChairPosition.Length > 2)
                {
                    Debug.Log($"{table.name} is 4 seat");
                    _tablesStatus[4].Add(new TableStatus(tablePosition));
                }
                else
                {
                    Debug.Log($"{table.name} is 4 seat");
                    Debug.Log("Customer have not more than 2");
                    _tablesStatus[2].Add(new TableStatus(tablePosition));
                }
                _tablesObjects.Add(table.name, table);
            }
        }
    }

    public TablePosition SearchVacantTable(int customerQuantity)
    {
        Debug.Log("Finding table");
        if (customerQuantity <= 2)
        {
            Debug.Log("Less than 2");
            
            List<TableStatus> tableStatusList = new List<TableStatus>();
            
            tableStatusList.AddRange(_tablesStatus[2]);
            tableStatusList.AddRange(_tablesStatus[4]);

            foreach (TableStatus table in tableStatusList)
            {
                if (!table.IsOccupied)
                {
                    Debug.Log("Found Vacant table");
                    table.IsOccupied = true;
                    return table.Table;
                }
            }
        }
        else
        {
            Debug.Log("More than 2");            
            
            List<TableStatus> tableStatusList = _tablesStatus[4];

            foreach (TableStatus table in tableStatusList)
            {
                if (!table.IsOccupied)
                {
                    Debug.Log("Found Vacant table");
                    table.IsOccupied = true;
                    return table.Table;
                }
            }
        }

        return null;
    }
}

[Serializable]
public class TableStatus
{
    public TablePosition Table;
    public bool IsOccupied = false;

    public TableStatus(TablePosition table)
    {
        Table = table;
        IsOccupied = false;
    }
}
