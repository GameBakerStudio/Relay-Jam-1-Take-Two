using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealthUI : MonoBehaviour
{
	[SerializeField] private PlayerHealth _playerHealth;
	[SerializeField] private TMP_Text _healthTMP;
	private readonly string _heartFull = "<sprite index=[0]>";
	private readonly string _heartEmpty = "<sprite index=[1]>";

	private void Start()
	{
		PlayerHealth.OnHealthChanged += OnHealthChanged;
		OnHealthChanged();
	}

	private void OnDestroy()
	{
		PlayerHealth.OnHealthChanged -= OnHealthChanged;
	}

	void OnHealthChanged()
	{
		int current = _playerHealth.health;
		int max = _playerHealth.maxHealth;

		string healthString = string.Empty;

		for (int i = 0; i < current; i++)
		{
			healthString += _heartFull;
		}

		for (int i = 0; i < max - current; i++)
		{
			healthString += _heartEmpty;
		}

		_healthTMP.text = healthString;
	}
}
