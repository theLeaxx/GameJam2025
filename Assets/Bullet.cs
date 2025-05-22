using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Rigidbody2D rb;

    public string[] validTags;

    private Transform shootLoc;
    private float dmg;

    public void Shoot(Transform position, Transform rotation, float speed, float damage)
    {
        rb = GetComponent<Rigidbody2D>();
        rb.transform.position = position.position;
        rb.transform.rotation = rotation.rotation;

        shootLoc = position;
        dmg = damage;

        rb.AddForce(rotation.right * speed, ForceMode2D.Impulse);
    }

    public void ShootTarget(Transform position, Vector3 target, float speed, float damage)
    {
        rb = GetComponent<Rigidbody2D>();
        rb.transform.position = position.position;
        rb.transform.rotation = position.rotation;
        shootLoc = position;
        dmg = damage;
        Vector2 direction = (target - position.position).normalized;
        rb.AddForce(direction * speed, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null || other.transform == null || shootLoc == null || shootLoc.transform == null)
        {
            Destroy(gameObject);
            return;
        }

        if (other.tag == shootLoc.tag)
            return;

        foreach (string tag in validTags)
        {
            if (other.CompareTag(tag))
            {
                if(other.gameObject.TryGetComponent(out PlayerScriptBase player))
                    player.TakeDamage(dmg);
                else if (other.gameObject.TryGetComponent(out BasicEnemy enemy))
                    enemy.TakeDamage(dmg);
            }
            else
                Destroy(gameObject);
        }
    }
}
