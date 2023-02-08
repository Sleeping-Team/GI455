using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    #region Variable

    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private Animator animator;
    [SerializeField] private float speed = 1f;
    
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

    private void MovementSystem()
    {
        Move(new Vector3(_movementValue.x, 0f, _movementValue.y));
        Facing();
    }

    private void Move(Vector3 control)
    {
        transform.position += (control * speed * Time.deltaTime);
    }

    private void Facing()
    {
        Vector3 currentPosition = transform.position;
        Vector3 lookForwardTo = new Vector3(_movementValue.x, 0f, _movementValue.y);
        Vector3 facing = currentPosition += lookForwardTo;
        
        transform.LookAt(facing);
    }

    #endregion
}
