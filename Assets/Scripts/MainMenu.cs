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

    [Header("Panel des Paramètres")]
    public SettingsPanelUI settingsPanel;

    // Start is called before the first frame update
    void Start()
    {
        // Assignation des variables 
        audioSource = gameObject.GetComponent<AudioSource>();

        if (SceneManager.GetActiveScene().name.Contains("Vuforia"))
        {
            levelToLoad = "Level01_Vuforia";
        }
        else if (SceneManager.GetActiveScene().name.Contains("ARCore"))
        {
            levelToLoad = "Level01_ARCore";
        }

        if (SettingsManager.Instance != null)
        {
            mute = SettingsManager.Instance.isMuteGlobal;
            if (soundButton != null && soundButton.image != null)
            {
                soundButton.image.sprite = mute ? muteImage : unmuteImage;
            }
        }
    }

    // Fonction pour le lancement du jeu 
    public void Play()
    {
        if (buttonAudioSource != null) buttonAudioSource.Play();
        if (sceneFader != null)
        {
            sceneFader.FadeTo(levelToLoad);
        }
        else
        {
            SceneManager.LoadScene(levelToLoad);
        }
    }

    // Fonction pour le son 
    public void MenuSound()
    {
        if (buttonAudioSource != null) buttonAudioSource.Play();
        mute = !mute;
        if (soundButton != null && soundButton.image != null)
        {
            soundButton.image.sprite = mute ? muteImage : unmuteImage;
        }
        if (audioSource != null)
        {
            audioSource.mute = mute;
        }
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.isMuteGlobal = mute;
            SettingsManager.Instance.ApplyAudioSettings();
        }
    }

    // Fonction pour les réglages
    public void Settings()
    {
        if (buttonAudioSource != null) buttonAudioSource.Play();
        if (settingsPanel != null)
        {
            settingsPanel.OpenPanel();
        }
        else
        {
            Debug.Log("[MainMenu] Ouverture des options de personnalisations");
        }
    }
}
