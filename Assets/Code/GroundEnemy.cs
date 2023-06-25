using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class GroundEnemy : Entity
{
    [SerializeField] private int health = 500;
    [SerializeField] private float movementSpeed = 3.755f;
    [SerializeField] private float visionRadius = 10f;
    [SerializeField] private float visionAngle = 140f;

    private IBehaviourState _currentState;

    private Transform _player;

    private void Awake()
    {
        SetupBrain();
    }

    private void Start()
    {
        _currentState = new IdleState();
        _currentState.EnterState(this);

        _player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        _currentState.UpdateState();

        if (PlayerInVisibleArea())
        {
            ChangeState(new ChasingState(_player));
        }
        else
        {
            ChangeState(new IdleState());
        }
    }

    public override void MoveTowards(Vector3 destination)
    {
        Brain.SetDestination(destination);
    }

    private bool PlayerInVisibleArea()
    {
        Vector3 directionToPlayer = transform.position - _player.position;
        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);

        if (distanceToPlayer <= visionRadius)
        {
            float angle = Vector3.Angle(-transform.forward, directionToPlayer);
            if (angle <= visionAngle / 2f)
            {
                LookAtPlayer(_player);
                return true;
            }
        }

        return false;
    }

    private void ChangeState(IBehaviourState newState)
    {
        _currentState.ExitState();
        _currentState = newState;
        _currentState.EnterState(this);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, visionRadius);

        // Draw the vision cone
        Vector3 visionLine1 = Quaternion.AngleAxis(-visionAngle / 2, Vector3.up) * transform.forward * visionRadius;
        Vector3 visionLine2 = Quaternion.AngleAxis(visionAngle / 2, Vector3.up) * transform.forward * visionRadius;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + visionLine1);
        Gizmos.DrawLine(transform.position, transform.position + visionLine2);
    }
}
