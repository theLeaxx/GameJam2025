using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }

    [SerializeField]
    private RoomDictionary[] roomDictionary;

    private List<Room> spawnedRooms = new List<Room>();

    public string currentRoomID;
    public string lastRoomID = "0";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        var currentRoom = FindFirstObjectByType<Room>();
        spawnedRooms.Add(currentRoom);
        currentRoomID = currentRoom.roomID;
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Alpha2))
            ResetPlayersPositions();
#endif
    }

    private void LoadRoom(string roomID)
    {
        if(roomID == currentRoomID)
            return;

        foreach (RoomDictionary room in roomDictionary)
        {
            if (room.roomID.ToString() == roomID)
            {
                GameObject roomPrefab = Instantiate(room.roomPrefab);
                roomPrefab.transform.position = new Vector3(5, -12.5f, 0);
                roomPrefab.transform.rotation = Quaternion.identity;

                spawnedRooms.Add(roomPrefab.GetComponent<Room>());

                break;
            }
        }

        foreach(var bullet in FindObjectsByType<Bullet>(FindObjectsSortMode.None))
            Destroy(bullet.gameObject);
    }

    private void UnloadRoom(string roomID)
    {
        foreach (RoomDictionary room in roomDictionary)
        {
            if (room.roomID.ToString() == roomID)
            {
                Room roomToUnload = spawnedRooms.Find(r => r.roomID == room.roomID);
                if (roomToUnload != null)
                {
                    spawnedRooms.Remove(roomToUnload);
                    Destroy(roomToUnload.gameObject);
                }
                break;
            }
        }
    }

    public void TransitionToNextRoom(string roomID)
    {
        if (roomID == currentRoomID)
            return;

        StartCoroutine(TransitionToNextRoomCoroutine(roomID));
    }

    private bool Fade(float timeToFade, float fadeAmount, float timeToFadeOut)
    {
        return true;
    }

    private IEnumerator TransitionToNextRoomCoroutine(string roomID, string last = "")
    {
        Fade(1f, 0.5f, 1f);
        yield return new WaitForSeconds(1f);

        LoadRoom(roomID);
        UnloadRoom(currentRoomID);

        lastRoomID = currentRoomID;

        if(!string.IsNullOrEmpty(last))
            lastRoomID = last;

        currentRoomID = roomID;
        ResetPlayersPositions();

        yield return new WaitForEndOfFrame();
        GameManager.Instance.UpdateCurrentNavMesh();
        SaveLoad.Instance.SaveGame();

        yield return new WaitForEndOfFrame();
        Room room = spawnedRooms.Find(r => r.roomID == roomID);
        room?.EnableEnemies();

        yield return null;
    }

    public void LoadLevel0ForNewGame()
    {
        StartCoroutine(LoadLevel0());
    }

    private IEnumerator LoadLevel0()
    {
        LoadRoom("0");
        currentRoomID = "0";

        GameManager.Instance.UpdateCurrentNavMesh();
        SaveLoad.Instance.SaveGame();

        yield return new WaitForEndOfFrame();
        Room room = spawnedRooms.Find(r => r.roomID == "0");
        room.EnableEnemies();
    }

    private void ResetPlayersPositions()
    {
        Debug.Log("Resetting players positions");

        foreach (Room room in spawnedRooms)
        {
            if (room.roomID == currentRoomID)
            {
                foreach (Door door in room.doors)
                {
                    if (door.isEntranceDoor)
                    {
                        GameManager.Instance.Striker.transform.position = door.transform.position + new Vector3(5, 1, 0);
                        GameManager.Instance.Defender.transform.position = door.transform.position + new Vector3(5, -1, 0);

                        break;
                    }
                }
            }
        }
    }
}

[Serializable]
public class RoomDictionary
{
    public string roomID;
    public GameObject roomPrefab;
}
