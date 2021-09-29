using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class HUDController : MonoBehaviour
{
	public RectTransform heartsContainer;

	[Tooltip("array of objects to activate/deactivate for the hearts")]
	public GameObject[] hearts;

	public RectTransform keysContainer;
	public TextMeshProUGUI keysLabel;

	public RectTransform coinsContainer;
	public TextMeshProUGUI coinsLabel;

	private int lastHealth = 0;
	private int lastKeys = 0;
	private int lastCoins = 0;

	private Vector3 initialHeartsContainerScale;
	private Vector3 initialCoinContainerScale;
	private Vector3 initialKeyContainerScale;

	private void Awake()
	{
		initialHeartsContainerScale = heartsContainer.localScale;
		initialCoinContainerScale = coinsContainer.localScale;
		initialKeyContainerScale = keysContainer.localScale;

	}
	public Vector3 punchScale = new Vector3(1.2f, 1.2f, 1.0f);
	public float punchDuration = 0.5f;

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
		PunchContainer(coinsContainer, initialCoinContainerScale);
		lastCoins = Player.coins;
	}

	private void RefreshKeys()
	{
		keysLabel.text = Player.keys.ToString();
		PunchContainer(keysContainer, initialKeyContainerScale);
		lastKeys = Player.keys;
	}

	private void RefreshHealth()
	{
		for (int i = 0; i < hearts.Length; i++)
		{
			hearts[i].gameObject.SetActive(i < Player.health);
		}

		PunchContainer(heartsContainer, initialHeartsContainerScale);
		lastHealth = Player.health;
	}

	/// <summary>
	/// make the container punch out to a scale temporarily
	/// </summary>
	private void PunchContainer(RectTransform container, Vector3 initialScale)
	{
		container.DOComplete();
		container.localScale = initialScale;
		container.DOPunchScale(punchScale, punchDuration, 0, 0);
	}
}
