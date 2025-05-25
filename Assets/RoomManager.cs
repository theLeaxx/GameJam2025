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

    [SerializeField]
    private GameObject fade;

    private List<Room> spawnedRooms = new List<Room>();

    public string currentRoomID;
    public string lastRoomID = "0";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
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

        if (Input.GetKeyDown(KeyCode.Alpha3))
            FindAnyObjectByType<Room>().SpawnEnergyPickup();

        if (Input.GetKeyDown(KeyCode.Alpha4))
            StartCoroutine(Fade(1f, 0.5f, 1f));

        if (Input.GetKeyDown(KeyCode.F3))
            TransitionToNextRoom("3A");

        if (Input.GetKeyDown(KeyCode.F12))
            TransitionToNextRoom("Boss1");
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

    public void TransitionToNextRoom(string roomID, bool _loadingData = false)
    {
        if (roomID == currentRoomID)
            return;

        StartCoroutine(TransitionToNextRoomCoroutine(roomID, loadingData: _loadingData));
    }

    public IEnumerator Fade(float timeToFade, float fadeAmount, float timeToFadeOut)
    {
        Debug.Log("Fading in and out");
        fade.SetActive(true);
        var fadeImage = fade.GetComponent<UnityEngine.UI.Image>();

        fadeImage.color = Color.clear;
        float timer = 0f;
        while (timer < timeToFade)
        {
            timer += Time.deltaTime;
            fadeImage.color = Color.Lerp(Color.clear, Color.black, timer / timeToFade);
            yield return null;
        }

        fadeImage.color = Color.black;

        yield return new WaitForSeconds(fadeAmount);

        timer = 0f;
        while (timer < timeToFadeOut)
        {
            timer += Time.deltaTime;
            fadeImage.color = Color.Lerp(Color.black, Color.clear, timer / timeToFadeOut);
            yield return null;
        }

        fadeImage.color = Color.clear;
        fade.SetActive(false);
    }

    private IEnumerator TransitionToNextRoomCoroutine(string roomID, string last = "", bool loadingData = false)
    {
        if(loadingData)
            StartCoroutine(Fade(0f, 1.5f, 1f));
        else
            StartCoroutine(Fade(1f, 0.5f, 1f));

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
