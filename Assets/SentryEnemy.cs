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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();
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
            yield return new WaitForSeconds(Random.Range(1f, 2.5f));

            if (Vector2.Distance(transform.position, target.transform.position) > 30f)
            {
                StartCoroutine(RandomMove(false));
                yield return new WaitForSeconds(2f);
                continue;
            }

            bool isPathClear = true;

            while (!isPathClear)
            {
                Vector2 direction = (target.transform.position - transform.position).normalized;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 30f, LayerMask.GetMask("Obstacle"));

                if (hit.collider == null || hit == false)
                    isPathClear = true;

                if(!isPathClear)
                    StartCoroutine(RandomMove(transform));

                yield return new WaitForSeconds(3f);
            }

            ShowFutureShot();

            yield return new WaitForSeconds(Random.Range(1.5f, 2.5f));
            Shoot();

            StartCoroutine(RandomMove(true));
        }
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
