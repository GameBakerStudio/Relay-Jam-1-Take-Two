using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour
{
	public AudioClip pickupSound;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		Player player = collision.gameObject.GetComponent<Player>();
		if (player == null) return;

		player.AddCoin();

		App.PlayVariedAudio(pickupSound, transform.position, 1.0f, 1.0f, 0.8f, 1.2f);

		Destroy(gameObject);
	}

}
