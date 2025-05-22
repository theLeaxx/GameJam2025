using System.Collections;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class SentryEnemy : BasicEnemy
{
    private Vector3 futureShotPosition;

    [SerializeField]
    private GameObject futureShotPrefab;

    [SerializeField]
    private GameObject bulletPrefab;

    [SerializeField]
    private float minDelayBetweenShots;
    [SerializeField]
    private float maxDelayBetweenShots;

    [SerializeField]
    private float minDelayBetweenWarningAndShot;
    [SerializeField]
    private float maxDelayBetweenWarningAndShot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();
        if(agent != null)
            agent.enabled = false;
        StartCoroutine(SentryLogic());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ShowFutureShot()
    {
        futureShotPosition = target.transform.position;

        var futureShot = Instantiate(futureShotPrefab, futureShotPosition, Quaternion.identity);
        Destroy(futureShot, 5f);
    }

    private void Shoot()
    {
        var bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        var bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.ShootTarget(transform, futureShotPosition, 10f, 15f);
    }

    private IEnumerator SentryLogic()
    {
        while (true)
        {
            if (!CanReachTarget() || !IsPathClear())
            {
                yield return new WaitForEndOfFrame();
                continue;        
            }

            if (!IsPathClear() && canMove)
            {
                StartCoroutine(RandomMove(transform));
                yield return new WaitForSeconds(3f);
            }

            yield return new WaitForSeconds(Random.Range(minDelayBetweenWarningAndShot, maxDelayBetweenWarningAndShot));

            if (Vector2.Distance(transform.position, target.transform.position) > 30f && canMove)
            {
                StartCoroutine(RandomMove(false));
                yield return new WaitForSeconds(2f);
                continue;
            }

            ShowFutureShot();

            yield return new WaitForSeconds(Random.Range(minDelayBetweenShots, maxDelayBetweenShots));
            Shoot();

            if(canMove)
                StartCoroutine(RandomMove(true));
        }
    }

    private bool IsPathClear()
    {
        Vector2 direction = (target.transform.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 30f, LayerMask.GetMask("Obstacle"));

        if (hit.collider == null || hit == false)
            return true;

        return false;
    }

    private IEnumerator RandomMove(bool targetChange)
    {
        var random = Random.Range(0, 1);
        if (random < 0.3f)
        {
            if (targetChange) ChangeTarget();
            agent.enabled = true;
            SetDestination();
            yield return new WaitForSeconds(1);
            agent.enabled = false;
        }
    }
}
