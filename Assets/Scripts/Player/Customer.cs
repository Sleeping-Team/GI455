using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    public CustomerState State => _customerState;
    
    [SerializeField] CustomerState _customerState = CustomerState.WaitingTable;

    [SerializeField]private TablePosition _table;
    public int Quantity = 1;
    
    public enum CustomerState
    {
        WaitingTable, //Stand at entrance waiting for table
        OnTable, // Standby at Table
        Roaming
    }
    
    private void OnEnable()
    {
        _customerState = CustomerState.WaitingTable;
    }
    
    public void SetState(CustomerState state)
    {
        _customerState = state;
    }

    public void AssignTable(TablePosition table)
    {
        if(table == null) return;
        
        Debug.Log("Assign Table");
        
        _table = table;
        _customerState = CustomerState.OnTable;

        TableOrder theTable = _table.GetComponent<TableOrder>();
        
        theTable.ChangeState(TableOrder.TableState.Ordering);
        theTable.SetStatus(true);
        theTable.AssignCustomer(this);
        
        transform.SetParent(_table.ChairPosition[0]);
        transform.localPosition = Vector3.zero;
    }
    
    IEnumerator EatCoroutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
    }
}

