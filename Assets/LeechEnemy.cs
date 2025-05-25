using System.Collections;
using UnityEngine;

public class LeechEnemy : BasicEnemy
{
    [SerializeField]
    private float distanceRequired;

    [SerializeField]
    private float energyLeechAmount;

    [SerializeField]
    private float energyLeechTime;

    private bool leechingEnergy;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToStriker = Vector3.Distance(transform.position, GameManager.Instance.Striker.transform.position);
        float distanceToDefender = Vector3.Distance(transform.position, GameManager.Instance.Defender.transform.position);

        if (distanceToStriker < distanceToDefender)
            ChangeTarget(GameManager.Instance.Striker);
        else
            ChangeTarget(GameManager.Instance.Defender);

        SetDestination();

        if (Vector3.Distance(transform.position, target.transform.position) < distanceRequired && !leechingEnergy)
        {
            leechingEnergy = true;
            StartCoroutine(LeechEnergy());
        }
        else if (Vector3.Distance(transform.position, target.transform.position) >= distanceRequired)
        {
            leechingEnergy = false;
            StopCoroutine(LeechEnergy());
        }
    }

    private IEnumerator LeechEnergy()
    {
        while (leechingEnergy)
        {
            GameManager.Instance.DecreaseEnergy(energyLeechAmount);
            yield return new WaitForSeconds(energyLeechTime);
        }
    }
}
