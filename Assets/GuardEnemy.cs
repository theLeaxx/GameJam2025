using System.Collections;
using UnityEngine;

public class GuardEnemy : BasicEnemy
{
    [SerializeField]
    private float minDmg;
    [SerializeField]
    private float maxDmg;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();
    }

    private void Update()
    {
        SetDestination();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && gameObject.name.Contains("Guard"))
        {
            PlayerScriptBase player = collision.GetComponent<PlayerScriptBase>();
            Attack(player, collision);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log($"Collision with {collision.gameObject.name}");
        if (collision.CompareTag("Player") && gameObject.name.Contains("Guard"))
        {
            PlayerScriptBase player = collision.GetComponent<PlayerScriptBase>();
            Attack(player, collision);
        }
        Debug.Log($"Collision ended with {collision.gameObject.name}");
    }

    public void Attack(PlayerScriptBase player, Collider2D collision)
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
