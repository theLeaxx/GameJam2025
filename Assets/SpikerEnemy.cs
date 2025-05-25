using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class SpikerEnemy : BasicEnemy
{
    [SerializeField]
    private GameObject spikesLinePrefab;

    private Vector3 newTarget;
    private int retriedCount = 0;

    [SerializeField]
    private float timeBetweenSpikes = 8f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();
        StartCoroutine(SpikerAttacks());
        agent.isStopped = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (newTarget != null)
            agent.SetDestination(newTarget);
    }

    private void PickNewWanderTarget()
    {
        Vector2 randomDirection = Random.insideUnitCircle * 25f;
        Vector2 newTargetCandidate = new Vector2(transform.position.x, transform.position.y) + randomDirection;

        if (agent != null && agent.isOnNavMesh)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(newTargetCandidate, out hit, 25 * 2, NavMesh.AllAreas))
            {
                newTarget = hit.position;
                agent.SetDestination(newTarget);
            }
            else
            {
                if(retriedCount >= 10)
                {
                    retriedCount = 0;
                    return;
                }

                retriedCount++;
                PickNewWanderTarget();
            }
        }
    }

    private IEnumerator SpikerAttacks()
    {
        while (true)
        {
            PickNewWanderTarget();

            yield return new WaitForSeconds(timeBetweenSpikes);

            SpawnSpikesInFront();

            yield return new WaitForSeconds(2f);
        }
    }

    private void SpawnSpikesInFront()
    {
        if (spikesLinePrefab != null)
        {
            int randomChance = Random.Range(0, 100);

            Vector3 spawnPosition;

            if (randomChance > 50)
                spawnPosition = transform.position + transform.right;
            else
                spawnPosition = transform.position - transform.right;

            spawnPosition *= 2f;
            GameObject spikesLine = Instantiate(spikesLinePrefab, spawnPosition, Quaternion.identity);
            spikesLine.transform.rotation = Quaternion.LookRotation(transform.forward);
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);
        return navHit.position;
    }
}
