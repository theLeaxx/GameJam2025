using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;
using Input = UnityEngine.Input;

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
    public Animator animator;
    private SpriteRenderer spriteRenderer;
    public bool canMove = true;
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
        GameManager.Instance.YouDied(transform.name);
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
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
        if (!canMove)
        {
            movement = Vector2.zero;
            rb.linearVelocity = Vector2.zero;
            animator.SetFloat("Speed", 0f);
            return;
        }

        float moveHorizontal = Input.GetAxis(HorizontalAxis);
        float moveVertical = Input.GetAxis(VerticalAxis);

        movement = new Vector2(moveHorizontal, moveVertical);

        animator.SetFloat("Horizontal", moveHorizontal);
        animator.SetFloat("Vertical", moveVertical);
        animator.SetFloat("Speed", movement.magnitude);

        if (movement.magnitude > 0.01f)
        {
            if (Mathf.Abs(moveHorizontal) > Mathf.Abs(moveVertical)) 
            {
                if(gameObject.name == "Defender")
                {
                    if (moveHorizontal > 0) animator.SetFloat("LastDir", 2); // Right
                    else animator.SetFloat("LastDir", 3); // Left
                }
                else
                {
                    if (moveHorizontal != 0) animator.SetFloat("LastDir", 2); // Right
                }
            }
            else
            {
                if (moveVertical > 0) animator.SetFloat("LastDir", 1); // Up
                else animator.SetFloat("LastDir", 0); // Down
            }

            Debug.Log(animator.GetFloat("LastDir"));
        }

        if (moveHorizontal < 0 && gameObject.name == "Striker")
            spriteRenderer.flipX = true;
        else if (moveHorizontal > 0 && gameObject.name == "Striker")
            spriteRenderer.flipX = false;

        if (forcedMovement != Vector2.zero)
            movement += forcedMovement;

        var toApply = movement * _moveSpeed;

        if (isDashing)
            toApply *= 3f;

        rb.linearVelocity = toApply;

        if (movement.x > 0)
        {
            forwardPlayer.localRotation = Quaternion.Euler(0, 0, 0);
            forwardPlayer.localPosition = new Vector3(0.24f, 0.1f, 0);
        }
        else if (movement.x < 0)
        {
            forwardPlayer.localRotation = Quaternion.Euler(0, 180, 0);
            forwardPlayer.localPosition = new Vector3(-0.24f, 0.1f, 0);
        }
        else if (movement.y > 0)
        {
            forwardPlayer.localRotation = Quaternion.Euler(0, 0, 90);
            forwardPlayer.localPosition = new Vector3(0, 0.6f, 0);
        }
        else if (movement.y < 0)
        {
            forwardPlayer.localRotation = Quaternion.Euler(0, 0, -90);
            forwardPlayer.localPosition = new Vector3(0, -0.6f, 0);
        }
    }
}
