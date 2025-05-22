using System.Collections;
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
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isAnyCoroutineRunning = true;
            StartCoroutine(FireAbility());
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            isAnyCoroutineRunning = false;
            StopCoroutine(FireAbility());
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(DashCooldown());
        }

        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            TransformEnergyInHealth();
        }

        if (Input.GetKeyDown(KeyCode.RightAlt))
        {
            TransformHealthInEnergy();
        }

        MoveLogic();
    }

    private IEnumerator DashCooldown()
    {
        canDash = false;

        Debug.Log("Dashing!");
        GameManager.Instance.DecreaseEnergy(15f);
        isDashing = true;
        yield return new WaitForSeconds(0.1f);
        isDashing = false;

        yield return new WaitForSeconds(1f); 

        canDash = true;
    }

    private IEnumerator FireAbility()
    {
        while(isAnyCoroutineRunning)
        {
            if (GameManager.Instance.EnergyLevel > 0)
            {
                GameManager.Instance.DecreaseEnergy(Random.Range(2.5f, 10.5f));

                var bullet = Instantiate(bulletPrefab, forwardPlayer.position, forwardPlayer.rotation);
                bullet.GetComponent<Bullet>().Shoot(forwardPlayer, forwardPlayer, 10f, Random.Range(2, 10));

                Debug.Log("Firing ability!");
            }
            else
            {
                Debug.Log("Not enough energy to fire ability.");
                yield break;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}
