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

    // Start is called before the first frame update
    void Start()
    {
        timer = 120f;
        gameEnded = false;
        pause = false;
        hasSpawn = false;
        Time.timeScale = 1f; //temps sur play 
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
        if(timer <= 60 && !hasSpawn)
        {
            hasSpawn = true;
            Debug.Log("Apparition du boss");
            GameObject finalEnemy = (GameObject)Instantiate(finalEnemyGO, pos, Quaternion.identity);
        }

        if(timer <= 0f)
        {
            GameOver();
        }

        if(playerStats.currentHealth <= 0f)
        {
            GameOver();
        }
    }

    public void Toggle()
    {
        pauseAudioSource.Play();
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
        pauseAudioSource.Play();
        Time.timeScale = 1f;
        string targetMenu = !string.IsNullOrEmpty(menuSceneName)
            ? menuSceneName
            : (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("Vuforia") ? "MainMenu_Vuforia" : "MainMenu_ARCore");
        sceneFader.FadeTo(targetMenu);
    }

    public void GameOver()
    {
        gameEnded = true;
        Debug.Log("Fin de la partie !");
        gameOverUI.SetActive(true);
        Time.timeScale = 0f; //temps en pause 
    }
}
