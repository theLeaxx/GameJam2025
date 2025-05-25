using UnityEngine;

public class EnergyPickup : PickupBase
{
    public override void Action()
    {
        GameManager.Instance.IncreaseEnergy(Random.Range(50, 120));
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        DefenderOnly = true;
    }
}
