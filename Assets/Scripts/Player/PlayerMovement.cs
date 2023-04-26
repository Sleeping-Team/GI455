using System;
using System.Collections.Generic;
using Unity.Netcode.Components;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkTransform
{
    #region Variable

    public bool IsMoving => _isMoving;

    private PlayerProperties _property;
    
    private Rigidbody _rigidbody;
    private Vector2 _movementValue;
    private bool _isMoving;

    public static Dictionary<ulong, PlayerMovement> Players = new Dictionary<ulong, PlayerMovement>();

    #endregion

    #region Unity Function

    private void FixedUpdate()
    {
        if(!IsSpawned || !IsOwner) return;
        
        MovementSystem();
    }

    public void OnMove(InputValue value)
    {
        _movementValue = value.Get<Vector2>();
    }

    #endregion

    #region Network

    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log("On Network");
        
        if (IsOwner)
        {
            var temp = transform.position;
            temp.y = 5f;
            transform.position = temp;

            _rigidbody = GetComponent<Rigidbody>();
            _property = GetComponent<PlayerProperties>();
        }

        Players[OwnerClientId] = this;
        
        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {
        if (Players.ContainsKey(OwnerClientId))
        {
            Players.Remove(OwnerClientId);
        }
        base.OnNetworkDespawn();
    }

    #endregion
    
    #region Custom Function

    /// <summary>
    /// What character have to do to move or while moving.
    /// </summary>
    private void MovementSystem()
    {
        Move(new Vector3(_movementValue.x, 0f, _movementValue.y));
        Facing();
    }

    /// <summary>
    /// Logic behind how to move
    /// </summary>
    /// <param name="control">Value to control direction</param>
    private void Move(Vector3 control)
    {
        if (control != Vector3.zero) _isMoving = true; //animator.SetBool("Walking", true);
        else _isMoving = false; //animator.SetBool("Walking", false);
            
        transform.position += (control * _property.Speed * Time.deltaTime);
        //transform.position = Vector3.Lerp(transform.position, transform.position + control * speed, Time.deltaTime);
    }

    /// <summary>
    /// Method to make character turn smoothly
    /// </summary>
    private void Facing()
    {
        Vector3 inputDirection = new Vector3(_movementValue.x, 0f, _movementValue.y);

        if (inputDirection.magnitude > 0.01f)
        {
            float lookAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
            float effectiveAngle = Mathf.LerpAngle(transform.rotation.eulerAngles.y, lookAngle, _property.LookSmooth);
            transform.rotation = Quaternion.Euler(0f, effectiveAngle, 0f);
        }
    }
    #endregion
}
