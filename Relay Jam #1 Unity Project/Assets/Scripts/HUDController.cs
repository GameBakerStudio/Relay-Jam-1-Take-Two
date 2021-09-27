using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class HUDController : MonoBehaviour
{
	[Tooltip("array of objects to activate/deactivate for the hearts")]
	public GameObject[] hearts;

	public TextMeshProUGUI keyLabel;
	public TextMeshProUGUI coinsLabel;

	private int lastHealth = 0;
	private int lastKeys = 0;
	private int lastCoins = 0;

	// Start is called before the first frame update
	void OnEnable()
	{
		RefreshHealth();
		RefreshKeys();
		RefreshCoins();
	}

	// Update is called once per frame
	void Update()
	{
		// just use simple polling with comparison against
		// last stored value. It's the easiest, most robust.
		// don't prematurely optimize

		if (Player.health != lastHealth)
		{
			RefreshHealth();
		}

		if (Player.keys != lastKeys)
		{
			RefreshKeys();
		}

		if (Player.coins != lastCoins)
		{
			RefreshCoins();
		}
	}

	private void RefreshCoins()
	{
		coinsLabel.text = Player.coins.ToString();
		coinsLabel.DOComplete();
		lastCoins = Player.coins;
	}

	private void RefreshKeys()
	{
		keyLabel.text = Player.keys.ToString();
		lastKeys = Player.keys;
	}

	private void RefreshHealth()
	{
		for (int i = 0; i < hearts.Length; i++)
		{
			hearts[i].gameObject.SetActive(i < Player.health);
		}
		lastHealth = Player.health;
	}
}
