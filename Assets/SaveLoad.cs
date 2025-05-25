using System.Security.Authentication.ExtendedProtection;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoad : MonoBehaviour
{
    public static SaveLoad Instance;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
            return;

        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SaveGame();
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            LoadGame();
        }
        if (Input.GetKeyDown(KeyCode.F7))
        {
            DeleteSave();
        }
#endif
    }

    public void SaveGame()
    {
        Debug.Log("Game Saved");

        PlayerPrefs.SetString("checkpoint", RoomManager.Instance.currentRoomID);
        PlayerPrefs.SetString("lastCheckpoint", RoomManager.Instance.lastRoomID);
        PlayerPrefs.SetFloat("strikerHealth", FindAnyObjectByType<StrikerPlayer>().health);
        PlayerPrefs.SetFloat("defenderHealth", FindAnyObjectByType<DefenderPlayer>().health);
        PlayerPrefs.SetFloat("currentEnergy", GameManager.Instance.EnergyLevel);
        PlayerPrefs.SetFloat("totalUsedEnergy", GameManager.Instance.TotalUsedEnergy);
        PlayerPrefs.SetInt("didLevel1variant", GameManager.Instance.didLevel1variant ? 1 : 0);

        PlayerPrefs.Save();
    }

    public void LoadGame()
    {
        Debug.Log("Game Loaded");

        string checkpoint = PlayerPrefs.GetString("checkpoint");

        if (string.IsNullOrEmpty(checkpoint))
        {
            Debug.Log("No saved game found");
            return;
        }

        string lastCheckpoint = PlayerPrefs.GetString("lastCheckpoint");
        float strikerHealth = PlayerPrefs.GetFloat("strikerHealth");
        float defenderHealth = PlayerPrefs.GetFloat("defenderHealth");
        float currentEnergy = PlayerPrefs.GetFloat("currentEnergy");
        float totalUsedEnergy = PlayerPrefs.GetFloat("totalUsedEnergy");
        bool didLevel1variant = PlayerPrefs.GetInt("didLevel1variant", 0) == 1;

        RoomManager.Instance.TransitionToNextRoom(checkpoint, true);
        RoomManager.Instance.lastRoomID = lastCheckpoint;
        FindAnyObjectByType<StrikerPlayer>().SetHealth(strikerHealth);
        FindAnyObjectByType<DefenderPlayer>().SetHealth(defenderHealth);
        GameManager.Instance.SetEnergy(currentEnergy);
        GameManager.Instance.TotalUsedEnergy = totalUsedEnergy;
        GameManager.Instance.DidLevel1Variant(true, didLevel1variant);
    }

    public void DeleteSave()
    {
        PlayerPrefs.DeleteKey("checkpoint");
        PlayerPrefs.DeleteKey("lastCheckpoint");
        PlayerPrefs.DeleteKey("strikerHealth");
        PlayerPrefs.DeleteKey("defenderHealth");
        PlayerPrefs.DeleteKey("currentEnergy");
        PlayerPrefs.DeleteKey("totalUsedEnergy");
        PlayerPrefs.DeleteKey("didLevel1variant");

        PlayerPrefs.Save();

        Debug.Log("Game Save Deleted");
    }
}
