using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class PlayerInteraction : NetworkBehaviour
{
    public GameObject DishOnHand => _dishOnHand;

    [SerializeField] private Transform _hand;

    private PlayerInput _input;
    
    [SerializeField] private GameObject _interactingObject;
    private GameObject _dishOnHand;

    private Color alphaWhite = new Color(255f, 255f, 255f, 0f);

    #region Unity Function

    private void Awake()
    {
        _input = new PlayerInput();
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            _input.Enable();
        
            _input.Player.Interact.performed += _ => Interaction();
        }
        else gameObject.GetComponent<SphereCollider>().enabled = false;

        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner) _input.Disable();
        

        base.OnNetworkDespawn();
    }

    private void Update()
    {
        if (IsOwner && Input.GetKeyDown(KeyCode.R)) _interactingObject = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_interactingObject == null && (!other.CompareTag("Disable") && !other.CompareTag("Untagged")))
        {
            _interactingObject = other.gameObject;

            if (_interactingObject.CompareTag("Table") && _interactingObject.GetComponent<TableOrder>().State != TableOrder.TableState.Vacant)
            {
                if(_dishOnHand == null && _interactingObject.GetComponent<TableOrder>().State == TableOrder.TableState.Waiting ) return;
                
                _interactingObject.GetComponent<TableOrder>().EnableIcon();
                _interactingObject.GetComponent<TableOrder>().OnEnter();
            }
            else if (_interactingObject.CompareTag("Customer")) _interactingObject.GetComponent<Customer>().OnEnter();
            else if (_interactingObject.CompareTag("Dish")) _interactingObject.GetComponent<Grabbable>().OnEnter();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_interactingObject == other.gameObject)
        {
            if (_interactingObject.CompareTag("Table") && _interactingObject.GetComponent<TableOrder>().State != TableOrder.TableState.Vacant)
            {
                if(_dishOnHand == null && _interactingObject.GetComponent<TableOrder>().State == TableOrder.TableState.Waiting ) return;
                
                _interactingObject.GetComponent<TableOrder>().OnExit();
                _interactingObject.GetComponent<TableOrder>().DisableIcon();
            }
            else if (_interactingObject.CompareTag("Customer")) _interactingObject.GetComponent<Customer>().OnExit();
            else if (_interactingObject.CompareTag("Dish")) _interactingObject.GetComponent<Grabbable>().OnExit();

            _interactingObject = null;
        }
    }

    #endregion

    /// <summary>
    /// All Player Interaction Logic
    /// </summary>
    private void Interaction()
    {
        if(_interactingObject == null) return;

        Debug.Log("Interact");
        
        bool pass;
        switch(_interactingObject.tag)
        {
            case "Customer":
                Debug.Log("It's Customer");
                pass = _interactingObject.TryGetComponent(out Customer customer);
                
                if(!pass) return;
                
                Debug.Log("Get Customer Pass");
                
                switch (customer.State)
                {
                    case Customer.CustomerState.WaitingTable:
                        Debug.Log("Customer is waiting");
                        customer.AssignTable();
                        customer.OnExit();
                        customer.DisableIcon();
                        _interactingObject = null;
                        break;
                }
                break;
            case "Table":
                pass = _interactingObject.TryGetComponent(out TableOrder table);
                
                if(!pass) return;

                Debug.Log("It's Table");
                
                switch (table.State)
                {
                    case TableOrder.TableState.Ordering:
                        Debug.Log("Initiate ordering protocol");
                        table.RandomOrder();
                        table.NextStateServerRpc();
                        table.OnExit();
                        table.DisableIcon();
                        _interactingObject = null;
                        break;
                    case TableOrder.TableState.Waiting:
                        CheckServeStatus(table);
                        if(_dishOnHand == null) return;

                        Debug.Log("Initiate serving protocol");

                        if (!table.OrderStatus.ContainsKey(_dishOnHand.name))
                        {
                            Debug.LogWarning("Not my order");
                            return;
                        }
                        
                        if (table.OrderStatus[_dishOnHand.name])
                        {
                            Debug.LogWarning("Already Served");
                            return;
                        }
                        
                        Debug.Log("Served!");
                        table.Serve(_dishOnHand.name);

                        Grabbable onHandDish = _dishOnHand.GetComponent<Grabbable>();
                        
                        onHandDish.PlaceOnTable(table.name);

                        _dishOnHand = null;
                        _interactingObject = null;

                        if (!IsHost) return;

                        CheckServeStatus(table);
                
                        break;
                    case TableOrder.TableState.Dirty:
                        // foreach (GameObject dish in table.transform)
                        // {
                        //     Destroy(dish);
                        // }
                        Debug.Log("Initiate cleaning Protocol");
                        table.NextStateServerRpc();
                        table.OnExit();
                        _interactingObject = null;
                        break;
                }
                break;
            case "Dish":
                if(_dishOnHand != null) return;
                
                Debug.Log("Activated Dish Protocol");
                
                _dishOnHand = _interactingObject;
                
                Grabbable dish = _dishOnHand.GetComponent<Grabbable>();
                
                dish.CarryLogic();
                dish.OnExit();
                dish.DisableIcon();

                // _dishOnHand.transform.SetParent(_hand);
                //
                // _dishOnHand.transform.localPosition = Vector3.zero;
            
                _interactingObject = null;
                break;
        }
    }

    private static void CheckServeStatus(TableOrder table)
    {
        bool allServed = false;

        foreach (string dish in table.OrderStatus.Keys)
        {
            allServed = table.OrderStatus[dish];
            if (!allServed) break;
        }

        if (allServed)
        {
            table.NextStateServerRpc();
            // table.ClearCustomer();
            table.Customers.SetState(Customer.CustomerState.Leaving);
            table.Customers.SetDestination(FloorPlan.Instance.Exit);
        }
    }
}
