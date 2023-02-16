using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField, Tooltip("In radius")] private float interactionArea = 1f;
    [SerializeField] private float interactionDistance = 5f;
    [SerializeField] private LayerMask interactableLayer;

    private Ray _rayProperty;

    private PlayerInput _input;

    private void Awake()
    {
        _input = new PlayerInput();
        
        _rayProperty = new Ray(transform.position, transform.forward);
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
        _input.Player.Interact.performed += _ => CastInteraction();
    }

    /// <summary>
    /// Do laser thing to cast interaction Ray with sphere on its end
    /// </summary>
    private void CastInteraction()
    {
        if (Physics.SphereCast(_rayProperty, interactionArea, out RaycastHit hitInfo, interactionDistance, interactableLayer))
        {
            Debug.Log("Interact");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (transform.forward * interactionDistance));
        Gizmos.DrawWireSphere(transform.position + (transform.forward * interactionDistance), interactionArea);
    }
}
