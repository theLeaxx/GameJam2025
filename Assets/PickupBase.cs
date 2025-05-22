using UnityEngine;

public class PickupBase : MonoBehaviour
{
    public bool DefenderOnly = false;
    public bool StrikerOnly = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (DefenderOnly && other.name == "Defender")
            ActionAndDestroy();
        else if (StrikerOnly && other.name == "Striker")
            ActionAndDestroy();
        else if (!DefenderOnly && !StrikerOnly && other.CompareTag("Player"))
            ActionAndDestroy();
    }

    private void ActionAndDestroy()
    {
        Action();
        Destroy(gameObject);
    }

    public virtual void Action()
    {

    }
}
