using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private PlayerInput _input;
    
    private GameObject _interactingObject;

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
        _input.Player.Interact.performed += _ => BasicInteraction();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null)
        {
            _interactingObject = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other != null)
        {
            _interactingObject = null;
        }
    }

    #endregion

    private void BasicInteraction()
    {
        if(_interactingObject != null) Debug.Log("Object Here");
    }
}
