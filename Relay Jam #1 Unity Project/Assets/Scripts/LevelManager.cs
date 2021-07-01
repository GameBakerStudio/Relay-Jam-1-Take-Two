using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    WaitForSeconds deathTime = new WaitForSeconds(1.5f);

    public void Awake()
    {
        PlayerHealth.OnPlayerDeath.AddListener(PlayerDeath);
    }

    public void PlayerDeath()
    {
        StartCoroutine(PlayerDeathCoroutine());
    }
    IEnumerator PlayerDeathCoroutine()
    {
        yield return deathTime;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
