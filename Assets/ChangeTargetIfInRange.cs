using UnityEngine;

public class ChangeTargetIfInRange : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other == GameManager.Instance.Defender || other == GameManager.Instance.Striker)
        {
            var script = GetComponentInParent<BasicEnemy>();
            script.ChangeTarget(other.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other == GameManager.Instance.Defender || other == GameManager.Instance.Striker)
        {
            var script = GetComponentInParent<BasicEnemy>();
            script.ChangeTarget();
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other == GameManager.Instance.Defender || other == GameManager.Instance.Striker)
        {
            var script = GetComponentInParent<BasicEnemy>();
            script.ChangeTarget(other.gameObject);
        }
    }
}
