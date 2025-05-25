using UnityEngine;

public class HealBoss : BasicEnemy
{
    [SerializeField]
    private float healAmount = 10f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();
        StartCoroutine(HealBossIE());
    }

    private System.Collections.IEnumerator HealBossIE()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f); // Heal every 5 seconds
            FindAnyObjectByType<Boss>()?.Heal(healAmount);
        }
    }


}
