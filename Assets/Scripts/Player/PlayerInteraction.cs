using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public GameObject DishOnHand => _dishOnHand;

    [SerializeField] private Transform _hand;
    
    private PlayerInput _input;
    
    private GameObject _interactingObject;
    private GameObject _dishOnHand;

    #region Unity Function

    private void Awake()
    {
        _input = new PlayerInput();
    }

    #region Input System Activation

    private void OnEnable()
    {
        _input.Enable();
    }

    private void OnDisable()
    {
        _input.Disable();
    }

    #endregion

    private void Start()
    {
        _input.Player.Interact.performed += _ => Interaction();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_interactingObject == null)
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

        bool pass;
        switch(_interactingObject.tag)
        {
            case "Customer":
                pass = _interactingObject.TryGetComponent(out Customer customer);
                
                if(!pass) return;

                switch (customer.State)
                {
                    case Customer.CustomerState.Entering:
                        //if(!FloorPlan.Instance.TableIsAvailable) return;
                        
                        customer.RandomOrder();
                        customer.SetState(Customer.CustomerState.Waiting);
                        break;
                    case Customer.CustomerState.Waiting:
                        if(_dishOnHand == null) return;

                        if (customer.OrderStatus[_dishOnHand.name])
                        {
                            Debug.LogWarning("Already Served");
                            return;
                        }
                        
                        Debug.Log("Served!");
                        customer.OrderStatus[_dishOnHand.name] = true;
                        
                        Destroy(_dishOnHand);

                        _dishOnHand = null;
                        break;
                }
                break;
            case "Dish":
                if(_dishOnHand != null) return;
                
                _dishOnHand = _interactingObject;
                
                _dishOnHand.transform.SetParent(_hand);
                
                _dishOnHand.transform.localPosition = Vector3.zero;

                _interactingObject = null;
                break;
        }
    }
}
