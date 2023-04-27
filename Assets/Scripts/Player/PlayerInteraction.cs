using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerInteraction : NetworkBehaviour
{
    public GameObject DishOnHand => _dishOnHand;

    [SerializeField] private Transform _hand;

    private PlayerInput _input;
    
    [SerializeField] private GameObject _interactingObject;
    private GameObject _dishOnHand;

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
            
            Debug.Log(transform.parent.parent.name);
        }
        else
        {
            gameObject.GetComponent<SphereCollider>().enabled = false;
        }

        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner)
        {
            _input.Disable();
        
            _input.Player.Interact.performed -= _ => Interaction();
        }

        base.OnNetworkDespawn();
    }

    private void Update()
    {
        if (IsOwner && Input.GetKeyDown(KeyCode.E))
        {
            Interaction();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_interactingObject == null && (!other.CompareTag("Disable") && !other.CompareTag("Untagged")))
        {
            _interactingObject = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_interactingObject == other.gameObject)
        {
            _interactingObject = null;
        }
    }

    #endregion

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
                        customer.AssignTable(FloorPlan.Instance.SearchVacantTable(customer.Quantity));
                        break;
                }
                break;
            case "Table":
                pass = _interactingObject.TryGetComponent(out TableOrder table);
                
                if(!pass) return;

                switch (table.State)
                {
                    case TableOrder.TableState.Ordering:
                        //if(!FloorPlan.Instance.TableIsAvailable) return;
                        
                        table.RandomOrder();
                        table.ChangeState(TableOrder.TableState.Waiting);
                        break;
                    case TableOrder.TableState.Waiting:
                        if(_dishOnHand == null) return;

                        if (table.OrderStatus[_dishOnHand.name])
                        {
                            Debug.LogWarning("Already Served");
                            return;
                        }
                        
                        Debug.Log("Served!");
                        table.OrderStatus[_dishOnHand.name] = true;

                        Grabbable onHandDish = _dishOnHand.GetComponent<Grabbable>();
                        
                        onHandDish.PlaceOnTable(table.transform);

                        _dishOnHand = null;

                        bool allServed = false;
                        
                        foreach (string dish in table.OrderStatus.Keys)
                        {
                            allServed = table.OrderStatus[dish];
                            if (!allServed) break;
                        }

                        if (allServed)
                        {
                            table.ChangeState(TableOrder.TableState.Dirty);
                            Destroy(table.Customers.gameObject);
                        }

                        break;
                    case TableOrder.TableState.Dirty:
                        // foreach (GameObject dish in table.transform)
                        // {
                        //     Destroy(dish);
                        // }
                        table.ChangeState(TableOrder.TableState.Vacant);
                        break;
                }
                break;
            case "Dish":
                if(_dishOnHand != null) return;
                
                Debug.Log("Activated Dish Protocal");
                
                _dishOnHand = _interactingObject;
                
                _dishOnHand.GetComponent<Grabbable>().CarryLogic();
                
                // _dishOnHand.transform.SetParent(_hand);
                //
                // _dishOnHand.transform.localPosition = Vector3.zero;
            
                _interactingObject = null;
                break;
        }
    }
    
}
