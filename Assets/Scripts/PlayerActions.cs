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

    private void OnEnable()
    {
        SettingsManager.OnSettingsChanged += SyncAudioAndControls;
    }

    private void OnDisable()
    {
        SettingsManager.OnSettingsChanged -= SyncAudioAndControls;
    }

    private void Start()
    {
        playerStats = gameObject.GetComponent<PlayerStats>();
        spawnProtection = initialSpawnProtection;

        EnsureIndependentAudioSources();
        SyncAudioAndControls();
    }

    /// <summary>
    /// Garantit que le bourdonnement 3D des moustiques et le son de piqûre (SFX)
    /// possèdent deux AudioSources distinctes pour des réglages de volume 100% indépendants.
    /// </summary>
    private void EnsureIndependentAudioSources()
    {
        if (flyingMosquitoAudioSource == null)
        {
            flyingMosquitoAudioSource = GetComponent<AudioSource>();
        }

        if (hurtAudioSource == null || hurtAudioSource == flyingMosquitoAudioSource)
        {
            AudioSource dedicatedHurt = gameObject.AddComponent<AudioSource>();
            dedicatedHurt.playOnAwake = false;
            dedicatedHurt.loop = false;
            dedicatedHurt.spatialBlend = 0f;
            hurtAudioSource = dedicatedHurt;
        }
    }

    public void SyncAudioAndControls()
    {
        ApplyHandedness();

        if (SettingsManager.Instance == null) return;
        SettingsManager sm = SettingsManager.Instance;

        EnsureIndependentAudioSources();

        // Bourdonnement moustiques (slider dédié Bourdonnement Moustiques 3D)
        if (flyingMosquitoAudioSource != null)
        {
            flyingMosquitoAudioSource.mute = sm.isMuteGlobal;
            flyingMosquitoAudioSource.volume = sm.mosquitoBuzzVolume;
        }

        // Spray / Tir (slider Effets Sonores SFX)
        if (buttonaudioSource != null)
        {
            buttonaudioSource.mute = sm.isMuteGlobal;
            buttonaudioSource.volume = sm.sfxVolume;
        }

        // Son de piqûre subie (slider Effets Sonores SFX)
        if (hurtAudioSource != null)
        {
            hurtAudioSource.mute = sm.isMuteGlobal;
            hurtAudioSource.volume = sm.sfxVolume;
        }
    }

    public void ApplyHandedness()
    {
        if (shootButton != null && SettingsManager.Instance != null)
        {
            RectTransform rt = shootButton.GetComponent<RectTransform>();
            if (rt != null)
            {
                if (SettingsManager.Instance.isLeftHanded)
                {
                    rt.anchorMin = new Vector2(0f, 0f);
                    rt.anchorMax = new Vector2(0f, 0f);
                    rt.anchoredPosition = new Vector2(Mathf.Abs(rt.anchoredPosition.x), rt.anchoredPosition.y);
                }
                else
                {
                    rt.anchorMin = new Vector2(1f, 0f);
                    rt.anchorMax = new Vector2(1f, 0f);
                    rt.anchoredPosition = new Vector2(-Mathf.Abs(rt.anchoredPosition.x), rt.anchoredPosition.y);
                }
            }
        }
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

        if (buttonaudioSource != null)
        {
            if (SettingsManager.Instance != null)
            {
                buttonaudioSource.mute = SettingsManager.Instance.isMuteGlobal;
                buttonaudioSource.volume = SettingsManager.Instance.sfxVolume;
            }
            buttonaudioSource.Play();
        }

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

        // Effet sonore de piqûre / blessure (lié au volume SFX)
        EnsureIndependentAudioSources();
        float sfxVol = SettingsManager.Instance != null ? SettingsManager.Instance.sfxVolume : 1f;
        bool isMuted = SettingsManager.Instance != null && SettingsManager.Instance.isMuteGlobal;

        if (!isMuted && hurtSound != null)
        {
            if (hurtAudioSource != null)
            {
                hurtAudioSource.volume = sfxVol;
                hurtAudioSource.PlayOneShot(hurtSound, 1f);
            }
            else
            {
                AudioSource.PlayClipAtPoint(hurtSound, transform.position, sfxVol);
            }
        }

        // Retour haptique smartphone (UNIQUEMENT sur piqûre subie)
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.TriggerBiteHaptic();
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
