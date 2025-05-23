using UnityEngine;
using UnityEngine.UI;

public class PlayerScriptBase : MonoBehaviour
{
    public virtual string HorizontalAxis { get; set; }
    public virtual string VerticalAxis { get; set; }
    public float _moveSpeed;

    [HideInInspector]
    public Transform forwardPlayer { get; private set; }
    public Rigidbody2D rb { get; private set; }

    public Vector2 movement { get; private set; }

    public bool isDashing = false;
    public float health { get; private set; } = 100f;

    private Image healthUI;
    private Vector2 forcedMovement;

    public void TakeDamage(float dmg)
    {
        health -= dmg;
        if (health <= 0)
        {
            health = 0f;
            Debug.Log($"{gameObject.name} is dead");
            Die();
        }

        healthUI.fillAmount = health / 100f;
    }

    private void Die()
    {
        GameManager.Instance.Restart();
    }

    public void SetHealth(float value)
    {
        health = value;
        if (health > 100f)
        {
            health = 100f;
        }
        else if (health < 0f)
        {
            health = 0f;
        }
        healthUI.fillAmount = health / 100f;
    }

    public void Heal(float heal)
    {
        health += heal;
        if (health > 100f)
        {
            health = 100f;
        }

        healthUI.fillAmount = health / 100f;
    }

    public void TransformEnergyInHealth()
    {
        if (GameManager.Instance.EnergyLevel >= 70 && health <= 90)
        {
            GameManager.Instance.DecreaseEnergy(70f);
            Heal(10f);
        }
    }

    public void TransformHealthInEnergy()
    {
        if (health > 10 && GameManager.Instance.EnergyLevel <= 950)
        {
            TakeDamage(10f);
            GameManager.Instance.IncreaseEnergy(50f);
        }
    }

    public void SetVariables()
    {
        rb = GetComponent<Rigidbody2D>();
        forwardPlayer = transform.Find("Forward");
        healthUI = transform.Find("Health/Color").GetComponent<Image>();
    }

    public void ForceAddMovement(Vector2 movement)
    {
        if(forcedMovement != Vector2.zero)
            forcedMovement += movement;
        else
            forcedMovement = movement;
    }

    public void RemoveForcedMovement()
    {
        forcedMovement = Vector2.zero;
    }

    public void MoveLogic()
    {
        float moveHorizontal = Input.GetAxis(HorizontalAxis);
        float moveVertical = Input.GetAxis(VerticalAxis);

        movement = new Vector2(moveHorizontal, moveVertical);

        if (forcedMovement != Vector2.zero)
            movement += forcedMovement;

        var toApply = movement * _moveSpeed;

        if (isDashing)
            toApply *= 3f;

        rb.linearVelocity = toApply;
    }
}
