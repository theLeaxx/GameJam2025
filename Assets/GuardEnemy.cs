using System.Collections;
using UnityEngine;

public class GuardEnemy : BasicEnemy
{
    [SerializeField]
    private float minDmg;
    [SerializeField]
    private float maxDmg;

    private bool isPathClear = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();

        //StartCoroutine(DestinationManager());
    }

    private IEnumerator DestinationManager()
    {
        while (true)
        {
            isPathClear = false;

            while (!isPathClear)
            {
                Vector2 direction = (target.transform.position - transform.position).normalized;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 30f, LayerMask.GetMask("Obstacle"));

                if (hit.collider == null || hit == false)
                    isPathClear = true;

                yield return new WaitForSeconds(3f);
            }
        }
    }

    private void Update()
    {
        SetDestination();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerScriptBase player = collision.GetComponent<PlayerScriptBase>();
            Attack(player, collision);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerScriptBase player = collision.GetComponent<PlayerScriptBase>();
            Attack(player, collision);
        }
    }

    private void Attack(PlayerScriptBase player, Collider2D collision)
    {
        if (player != null && canAttack)
        {
            player.TakeDamage(Random.Range(minDmg, maxDmg));
            StartCoroutine(AttackCooldown());
            Debug.Log($"{gameObject.name} hit {collision.gameObject.name}");
        }
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(1f);
        canAttack = true;
    }
}
