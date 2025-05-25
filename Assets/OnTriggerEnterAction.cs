using UnityEngine;
using UnityEngine.Events;

public class OnTriggerEnterAction : MonoBehaviour
{
    public UnityEvent onTriggerEnterEvent;
    public UnityEvent onTriggerExitEvent;
    public UnityEvent onTriggerStayEvent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            onTriggerEnterEvent.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            onTriggerExitEvent.Invoke();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            onTriggerStayEvent.Invoke();
        }
    }

    public void DestroySelf()
    {
        Destroy(this);
    }

    public void DestroyGameObject()
    {
        Destroy(gameObject);
    }
}

