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
    [Tooltip("Délai de sécurité au début avant de pouvoir tirer")]
    public float initialSpawnProtection = 3.0f;
    private float spawnProtection;
    private float shootCountdown = 0f;
    public GameObject bulletPrefab;
    public Transform shootPoint;

    [Header("Protection Dégâts")]
    [Tooltip("Délai d'invulnérabilité entre deux piqûres pour éviter les coups instantanés multiples")]
    public float damageCooldown = 0.8f;
    private float lastDamageTime = -10f;

    [Header("Audio Dégâts")]
    [Tooltip("Son joué lorsque le joueur est piqué par un moustique")]
    public AudioClip hurtSound;
    public AudioSource hurtAudioSource;

    private void Start()
    {
        playerStats = gameObject.GetComponent<PlayerStats>();
        spawnProtection = initialSpawnProtection;
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

        // Effet sonore de piqûre / blessure
        if (hurtAudioSource != null && hurtSound != null)
        {
            hurtAudioSource.PlayOneShot(hurtSound);
        }
        else if (hurtSound != null)
        {
            AudioSource.PlayClipAtPoint(hurtSound, transform.position);
        }

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
