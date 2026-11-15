using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerActions : MonoBehaviour
{
    [Header("Variables")]
    private PlayerStats playerStats;
    public GameManager gameManager;
    public Button shootButton;
    public AudioSource buttonaudioSource;
    public AudioSource flyingMosquitoAudioSource;

    [Header("Shoot Action")]
    public float shootRate = 1f;
    private float spawnProtection;
    private float shootCountdown = 0f;
    public GameObject bulletPrefab;
    public Transform shootPoint;

    [Header("Protection Dégâts")]
    [Tooltip("Délai d'invulnérabilité entre deux piqûres pour éviter les coups instantanés multiples")]
    public float damageCooldown = 0.8f;
    private float lastDamageTime = -10f;

    private void Start()
    {
        playerStats = gameObject.GetComponent<PlayerStats>();
        spawnProtection = 10f;
    }

    private void Update()
    {
        spawnProtection -= Time.deltaTime;

        if (spawnProtection > 0f)
        {
            shootButton.interactable = false;
        }
        else
        {
            shootButton.interactable = true;
            //flyingMosquitoAudioSource.Play();
        }
    }

    public void Shoot()
    {
        if (shootCountdown <= 0)
        {
            GameObject bulletGO = (GameObject)Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
            shootCountdown = 1 / shootRate;
        }

        shootCountdown -= 10;
        buttonaudioSource.Play();

        StartCoroutine(ShootColdDown());
    }

    public void TakeDamage(float damage)
    {
        // Empêcher les dégâts en rafale si plusieurs moustiques frappent en même temps
        if (Time.time - lastDamageTime < damageCooldown)
        {
            return;
        }
        lastDamageTime = Time.time;

        if (playerStats == null)
        {
            playerStats = GetComponent<PlayerStats>();
        }

        if (playerStats != null)
        {
            playerStats.currentHealth -= damage;
            Debug.Log($"[PlayerActions] Dégâts reçus : {damage} | PV restants : {playerStats.currentHealth}");

            if (playerStats.currentHealth <= 0)
            {
                if (gameManager == null)
                {
                    gameManager = FindObjectOfType<GameManager>();
                }

                if (gameManager != null)
                {
                    gameManager.GameOver();
                }
            }
        }
    }

    // Coroutine pour le cooldown de tir
    IEnumerator ShootColdDown()
    {
        shootButton.interactable = false;
        yield return new WaitForSeconds(2f);
        shootButton.interactable = true;
    }
}
