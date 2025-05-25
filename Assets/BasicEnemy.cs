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

    public float chanceForDefender = -1;

    public void Initialize()
    {
        if (!GetComponent<NavMeshAgent>())
            return;

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.isStopped = true;

        ChangeTarget();
    }

    public void ChangeTarget(GameObject _target = null)
    {
        if (_target != null)
            target = _target;
        else
        {
            if (chanceForDefender != -1)
                target = GameManager.Instance.GetRandomTargetForEnemy(chanceForDefender);
            else
                target = GameManager.Instance.GetRandomTargetForEnemy();
        }
    }

    public void SwitchTarget()
    {
        if(target == GameManager.Instance.Defender)
            target = GameManager.Instance.Striker;
        else if (target == GameManager.Instance.Striker)
            target = GameManager.Instance.Defender;
        else
            ChangeTarget();
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

        OnDamageActions(dmg);

        health -= dmg;
        if (health <= 0)
        {
            Debug.Log($"{gameObject.name} is dead");
            Die();
        }
    }

    public virtual void OnDamageActions(float dmg) { }

    public void Heal(float heal)
    {
        health += heal;
        if (health > 100f)
        {
            health = 100f;
        }
    }

    public virtual void DieActions() { }

    public void Die()
    {
        dead = true;
        GetComponentInParent<Room>().EnemyDeath();
        canAttack = false;
        canMove = false;

        DieActions();

        Destroy(gameObject);
    }

    public GameObject CloserTarget()
    {
        float distanceToStriker = Vector2.Distance(transform.position, GameManager.Instance.Striker.transform.position);
        float distanceToDefender = Vector2.Distance(transform.position, GameManager.Instance.Defender.transform.position);

        if (distanceToStriker < distanceToDefender)
            return GameManager.Instance.Striker;
        else
            return GameManager.Instance.Defender;
    }

    public bool CanReachTarget()
    {
        if(target == null)
            return false;

        if (gameObject.activeSelf && !agent.isActiveAndEnabled)
            agent.enabled = true;

        if(Vector2.Distance(transform.position, CloserTarget().transform.position) > 15f)
            return false;

        NavMeshPath path = new();
        bool isPathValid = agent.CalculatePath(target.transform.position, path);
        if (isPathValid && path.status == NavMeshPathStatus.PathComplete)
            return true;
        else
            return false;
    }
}
