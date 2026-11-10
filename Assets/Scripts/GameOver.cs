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

    // Start is called before the first frame update
    void Start()
    {
        finalScore.text = "Score : " + stats.points;
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
