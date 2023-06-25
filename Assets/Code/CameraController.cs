using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Transform playerObject;
    [SerializeField] private float rotationSpeed = 3f;
    [SerializeField] private float distanceFromPlayer = 10f;

    [Header("Camera Layers")]
    [SerializeField] private LayerMask ignoreLayers;
    [SerializeField] private LayerMask lockOnLayer;

    [Header("Lock on")]
    [SerializeField] private Transform lockOnTarget;
    [SerializeField] private float detectionRadius = 20f;

    public bool LockOn { get => _lockOnState; }

    public Transform Target { get => lockOnTarget; }

    private float _currentDistanceFromPlayer = 0f;

    private Vector3 _cameraOffset = Vector3.zero;

    private bool _lockOnState = false;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _currentDistanceFromPlayer = distanceFromPlayer;
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            ToggleLockOnState();

        if (_lockOnState)
        {
            LockOnTarget(lockOnTarget);
        }
        else
        {
            RotateTheCamera();
        }
    }

    private void RotateTheCamera()
    {
        float horizontalInput = Input.GetAxis("Mouse X") * rotationSpeed;
        float verticalInput = Input.GetAxis("Mouse Y") * rotationSpeed;

        float targetAngleY = transform.eulerAngles.y + horizontalInput;
        float targetAngleX = transform.eulerAngles.x - verticalInput;

        Quaternion rotation = Quaternion.Euler(targetAngleX, targetAngleY, 0f);

        _currentDistanceFromPlayer = GetFurthestPossiblePosition();
        _cameraOffset = new Vector3(0f, 0f, _currentDistanceFromPlayer);

        transform.position = Vector3.Lerp(transform.position, playerObject.position - (rotation * _cameraOffset), Time.deltaTime * 100f);

        transform.LookAt(playerObject);
    }

    private float GetFurthestPossiblePosition()
    {
        Ray ray = new Ray(playerObject.position, transform.position - playerObject.position);
        if (Physics.Raycast(ray, out RaycastHit hit, distanceFromPlayer, ~ignoreLayers))
        {
            float distance = Vector3.Distance(playerObject.position, hit.point);
            distance = Mathf.Clamp(distance, 0f, distanceFromPlayer);

            float smoothedDistance = Mathf.Lerp(_currentDistanceFromPlayer, distance, Time.deltaTime * 10f);

            return smoothedDistance;
        }

        return distanceFromPlayer;
    }

    private void ToggleLockOnState()
    {
        _lockOnState = !_lockOnState;

        if (!_lockOnState) return;

        lockOnTarget = GetClosestEnemyInRadius();

        if (lockOnTarget != null)
        {
            LockOnTarget(lockOnTarget);
        }
        else
        {
            HandleChangeLockOnState(false);
        }
    }

    public void HandleChangeLockOnState(bool value)
    {
        _lockOnState = value;
    }

    private Transform GetClosestEnemyInRadius()
    {
        Collider[] enemiesInRadius = Physics.OverlapSphere(playerObject.position, detectionRadius, lockOnLayer);
        Transform closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider enemyCollider in enemiesInRadius)
        {
            Transform enemy = enemyCollider.transform;

            if (!IsVisible(enemy))
            {
                Debug.Log(enemy.name + " is not visible!");
                continue;
            }
            if (!IsNotBlocked(enemy))
            {
                Debug.Log(enemy.name + " is blocked by an object!");
                continue;
            }

            float distanceToEnemy = Vector3.Distance(playerObject.position, enemy.position);
            if (distanceToEnemy < closestDistance)
            {
                closestDistance = distanceToEnemy;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }

    private bool IsVisible(Transform targetObject)
    {
        Vector3 viewportPosition = GetComponent<Camera>().WorldToViewportPoint(targetObject.transform.position);

        return viewportPosition.x >= 0 && viewportPosition.x <= 1 &&
               viewportPosition.y >= 0 && viewportPosition.y <= 1 &&
               viewportPosition.z > 0;
    }

    private bool IsNotBlocked(Transform targetObject)
    {
        Vector3 directionToTarget = (targetObject.transform.position - transform.position).normalized;

        Ray origin = new Ray(transform.position, directionToTarget);
        //Debug.DrawRay(origin.origin, origin.direction * 100, Color.red, 1f);

        RaycastHit hit;

        if (Physics.Raycast(origin, out hit, Mathf.Infinity, ~ignoreLayers))
        {
            //Debug.Log(hit.collider.name);
            return hit.collider.gameObject.transform == targetObject;   
        }

        return true;
    }

    private void LockOnTarget(Transform target)
    {
        Vector3 playerToTargetDirection = (target.position - playerObject.position).normalized;
        Vector3 cameraTargetPosition = playerObject.position + Vector3.up - playerToTargetDirection * _currentDistanceFromPlayer;

        if (Physics.Linecast(target.position, cameraTargetPosition, out RaycastHit hit, ~ignoreLayers))
        {
            cameraTargetPosition = hit.point + (cameraTargetPosition - target.position).normalized * 0.5f;
        }

        transform.position = Vector3.Lerp(transform.position, cameraTargetPosition, Time.deltaTime * 10f);
        transform.LookAt(target.position);
    }
}