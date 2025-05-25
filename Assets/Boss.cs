using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Boss : BasicEnemy
{
    [SerializeField]
    private string bossName;

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

    public int percentageOfDamageApplied = 100;

    [SerializeField]
    private BossState bossState = BossState.Idle;

    private float maxHealth;

    public bool IsRaging()
    {
        return isRaging;
    }

    public void InitializeBoss()
    {
        maxHealth = health;
        GameManager.Instance.bossBar.value = health / 10;
        GameManager.Instance.bossBarText.text = bossName;
        GameManager.Instance.bossBar.gameObject.SetActive(true);
    }

    public void StartBoss()
    {
        bossState = BossState.Move;
        StartCoroutine(BossLogic());
        AudioManager.Instance.BossTime();
    }

    public void UpdateBoss()
    {
        if (bossState == BossState.Move)
            SetDestination();

        GameManager.Instance.bossBar.value = health / 10;
    }

    public void OnDestroyBoss()
    {
        if(GameManager.Instance.bossBar != null)
            GameManager.Instance.bossBar.gameObject.SetActive(false);

        FindAnyObjectByType<Room>().UnlockDoor();
    }

    private IEnumerator BossLogic()
    {
        while (true)
        {
            if (bossState == BossState.Move)
            {
                agent.isStopped = false;
                MoveState();
                yield return new WaitForSeconds(Random.Range(minMove, maxMove));
                bossState = BossState.Attack;

                if (Random.Range(0, 2) == 0)
                    SwitchTarget();
            }
            else if (bossState == BossState.Idle)
            {
                agent.isStopped = true;
                IdleState();
                yield return new WaitForSeconds(Random.Range(minIdle, maxIdle));
                agent.isStopped = false;
                bossState = BossState.Move;
            }
            else if (bossState == BossState.Attack)
            {
                agent.isStopped = true;
                AttackState();
                yield return new WaitForSeconds(Random.Range(minAttack, maxAttack));
                agent.isStopped = false;

                if (Random.Range(0, 2) == 0)
                    bossState = BossState.Idle;
                else
                    bossState = BossState.Move;
            }

            if (health <= maxHealth / 3 && !isRaging)
                Rage();
        }
    }

    public override void OnDamageActions(float dmg)
    {
        base.OnDamageActions(dmg);

        health += dmg * percentageOfDamageApplied / 100;
    }

    public new void Heal(float dmg)
    {
        health += dmg;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        GameManager.Instance.bossBar.value = health / 10;
    }

    public virtual void AttackState() { }
    public virtual void MoveState() { }
    public virtual void IdleState() { }
    public virtual void RageState() { }

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

        RageState();
    }

    public void Attack(PlayerScriptBase player, Collider2D collision)
    {
        if (player != null && canAttack)
        {
            player.TakeDamage(Random.Range(minDmg, maxDmg));
            StartCoroutine(AttackCooldown());
        }
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(1f);
        canAttack = true;
    }

    public void AttackOnTrigger(Collider2D collision)
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