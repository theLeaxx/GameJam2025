using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class DamageOnTouch : MonoBehaviour
{
    public float damageValue;
    public float interval;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            StartCoroutine(DamageEveryTick(other.GetComponent<PlayerScriptBase>()));
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            StopAllCoroutines();
    }
    private IEnumerator DamageEveryTick(PlayerScriptBase player)
    {
        while (true)
        {
            player.TakeDamage(damageValue);
            yield return new WaitForSeconds(interval);
        }
    }
}
