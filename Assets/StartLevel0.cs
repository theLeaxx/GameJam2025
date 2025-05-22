using Mono.Cecil.Cil;
using TMPro;
using UnityEngine;

public class StartLevel0 : MonoBehaviour
{
    private bool strikerInArea;
    private bool defenderInArea;

    [SerializeField]
    private TextMeshPro counter;

    private bool gameStarted = false;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(gameStarted)
            return;

        if (collision.CompareTag("Player") && collision.name == "Striker")
            strikerInArea = true;

        if (collision.CompareTag("Player") && collision.name == "Defender")
            defenderInArea = true;

        UpdateCounter();

        if (strikerInArea && defenderInArea)
        {
            FindAnyObjectByType<Room>().StartEnemies(2, 8);
            counter.gameObject.SetActive(false);
            gameStarted = true;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (gameStarted)
            return;

        if (collision.CompareTag("Player") && collision.name == "Striker")
            strikerInArea = false;
        if (collision.CompareTag("Player") && collision.name == "Defender")
            defenderInArea = false;

        UpdateCounter();
    }

    private void UpdateCounter()
    {
        if (!strikerInArea && !defenderInArea)
            counter.text = "0 / 2 players";

        if ((strikerInArea && !defenderInArea) || (!strikerInArea && defenderInArea))
            counter.text = "1 / 2 players";

        if (strikerInArea && defenderInArea)
            counter.text = "2 / 2 players";
    }
}
