using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    public static UnityEvent OnPlayerDeath = new UnityEvent();

    public int health;

    Rigidbody2D rb;
    SpriteRenderer rend;

    private void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int damage)
    {
        health--;
        
        if (health <= 0)
        {
            Death();
            return;
        }

        StartCoroutine(DamageCoroutine());
    }
    IEnumerator DamageCoroutine()
    {
        //Fade the player sprite to alpha = 0, in 0.6f seconds;
        rend.color = Color.red;

        yield return new WaitForSeconds(0.1f);

        rend.color = Color.white;
    }

    private void Death()
    {
        OnPlayerDeath.Invoke();
        Destroy(gameObject);
    }
}
