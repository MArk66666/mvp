using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AnimatorController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CameraController playerCamera;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float sprintSpeed = 5.335f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 10f;

    [Header("Sharpness")]
    [SerializeField] private float movementSharpness = 10f;
    [SerializeField] private float rotationSharpness = 10f;

    private float _currentSpeed = 0f;

    private float _gravity = -30f;
    private Vector3 _verticalVelocity = Vector3.zero;

    private CharacterController _controller;
    private AnimatorController _animatorController;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _animatorController = GetComponent<AnimatorController>();
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector3 movementDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));

        Vector3 rightDirection = movementDirection.x * playerCamera.transform.right;
        Vector3 forwardDirection = movementDirection.z * playerCamera.transform.forward;

        Vector3 cameraRelativeMovementDirection = (rightDirection + forwardDirection).normalized;
        cameraRelativeMovementDirection.y = 0;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            _currentSpeed = Mathf.Lerp(_currentSpeed, sprintSpeed, movementSharpness * Time.deltaTime);
            playerCamera.HandleChangeLockOnState(false);
        }
        else if(movementDirection != Vector3.zero)
        {
            _currentSpeed = Mathf.Lerp(_currentSpeed, walkSpeed, movementSharpness * Time.deltaTime);
        }
        else
        {
            _currentSpeed = Mathf.Lerp(_currentSpeed, 0, movementSharpness * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.Space))
            Jump();

        _verticalVelocity.y += _gravity * Time.deltaTime;

        if (movementDirection != Vector3.zero)
        {
            if (!playerCamera.LockOn)
            {
                Rotate(cameraRelativeMovementDirection);
            } 
            else
            {
                Rotate(playerCamera.Target);
            }
        }

        bool strafing = playerCamera.LockOn;
        _animatorController.SetStrafeState(strafing);

        _animatorController.PlayMoveAnimation(_currentSpeed);
        _animatorController.PlayStrafeAnimation(cameraRelativeMovementDirection);

        _controller.Move(cameraRelativeMovementDirection * _currentSpeed * Time.deltaTime +
            _verticalVelocity * Time.deltaTime);
    }

    private void Rotate(Vector3 movementDirection)
    {
        Quaternion targetRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSharpness * Time.deltaTime);
    }

    private void Rotate(Transform target)
    {
        Vector3 lookDirection = (target.position - transform.position).normalized;
        lookDirection.y = 0f;

        Quaternion targetRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSharpness * Time.deltaTime);
    }

    private void Jump()
    {
        if (!_controller.isGrounded) return;

        _verticalVelocity.y = Mathf.Sqrt(jumpForce * -2 * _gravity);
    }
}