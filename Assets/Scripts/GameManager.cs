using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text timerText;
    float timer;

    [Header("UI")]
    public SceneFader sceneFader;
    public AudioSource pauseAudioSource;
    bool pause;
    public Button shoot;
    public GameObject pauseMenuUI;
    public GameObject gameOverUI;
    bool gameEnded;

    //Player Ref
    [Header("Player")]
    public PlayerStats playerStats;
    public Transform playerPos;

    [Header("Scene Navigation")]
    public string menuSceneName = "";

    [Header("Final Enemy")]
    public GameObject finalEnemyGO;
    bool hasSpawn = false;

    [Header("Paramètres")]
    public SettingsPanelUI settingsPanel;

    private AudioSource gameOverAudioSource;

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
        timer = 120f;
        gameEnded = false;
        pause = false;
        hasSpawn = false;
        Time.timeScale = 1f; //temps sur play 

        if (gameOverUI != null)
        {
            gameOverAudioSource = gameOverUI.GetComponent<AudioSource>();
        }

        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.ApplyAudioSettings();
            SyncAudioFromSettings();
        }
    }

    public void SyncAudioFromSettings()
    {
        if (SettingsManager.Instance == null) return;
        SettingsManager sm = SettingsManager.Instance;

        // Clic boutons Pause / Menu (canal SFX)
        if (pauseAudioSource != null)
        {
            pauseAudioSource.mute = sm.isMuteGlobal;
            pauseAudioSource.volume = sm.sfxVolume;
        }

        // Jingle Game Over (canal SFX, selon choix utilisateur)
        if (gameOverAudioSource == null && gameOverUI != null)
        {
            gameOverAudioSource = gameOverUI.GetComponent<AudioSource>();
        }
        if (gameOverAudioSource != null)
        {
            gameOverAudioSource.mute = sm.isMuteGlobal;
            gameOverAudioSource.volume = sm.sfxVolume;
        }
    }

    private void PlayPauseClick()
    {
        if (pauseAudioSource == null) return;
        if (SettingsManager.Instance != null)
        {
            if (SettingsManager.Instance.isMuteGlobal) return;
            pauseAudioSource.volume = SettingsManager.Instance.sfxVolume;
        }
        pauseAudioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameEnded)
        {
            return;
        }

        // Gestion du temps 
        timer -= Time.deltaTime;
        timer = Mathf.Clamp(timer, 0f, Mathf.Infinity);
        timerText.text = string.Format("{0:00.00}", timer);

        Vector3 pos = playerPos.position + new Vector3(-5f, 0f, 0f);
        if (timer <= 60 && !hasSpawn)
        {
            hasSpawn = true;
            Debug.Log("Apparition du boss");
            GameObject finalEnemy = (GameObject)Instantiate(finalEnemyGO, pos, Quaternion.identity);
        }

        if (timer <= 0f)
        {
            GameOver();
        }

        if (playerStats.currentHealth <= 0f)
        {
            GameOver();
        }
    }

    public void Toggle()
    {
        PlayPauseClick();
        pause = !pause;

        //mettre le temps sur pause si on est en pause et le faire continuer sinon 
        if (pause)
        {
            shoot.interactable = false;
            pauseMenuUI.SetActive(true);
            Time.timeScale = 0f; //temps en pause 
        }
        else
        {
            shoot.interactable = true;
            pauseMenuUI.SetActive(false);
            Time.timeScale = 1f; //temps sur play 
        }
    }

    public void Menu()
    {
        PlayPauseClick();
        Time.timeScale = 1f;
        string targetMenu = !string.IsNullOrEmpty(menuSceneName)
            ? menuSceneName
            : (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("Vuforia") ? "MainMenu_Vuforia" : "MainMenu_ARCore");
        sceneFader.FadeTo(targetMenu);
    }

    public void OpenSettings()
    {
        PlayPauseClick();
        if (settingsPanel != null)
        {
            settingsPanel.OpenPanel();
        }
    }

    public void GameOver()
    {
        gameEnded = true;
        Debug.Log("Fin de la partie !");
        SyncAudioFromSettings();
        gameOverUI.SetActive(true);
    }
}
