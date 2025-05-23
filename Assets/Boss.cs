using Mono.Cecil.Cil;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Boss : BasicEnemy
{
    [SerializeField]
    private float minDmg;
    [SerializeField]
    private float maxDmg;

    [SerializeField]
    private float minMove = 5f;
    [SerializeField]
    private float maxMove = 10f;

    [SerializeField]
    private float minIdle = 3f;
    [SerializeField]
    private float maxIdle = 7f;

    [SerializeField]
    private float minAttack = 2f;
    [SerializeField]
    private float maxAttack = 5f;

    [SerializeField]
    private bool isRaging = false;

    [SerializeField]
    private BossState bossState = BossState.Idle;

    [SerializeField]
    private Slider healthBar;

    private float maxHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();
        maxHealth = health;
    }

    public void StartBoss()
    {
        bossState = BossState.Move;
        StartCoroutine(BossLogic());
    }

    // Update is called once per frame
    void Update()
    {
        if(bossState == BossState.Move)
            SetDestination();

        healthBar.value = health / 10;
    }

    private void OnDestroy()
    {
        healthBar.gameObject.SetActive(false);
    }

    IEnumerator BossLogic()
    {
        while (true)
        {
            if (bossState == BossState.Move)
            {
                Debug.Log("MOVING!!");
                agent.isStopped = false;
                yield return new WaitForSeconds(Random.Range(minMove, maxMove));
                bossState = BossState.Attack;

                if (Random.Range(0, 2) == 0)
                    SwitchTarget();

                Debug.Log("MOVING OVER!!");
            }
            else if (bossState == BossState.Idle)
            {
                Debug.Log("IDLE!!");
                agent.isStopped = true;
                yield return new WaitForSeconds(Random.Range(minIdle, maxIdle));
                agent.isStopped = false;
                bossState = BossState.Move;
                Debug.Log("IDLE OVER!!");
            }
            else if (bossState == BossState.Attack)
            {
                agent.isStopped = true;
                Debug.Log("ATTACKING!!");
                yield return new WaitForSeconds(Random.Range(minAttack, maxAttack));
                agent.isStopped = false;

                if (Random.Range(0, 2) == 0)
                    bossState = BossState.Idle;
                else
                    bossState = BossState.Move;
                Debug.Log("ATTACK OVER!!");
            }

            if (health <= maxHealth / 3 && !isRaging)
                Rage();
        }
    }

    private void Rage()
    {
        isRaging = true;
        agent.stoppingDistance = 1.35f;
        agent.speed += 0.75f;
        maxAttack += maxAttack / 2;
        minAttack += minAttack / 2;
        minDmg += minDmg / 2;
        maxDmg += maxDmg / 2;
        minIdle -= minIdle / 2;
        maxIdle -= maxIdle / 2;
        Debug.Log("RAGING!!");
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isRaging || bossState != BossState.Move)
            return;

        if (collision.CompareTag("Player") && gameObject.name.Contains("Boss"))
        {
            PlayerScriptBase player = collision.GetComponent<PlayerScriptBase>();
            Attack(player, collision);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!isRaging || bossState != BossState.Move)
            return;

        if (collision.CompareTag("Player") && gameObject.name.Contains("Boss"))
        {
            PlayerScriptBase player = collision.GetComponent<PlayerScriptBase>();
            Attack(player, collision);
        }
    }
}

public enum BossState
{
    Idle,
    Move,
    Attack,
    Dead
}