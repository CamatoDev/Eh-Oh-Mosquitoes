using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameOver : MonoBehaviour
{
    public SceneFader sceneFader;
    public Text finalScore;
    public PlayerStats stats;
    public string menuSceneName = "";

    private bool scoreRecorded = false;

    private void OnEnable()
    {
        UpdateScoreDisplay();
    }

    // Start is called before the first frame update
    void Start()
    {
        // OnEnable prend déjà en charge l'affichage et l'enregistrement unique du score
    }

    public void UpdateScoreDisplay()
    {
        if (stats == null)
        {
            stats = FindObjectOfType<PlayerStats>();
        }

        if (stats != null && finalScore != null)
        {
            int score = Mathf.RoundToInt(stats.points);
            finalScore.text = "Score : " + score.ToString();
            if (!scoreRecorded && SettingsManager.Instance != null)
            {
                scoreRecorded = true;
                SettingsManager.Instance.UpdateBestScore(score);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Menu()
    {
        string targetMenu = !string.IsNullOrEmpty(menuSceneName)
            ? menuSceneName
            : (SceneManager.GetActiveScene().name.Contains("Vuforia") ? "MainMenu_Vuforia" : "MainMenu_ARCore");
        sceneFader.FadeTo(targetMenu);
    }
}
