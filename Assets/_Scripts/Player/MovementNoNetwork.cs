using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementNoNetwork : MonoBehaviour
{
    #region Variable

    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private Animator animator;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float lookSmooth = 0.05f;

    private Vector2 _movementValue;

    #endregion

    #region Unity Function

    private void Update()
    {
        MovementSystem();
    }

    public void OnMove(InputValue value)
    {
        _movementValue = value.Get<Vector2>();
        
        if(_movementValue != Vector2.zero) animator.SetBool("Walking", true);
        else animator.SetBool("Walking", false);
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
        transform.position += (control * speed * Time.deltaTime);
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
            float effectiveAngle = Mathf.LerpAngle(transform.rotation.eulerAngles.y, lookAngle, lookSmooth);
            transform.rotation = Quaternion.Euler(0f, effectiveAngle, 0f);
        }
    }
    #endregion
}