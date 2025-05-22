using UnityEngine;
using UnityEngine.AI;

public class BasicEnemy : MonoBehaviour
{
    public NavMeshAgent agent;
    public GameObject target;
    public float health = 100f;

    public bool canAttack = true;
    public bool canMove = true;
    private bool dead = false;

    public void Initialize()
    {
        if (canMove)
        {
            agent = GetComponent<NavMeshAgent>();
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            agent.isStopped = true;
        }

        ChangeTarget();
    }

    public void ChangeTarget(GameObject _target = null)
    {
        if(_target != null)
            target = _target;
        else
            target = GameManager.Instance.GetRandomTargetForEnemy();
    }

    public void SetDestination()
    {
        if (target != null)
        {
            if (!agent.isOnNavMesh)
                return;

            if (CanReachTarget())
            {
                agent.isStopped = false;
                agent.SetDestination(target.transform.position);
            }
            else
                agent.isStopped = true;
        }

    }

    public void TakeDamage(float dmg)
    {
        if (dead)
            return;

        health -= dmg;
        if (health <= 0)
        {
            Debug.Log($"{gameObject.name} is dead");
            Die();
        }
    }

    public void Heal(float heal)
    {
        health += heal;
        if (health > 100f)
        {
            health = 100f;
        }
    }

    public void Die()
    {
        dead = true;
        GetComponentInParent<Room>().EnemyDeath();
        canAttack = false;
        canMove = false;

        Destroy(gameObject);
    }

    public bool CanReachTarget()
    {
        if(target == null)
            return false;

        if (gameObject.activeSelf && !agent.isActiveAndEnabled)
            agent.enabled = true;

        NavMeshPath path = new();
        bool isPathValid = agent.CalculatePath(target.transform.position, path);
        if (isPathValid && path.status == NavMeshPathStatus.PathComplete)
            return true;
        else
            return false;
    }
}
