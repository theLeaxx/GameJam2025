using System.Collections;
using Unity.VisualScripting;
using UnityEditor.AnimatedValues;
using UnityEngine;


public class StrikerPlayer : PlayerScriptBase
{
    public override string HorizontalAxis => "HorizontalWASD";
    public override string VerticalAxis => "VerticalWASD";

    private bool isAnyCoroutineRunning = false;
    private bool canDash = true;

    [SerializeField]
    private GameObject bulletPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetVariables();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && !PauseManager.Instance.isPaused && animator.GetFloat("Vertical") != -1 && animator.GetFloat("LastDir") != 0)
        {
            isAnyCoroutineRunning = true;
            StartCoroutine(FireAbility());
        }

        if (Input.GetKeyUp(KeyCode.LeftControl) && !PauseManager.Instance.isPaused)
        {
            isAnyCoroutineRunning = false;
            StopCoroutine(FireAbility());
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && (Input.GetAxis(HorizontalAxis) != 0 || Input.GetAxis(VerticalAxis) != 0) && !PauseManager.Instance.isPaused && GameManager.Instance.EnergyLevel >= 15f)
        {
            StartCoroutine(DashCooldown());
        }

        if (Input.GetKeyDown(KeyCode.LeftAlt) && !PauseManager.Instance.isPaused)
        {
            TransformEnergyInHealth();
        }

        if (Input.GetKeyDown(KeyCode.RightAlt) && !PauseManager.Instance.isPaused)
        {
            TransformHealthInEnergy();
        }

        MoveLogic();
    }

    private IEnumerator DashCooldown()
    {
        canDash = false;

        animator.SetBool("IsDashing", true);
        Debug.Log("Dashing!");
        GameManager.Instance.DecreaseEnergy(10f);
        isDashing = true;
        yield return new WaitForSeconds(0.05f);
        animator.SetBool("IsDashing", false);
        yield return new WaitForSeconds(0.1f);
        isDashing = false;

        yield return new WaitForSeconds(1f);

        canDash = true;
    }

    private IEnumerator FireAbility()
    {
        while(isAnyCoroutineRunning)
        {
            canMove = false;
            animator.SetBool("IsShooting", true);

            if(animator.GetFloat("LastDir") == 2)
                animator.SetFloat("LastDir", 3);

            if (GameManager.Instance.EnergyLevel > 2.5f)
            {
                GameManager.Instance.DecreaseEnergy(Random.Range(1.5f, 2.5f));

                var bullet = Instantiate(bulletPrefab, forwardPlayer.position, forwardPlayer.rotation);
                bullet.GetComponent<Bullet>().Shoot(forwardPlayer, forwardPlayer, 7f, Random.Range(2, 10));

                Debug.Log("Firing ability!");

                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                Debug.Log("Not enough energy to fire ability.");
                animator.SetBool("IsShooting", false);
                canMove = true;
                animator.SetFloat("LastDir", 2);
                yield break;
            }
            yield return new WaitForSeconds(0.1f);
        }

        animator.SetFloat("LastDir", 2);
        canMove = true;
        animator.SetBool("IsShooting", false);
    }
}
