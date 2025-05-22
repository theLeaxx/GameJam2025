using TMPro;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField]
    private string nextRoomID;

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
            doorCounterText.gameObject.SetActive(false);
    }

    public void UpdateCounter(int total, int left)
    {
        doorCounterText.text = $"{left}/{total}";

        if (left <= 0)
            Unlock();
    }

    public void Unlock()
    {
        canUseDoor = true;
        doorCounterText.gameObject.SetActive(false);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && canUseDoor && collision.name == "Striker")
            strikerInDoor = true;

        if (collision.CompareTag("Player") && canUseDoor && collision.name == "Defender")
            defenderInDoor = true;

        if (strikerInDoor && defenderInDoor && canUseDoor && !isEntranceDoor)
            RoomManager.Instance.TransitionToNextRoom(nextRoomID);
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.name == "Striker")
            strikerInDoor = false;
        if (collision.CompareTag("Player") && collision.name == "Defender")
            defenderInDoor = false;
    }
}
