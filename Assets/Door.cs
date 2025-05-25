using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Door : MonoBehaviour
{
    public string nextRoomID;

    private string doorID;
    public bool canUseDoor = false;
    public bool isEntranceDoor = false;

    private bool strikerInDoor;
    private bool defenderInDoor;

    [SerializeField]
    private TextMeshProUGUI doorCounterText;

    private void Awake()
    {
        if(isEntranceDoor)
            doorCounterText?.gameObject.SetActive(false);

        if (canUseDoor)
            Unlock();
    }

    public void UpdateCounter(int total, int left)
    {
        if(doorCounterText != null)
            doorCounterText.text = $"{left}/{total}";

        if (left <= 0)
            Unlock();
    }

    public void Unlock()
    {
        canUseDoor = true;
        doorCounterText?.gameObject.SetActive(false);
        transform.Find("Counter/lock_0")?.gameObject.SetActive(false);

        GetComponent<Light2D>().enabled = true;
    }

    public void Lock()
    {
        canUseDoor = false;
        doorCounterText?.gameObject.SetActive(true);
        transform.Find("Counter/lock_0").gameObject.SetActive(true);

        GetComponent<Light2D>().enabled = true;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && canUseDoor && collision.name == "Striker")
            strikerInDoor = true;

        if (collision.CompareTag("Player") && canUseDoor && collision.name == "Defender")
            defenderInDoor = true;

        if (strikerInDoor && defenderInDoor && canUseDoor && !isEntranceDoor)
        {
            if(FindAnyObjectByType<Room>().roomID.Contains("1") && !GameManager.Instance.didLevel1variant)
                GameManager.Instance.DidLevel1Variant(true, true);

            RoomManager.Instance.TransitionToNextRoom(nextRoomID);
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.name == "Striker")
            strikerInDoor = false;
        if (collision.CompareTag("Player") && collision.name == "Defender")
            defenderInDoor = false;
    }
}
