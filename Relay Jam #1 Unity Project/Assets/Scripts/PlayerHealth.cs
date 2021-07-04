using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    public static UnityEvent OnPlayerDeath = new UnityEvent();
    public static Action OnTakeDamage;

    public int health;
    public int baseHealth { get; private set; }

    [SerializeField] private GameObject _deathFX;
    [SerializeField] private SpriteRenderer rend;
    Rigidbody2D rb;

    private bool _isDead;

    private void Awake()
    {
        baseHealth = health;
        if(rend == null) { rend = GetComponent<SpriteRenderer>(); }
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int damage)
    {
        health--;
        OnTakeDamage?.Invoke();
        if (health <= 0 && !_isDead)
        {
            _isDead = true;
            StartCoroutine(Death());
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

    private IEnumerator Death()
    {
        SpawnDeathFX();
        rend.enabled = false;
        App.acceptingMoveInput = false;
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        OnPlayerDeath.Invoke();
        yield return new WaitForSeconds(App.deathDelay);

        _isDead = false;
        rend.enabled = true;
        health = baseHealth;
        rb.velocity = Vector3.zero;
        transform.position = RoomHandler.activeRoom.spawnPoint.position;
        rb.isKinematic = false;
        OnTakeDamage?.Invoke();
        App.acceptingMoveInput = true;
        
    }

    private void SpawnDeathFX()
    {
        App.SpawnDeathMarker(transform.position);
        if(_deathFX == null) { return; }
        var FX = Instantiate(_deathFX, transform.position - Vector3.forward, Quaternion.identity);
        Destroy(FX.gameObject, 5f);
    }
}
