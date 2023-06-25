using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class Entity : MonoBehaviour
{
    public NavMeshAgent Brain { get; protected set; }

    protected void SetupBrain()
    {
        Brain = GetComponent<NavMeshAgent>();
    }

    public virtual void MoveTowards(Vector3 movementDirection)
    {

    }

    protected virtual void Attack(int amount)
    {

    }

    protected virtual void LookAtPlayer(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0;

        Quaternion newRotation = Quaternion.LookRotation(direction, Vector3.up);

        transform.rotation = newRotation;
    }
}