using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    // Variables pour le son 
    [Header("Sound UI")]
    private AudioSource audioSource;
    private bool mute = false;
    public Sprite muteImage;
    public Sprite unmuteImage;
    // Variable pour le bouton de volume
    public Button soundButton;
    public AudioSource buttonAudioSource;

    // Variables pour le chargement du niveau
    [Header("Level Loader")]
    public string levelToLoad = "Level01_Vuforia";
    public SceneFader sceneFader;

    [Header("Panel des Paramètres & Classement")]
    public SettingsPanelUI settingsPanel;
    public LeaderboardPanelUI leaderboardPanel;

    private void Awake()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        SettingsManager.OnSettingsChanged += SyncAudioFromSettings;
    }

    private void OnDisable()
    {
        SettingsManager.OnSettingsChanged -= SyncAudioFromSettings;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.GetComponent<AudioSource>();
        }

        if (SceneManager.GetActiveScene().name.Contains("Vuforia"))
        {
            levelToLoad = "Level01_Vuforia";
        }
        else if (SceneManager.GetActiveScene().name.Contains("ARCore"))
        {
            levelToLoad = "Level01_ARCore";
        }

        // Déclenche l'initialisation de SettingsManager si pas encore présent et applique les volumes
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.ApplyAudioSettings();
            SyncAudioFromSettings();
        }
    }

    /// <summary>
    /// Synchronise en temps réel le volume BGM du menu, le volume SFX des boutons
    /// et l'icône Mute/Unmute du menu principal avec SettingsManager.
    /// </summary>
    public void SyncAudioFromSettings()
    {
        if (SettingsManager.Instance == null) return;
        SettingsManager sm = SettingsManager.Instance;

        mute = sm.isMuteGlobal;

        if (soundButton != null && soundButton.image != null)
        {
            soundButton.image.sprite = mute ? muteImage : unmuteImage;
        }

        if (audioSource != null)
        {
            audioSource.mute = mute;
            audioSource.volume = sm.bgmVolume;
        }

        if (buttonAudioSource != null)
        {
            buttonAudioSource.mute = mute;
            buttonAudioSource.volume = sm.sfxVolume;
        }
    }

    private void PlayButtonClick()
    {
        if (buttonAudioSource == null) return;
        if (SettingsManager.Instance != null)
        {
            if (SettingsManager.Instance.isMuteGlobal) return;
            buttonAudioSource.volume = SettingsManager.Instance.sfxVolume;
        }
        buttonAudioSource.Play();
    }

    // Fonction pour le lancement du jeu 
    public void Play()
    {
        PlayButtonClick();
        if (sceneFader != null)
        {
            sceneFader.FadeTo(levelToLoad);
        }
        else
        {
            SceneManager.LoadScene(levelToLoad);
        }
    }

    // Fonction pour le son (synchronisation bidirectionnelle avec Mute Global dans Settings)
    public void MenuSound()
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.isMuteGlobal = !SettingsManager.Instance.isMuteGlobal;
            SettingsManager.Instance.ApplyAudioSettings();
            SettingsManager.Instance.SaveAllSettings();
            // SyncAudioFromSettings est appelé via OnSettingsChanged, on le sécurise aussi ici
            SyncAudioFromSettings();
            if (!SettingsManager.Instance.isMuteGlobal)
            {
                PlayButtonClick();
            }
        }
        else
        {
            mute = !mute;
            if (soundButton != null && soundButton.image != null)
            {
                soundButton.image.sprite = mute ? muteImage : unmuteImage;
            }
            if (audioSource != null) audioSource.mute = mute;
        }
    }

    // Fonction pour les réglages
    public void Settings()
    {
        PlayButtonClick();
        if (settingsPanel != null)
        {
            settingsPanel.OpenPanel();
        }
        else
        {
            Debug.Log("[MainMenu] Ouverture des options de personnalisations");
        }
    }

    // Fonction pour le classement des meilleurs scores (Top 5 / Top 10)
    public void OpenLeaderboard()
    {
        PlayButtonClick();
        if (leaderboardPanel != null)
        {
            leaderboardPanel.OpenPanel();
        }
        else
        {
            Debug.Log("[MainMenu] Ouverture du panel Classement (MyScore)");
        }
    }
}
