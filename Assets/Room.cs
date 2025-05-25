using UnityEngine;
using System.Collections;
using Unity.Burst;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem.UI;

public class Room : MonoBehaviour
{
    public string roomID;
    public string roomName;
    public int totalEnemies;
    public int enemiesLeft;

    public Door[] doors;
    public List<BasicEnemy> enemies = new List<BasicEnemy>();
    public List<BasicEnemy> enemiesToSpawn = new List<BasicEnemy>();
    private List<Transform> energyPickupLocs = new List<Transform>();

    [SerializeField]
    private float timeMinBetweenEnemies = 2f;
    [SerializeField]
    private float timeMaxBetweenEnemies = 8f;

    [SerializeField]
    private bool isBossBattle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (roomID.Contains("Corridor"))
        {
            if ((GameManager.Instance.didLevel1variant && roomID.Contains("0-1AB-2")))
            {
                GetDoorNextLevel().Unlock();
                GetDoorVariantA().Lock();
                GetDoorVariantB().Lock();
            }
            else if((!GameManager.Instance.didLevel1variant && roomID.Contains("0-1AB-2")))
            {
                GetDoorNextLevel().Lock();
                GetDoorVariantA().Unlock();
                GetDoorVariantB().Unlock();
            }

            return;
        }

        enemies = transform.Find("Enemies")?.GetComponentsInChildren<BasicEnemy>(true).ToList();

        totalEnemies = enemies == null ? 0 : enemies.Count;
        enemiesLeft = totalEnemies;

        enemiesToSpawn = enemies?.Where(enemy => enemy.gameObject.activeSelf == false).ToList();

        energyPickupLocs = transform.Find("EnergyPickupLocs")?.GetComponentsInChildren<Transform>(true).ToList();

        UpdateDoors();
    }

    public void StartEnemies(float minDelay, float maxDelay)
    {
        StartCoroutine(SpawnEnemies(minDelay, maxDelay));
        StartCoroutine(SpawnEnergyPickups());
    }
    
    public void EnableEnemies()
    {
        transform.Find("Enemies")?.gameObject.SetActive(true);
        StartEnemies(timeMinBetweenEnemies, timeMaxBetweenEnemies);
    }

    public void SpawnEnergyPickup(Transform transform = null)
    {
        if (transform == null)
            transform = energyPickupLocs[0];

        Instantiate(GameManager.Instance.energyPickupPrefab, transform.position, Quaternion.identity);
    }

    private IEnumerator SpawnEnergyPickups()
    {
        if(energyPickupLocs == null || energyPickupLocs.Count == 0)
        {
            Debug.LogWarning("No energy pickup locations found in the room.");
            yield break;
        }

        if(isBossBattle)
        {
            yield return new WaitForSeconds(Random.Range(5, 10));
            SpawnEnergyPickup();
            yield break;
        }

        yield return new WaitForSeconds(Random.Range(12, 25));

        int chanceToSpawn = Random.Range(0, 100);

        if (chanceToSpawn > 55)
        {
            int randomIndex = Random.Range(0, energyPickupLocs.Count);
            Transform randomTransform = energyPickupLocs[randomIndex];
            SpawnEnergyPickup(randomTransform);
        }

        yield return new WaitForEndOfFrame();
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

    private Door GetDoorVariantA()
    {
        foreach (Door door in doors)
        {
            if (door.name.Contains("VariantA"))
                return door;
        }
        return null;
    }

    private Door GetDoorVariantB()
    {
        foreach (Door door in doors)
        {
            if (door.name.Contains("VariantB"))
                return door;
        }
        return null;
    }

    private Door GetDoorNextLevel()
    {
        foreach (Door door in doors)
        {
            if (door.name.Contains("NextLevel"))
                return door;
        }
        return null;
    }

    private Door GetEntranceDoor()
    {
        foreach (Door door in doors)
        {
            if (door.isEntranceDoor)
                return door;
        }

        return null;
    }

    public void UpdateDoors()
    {
        foreach (Door door in doors)
        {
            door.UpdateCounter(totalEnemies, enemiesLeft);
        }
    }
}
