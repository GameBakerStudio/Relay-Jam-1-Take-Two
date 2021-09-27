using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
	public AudioClip pickupSound;
	public AudioClip rejectedSound;

	public int healAmount = 1;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		// trigger only if the object has playerHealth
		PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
		if (playerHealth == null) return;

		if (playerHealth.isHealable)
		{
			HealPlayerAndDestroyMe(playerHealth);
		}
		else
		{
			PlayUnhealableSound();
		}
	}

	private void PlayUnhealableSound()
	{

		App.PlayVariedAudio(rejectedSound, transform.position, 1.0f, 1.0f, 0.9f, 1.1f);
	}

	private void HealPlayerAndDestroyMe(PlayerHealth playerHealth)
	{
		playerHealth.Heal(healAmount);
		App.PlayVariedAudio(pickupSound, transform.position, 1.0f, 1.0f, 0.9f, 1.1f);
		gameObject.SetActive(false);
		Destroy(gameObject);
	}

}
