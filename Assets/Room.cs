using UnityEngine;
using System.Collections;
using Unity.Burst;
using UnityEditor.Build.Reporting;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

public class Room : MonoBehaviour
{
    public string roomID;
    public string roomName;
    public int totalEnemies;
    public int enemiesLeft;

    public Door[] doors;
    public List<BasicEnemy> enemies = new List<BasicEnemy>();
    public List<BasicEnemy> enemiesToSpawn = new List<BasicEnemy>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemies = transform.Find("Enemies").GetComponentsInChildren<BasicEnemy>(true).ToList();

        totalEnemies = enemies.Count;
        enemiesLeft = totalEnemies;

        enemiesToSpawn = enemies.Where(enemy => enemy.gameObject.activeSelf == false).ToList();

        UpdateDoors();
    }

    public void StartEnemies(float minDelay, float maxDelay)
    {
        StartCoroutine(SpawnEnemies(minDelay, maxDelay)); 
    }
    
    public void EnableEnemies()
    {
        transform.Find("Enemies").gameObject.SetActive(true);
    }

    private IEnumerator SpawnEnemies(float minDelay, float maxDelay)
    { 
        while(enemiesToSpawn.Count > 0)
        {
            int randomIndex = Random.Range(0, enemiesToSpawn.Count);
            Debug.Log($"Random index: {randomIndex}");

            if (randomIndex >= enemiesToSpawn.Count)
                randomIndex = enemiesToSpawn.Count - 1;

            if (enemiesToSpawn[randomIndex] == null)
            {
                enemiesToSpawn.RemoveAt(randomIndex);
                yield return new WaitForEndOfFrame();
                continue;
            }

            BasicEnemy enemy = enemiesToSpawn[randomIndex];

            if (enemy == null)
            {
                enemiesToSpawn.RemoveAt(randomIndex);
                yield return new WaitForEndOfFrame();
                continue;
            }

            enemy.gameObject.SetActive(true);
            enemiesToSpawn.RemoveAt(randomIndex);

            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
            yield return new WaitForEndOfFrame();
        }
    }

    public void EnemyDeath()
    {
        enemiesLeft--;

        UpdateDoors();
    }

    public void UnlockDoor()
    {
        foreach (Door door in doors)
        {
            if (door.isEntranceDoor)
                continue;

            door.canUseDoor = true;
        }
    }

    public void UpdateDoors()
    {
        foreach (Door door in doors)
        {
            door.UpdateCounter(totalEnemies, enemiesLeft);
        }
    }
}
