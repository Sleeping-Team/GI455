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
        TablePosition tableDetail = _table.GetComponent<TablePosition>();
        
        theTable.ChangeState(TableOrder.TableState.Ordering);
        theTable.SetStatus(true);
        theTable.AssignCustomer(this);
        
        transform.SetParent(_table.transform);
        tableDetail.AssignObject(TablePosition.ObjectOnFocus.Chair, this.transform);
    }
    
    IEnumerator EatCoroutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
    }
}

