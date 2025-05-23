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

    [SerializeField]
    private float timeMinBetweenEnemies = 2f;
    [SerializeField]
    private float timeMaxBetweenEnemies = 8f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (roomID.Contains("Corridor"))
        {
            if (RoomManager.Instance.lastRoomID == GetEntranceDoor().nextRoomID)
            {
                GetDoorNextLevel().canUseDoor = false;
                GetDoorVariantA().canUseDoor = true;
                GetDoorVariantB().canUseDoor = true;
            }
            else if(RoomManager.Instance.lastRoomID == GetDoorVariantA().nextRoomID || RoomManager.Instance.lastRoomID == GetDoorVariantB().nextRoomID)
            {
                GetDoorNextLevel().canUseDoor = true;
                GetDoorVariantA().canUseDoor = false;
                GetDoorVariantB().canUseDoor = false;
            }

            return;
        }

        enemies = transform.Find("Enemies")?.GetComponentsInChildren<BasicEnemy>(true).ToList();

        totalEnemies = enemies == null ? 0 : enemies.Count;
        enemiesLeft = totalEnemies;

        enemiesToSpawn = enemies?.Where(enemy => enemy.gameObject.activeSelf == false).ToList();

        UpdateDoors();
    }

    public void StartEnemies(float minDelay, float maxDelay)
    {
        StartCoroutine(SpawnEnemies(minDelay, maxDelay)); 
    }
    
    public void EnableEnemies()
    {
        transform.Find("Enemies")?.gameObject.SetActive(true);
        StartEnemies(timeMinBetweenEnemies, timeMaxBetweenEnemies);
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
