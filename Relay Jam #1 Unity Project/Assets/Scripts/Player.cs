using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Don't overengineer stuff in a game jam.
// Keep it effing simple and brute until it doesn't need to be.
// Technical debt in a jam usually doesn't need to get repaid.

public class Player : MonoBehaviour
{
	public const int START_HEALTH = 5;

	// amount of time after being damaged to be invulnerable
	// this prevents double triggering of damage within a very short time
	public const float DAMAGED_INVULNERABILITY_TIME = 0.5f;

	// Here, we just f*** it and make the stats into globals. It's a game jam game.
	// We want this stuff to persist across scenes
	// This also makes it much easier for UI to grab.
	// Use polling rather than events for more robustness.
	public static int keys = 0;
	public static int coins = 0;
	public static int health = START_HEALTH;
	public static float invulnerableUntilTime = 0;

	public static Player instance = null;


	public SpriteRenderer spriteRenderer;
	public Rigidbody2D rb;

	public AudioClip damageSound;
	public AudioClip deathSound;

	private void OnEnable()
	{
		instance = this;
	}
	private void OnDisable()
	{
		if (instance == this)
			instance = null;
	}

	/// <summary>call this when restarting game to clean things out and reset the stats</summary>
	public void ResetStats()
	{
		keys = 0;
		coins = 0;
		health = START_HEALTH;
		invulnerableUntilTime = 0;
	}


	// ---------- HEALTH ----------- 
	//
	#region HEALTH

	public GameObject deathFXPrefab;

	public static bool isDead { get { return health <= 0; } }
	public static bool isInvulnerable { get { return Time.time < invulnerableUntilTime; } }

	public void DamageHealth(int damage)
	{
		if (isInvulnerable || isDead) return;

		health -= damage;
		StartCoroutine(DamageCoroutine());

		if (health <= 0)
		{
			health = 0;
			StartCoroutine(Death());
		}
		else
		{
			App.PlayVariedAudio(damageSound, transform.position, 0.5f, 0.6f, 0.9f, 1.1f);
			invulnerableUntilTime = Time.time + DAMAGED_INVULNERABILITY_TIME;
		}
	}

	private IEnumerator Death()
	{
		App.PlayVariedAudio(deathSound, transform.position, 0.5f, 0.6f, 0.9f, 1.1f);
		SpawnDeathFX();
		spriteRenderer.enabled = false;
		App.acceptingMoveInput = false;
		rb.isKinematic = true;
		rb.velocity = Vector3.zero;

		yield return new WaitForSeconds(App.deathDelay);

		spriteRenderer.enabled = true;
		health = 1;

		rb.velocity = Vector3.zero;
		transform.position = RoomHandler.activeRoom.spawnPoint.position;
		rb.isKinematic = false;
		invulnerableUntilTime = Time.time + RESPAWN_INVULNERABILITY_DURATION;
		App.acceptingMoveInput = true;
	}
	const float RESPAWN_INVULNERABILITY_DURATION = 3.0f;

	private void SpawnDeathFX()
	{
		App.SpawnDeathMarker(transform.position);
		if (deathFXPrefab == null) { return; }
		GameObject deathEffectInstance = Instantiate(deathFXPrefab, transform.position - Vector3.forward, Quaternion.identity);
		Destroy(deathEffectInstance, 5f);
	}

	public void Heal(int amount)
	{
		health += amount;
	}


	IEnumerator DamageCoroutine()
	{
		//Fade the player sprite to alpha = 0, in 0.6f seconds;
		spriteRenderer.color = Color.red;
		yield return new WaitForSeconds(0.1f);
		spriteRenderer.color = Color.white;
	}

	#endregion HEALTH

	// ---------- KEYS ----------- 
	//

	#region Keys

	public static bool hasKeys { get => keys > 0; }

	public void AddKey(int amount = 1)
	{
		keys += amount;
	}

	public void UseKey()
	{
		if (hasKeys)
		{
			keys--;
		}
		else
		{
			Debug.LogError("UseKey called without any keys!!");
		}
	}
	#endregion Keys

	// ---------- COINS -----------
	//

	#region COINS
	public void AddCoin(int amount = 1)
	{
		coins += amount;
	}
	#endregion COINS


}
