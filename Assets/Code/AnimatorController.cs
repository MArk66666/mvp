using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorController : MonoBehaviour, IAnimatable
{
    private Animator _animator;

    private float _currentHorizontal = 0f;
    private float _currentVertical = 0f;
    private float _interpolationSpeed = 5f;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void PlayMoveAnimation(float speed)
    {
        _animator.SetFloat("Speed", speed);    
    }

    public void PlayJumpAnimation()
    {
    }

    public void PlayStrafeAnimation(Vector3 movementDirection)
    {
        _currentHorizontal = Mathf.Lerp(_currentHorizontal, movementDirection.x, Time.deltaTime * _interpolationSpeed);
        _currentVertical = Mathf.Lerp(_currentVertical, movementDirection.z, Time.deltaTime * _interpolationSpeed);

        _animator.SetFloat("Horizontal", _currentHorizontal);
        _animator.SetFloat("Vertical", _currentVertical);
    }


    public void SetStrafeState(bool value)
    {
        _animator.SetBool("Strafing", value);
    }
}
