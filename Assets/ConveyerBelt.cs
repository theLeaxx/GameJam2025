using UnityEngine;

public class ConveyerBelt : MonoBehaviour
{
    [SerializeField]
    private ConveyerBeltDirection direction;

    [SerializeField]
    private float speed = 0.5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player entered the conveyer belt");
            var script = collision.GetComponent<PlayerScriptBase>();
            script.ForceAddMovement(DirectionToVector2());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player exited the conveyer belt");
            var script = collision.GetComponent<PlayerScriptBase>();
            script.RemoveForcedMovement();
        }
    }

    private Vector2 DirectionToVector2()
    {
        switch (direction)
        {
            case ConveyerBeltDirection.Left:
                return Vector2.left * speed;
            case ConveyerBeltDirection.Right:
                return Vector2.right * speed;
            case ConveyerBeltDirection.Up:
                return Vector2.up * speed;
            case ConveyerBeltDirection.Down:
                return Vector2.down * speed;
            default:
                return Vector2.zero;
        }
    }
}

public enum ConveyerBeltDirection
{
    Left,
    Right,
    Up,
    Down
}
