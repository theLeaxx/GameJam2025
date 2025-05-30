using NavMeshPlus.Components;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

public class DefenderPlayer : PlayerScriptBase
{
    public override string HorizontalAxis => "HorizontalIJKL";
    public override string VerticalAxis => "VerticalIJKL";

    private bool isAnyCoroutineRunning = false;
    private bool canKnockback = true;

    [SerializeField]
    private GameObject forceField;

    [SerializeField]
    private GameObject knockbackCircle;

    private KeyCode knockbackCircleToggle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetVariables();

        knockbackCircleToggle = KeyCode.RightControl;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightShift) && GameManager.Instance.EnergyLevel >= 5f && !PauseManager.Instance.isPaused && !isAnyCoroutineRunning && canKnockback)
        {
            isAnyCoroutineRunning = true;
            StartCoroutine(ShowForceField());
        }

        if ((Input.GetKeyUp(KeyCode.RightShift) || GameManager.Instance.EnergyLevel < 5f) && !PauseManager.Instance.isPaused)
        {
            HideForceField();
        }

        if (Input.GetKeyDown(knockbackCircleToggle) && canKnockback && !PauseManager.Instance.isPaused && GameManager.Instance.EnergyLevel > 15f && !isAnyCoroutineRunning)
        {
            StartCoroutine(PushCircle());
        }

        if (Input.GetKeyDown(KeyCode.LeftAlt) && !PauseManager.Instance.isPaused)
        {
            TransformEnergyInHealth();
        }

        if (Input.GetKeyDown(KeyCode.RightAlt) && !PauseManager.Instance.isPaused)
        {
            TransformHealthInEnergy();
        }

        MoveLogic();
    }

    private IEnumerator ShowForceField()
    {
        forceField.SetActive(true);
        animator.SetBool("ForceField", true);
        animator.SetBool("ForceFieldLastFrame", true);

        while (isAnyCoroutineRunning)
        {
            GameManager.Instance.DecreaseEnergy(5f);

            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 2.3f);

            foreach (Collider2D collider in colliders)
            {
                Rigidbody2D rb = collider.GetComponent<Rigidbody2D>();

                if (rb != null)
                {
                    if (collider.gameObject.TryGetComponent(out BasicEnemy enemy))
                        enemy.TakeDamage(1f);

                    if (collider.gameObject.TryGetComponent(out PlayerScriptBase player))
                        player.TakeDamage(1f);
                }
            }

            yield return new WaitForSeconds(Random.Range(0.3f, 1f));
        }
    }

    private IEnumerator PushCircle()
    {
        animator.SetBool("Explosion", true);
        canKnockback = false;
        GameManager.Instance.DecreaseEnergy(10f);

        knockbackCircle.SetActive(true);
        List<NavMeshAgent> list = new List<NavMeshAgent>();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 2f);
        HashSet<Vector3Int> cellsToDelete = new HashSet<Vector3Int>();

        foreach (Collider2D collider in colliders)
        {
            Tilemap tilemap = collider.GetComponent<Tilemap>();

            if (!collider.gameObject.CompareTag("Player") || collider.gameObject.name == "Striker")
            {
                Rigidbody2D rb = collider.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 direction = (collider.transform.position - transform.position).normalized;
                    rb.AddForce(direction * 5f, ForceMode2D.Impulse);
                    if (collider.TryGetComponent<NavMeshAgent>(out var agent))
                    {
                        agent.enabled = false;
                        list.Add(agent);
                    }

                    if (collider.gameObject.TryGetComponent(out BasicEnemy enemy))
                        enemy.TakeDamage(5f);
                }
            }

            if(tilemap != null && tilemap.name == "Obstacles")
            {
                Vector3Int centerCell = tilemap.WorldToCell(transform.position);

                int minX = centerCell.x - Mathf.CeilToInt(2 / tilemap.cellSize.x) - 1;
                int maxX = centerCell.x + Mathf.CeilToInt(2 / tilemap.cellSize.x) + 1;
                int minY = centerCell.y - Mathf.CeilToInt(2 / tilemap.cellSize.y) - 1;
                int maxY = centerCell.y + Mathf.CeilToInt(2 / tilemap.cellSize.y) + 1;

                for (int x = minX; x <= maxX; x++)
                {
                    for (int y = minY; y <= maxY; y++)
                    {
                        Vector3Int cell = new Vector3Int(x, y, centerCell.z); 

                        Vector3 cellWorldCenter = tilemap.GetCellCenterWorld(cell);

                        if (Vector3.Distance(transform.position, cellWorldCenter) <= 2)
                            if (tilemap.HasTile(cell))
                                cellsToDelete.Add(cell);
                    }
                }

                if(cellsToDelete.Count > 0)
                    foreach (var cell in cellsToDelete)
                        tilemap.SetTile(cell, null);

                GameManager.Instance.UpdateCurrentNavMesh();
            }
        }

        yield return new WaitForSeconds(0.5f);

        foreach (var agent in list)
        {
            if(agent != null)
                agent.enabled = true;
        }

        animator.SetBool("Explosion", false);
        knockbackCircle.SetActive(false);

        yield return new WaitForSeconds(3f);
        canKnockback = true;    
    }

    private void HideForceField()
    {
        isAnyCoroutineRunning = false;
        StopCoroutine(ShowForceField());
        forceField.SetActive(false);
        animator.SetBool("ForceField", false);
    }
}
