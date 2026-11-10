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

    // Variales pour le chargement du niveau
    [Header("Level Loader")]
    public string levelToLoad = "Level01_ARCore";
    public SceneFader sceneFader;

    // Start is called before the first frame update
    void Start()
    {
        // Assignation des variables 
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    // Fonction pour le lacement du jeu 
    public void Play()
    {
        //Lacement du jeu 
        buttonAudioSource.Play();
        sceneFader.FadeTo(levelToLoad);
    }

    //Fonction pour le son 
    public void MenuSound()
    {
        buttonAudioSource.Play();
        mute = !mute;
        if (mute)
        {
            soundButton.image.sprite = muteImage;
            audioSource.mute = true;
        }
        else
        {

            soundButton.image.sprite = unmuteImage;
            audioSource.mute = false;
        }
    }

    // Fonction pour les réglages
    public void Settings()
    {
        //
        buttonAudioSource.Play();
        Debug.Log("Ouvertures des options de personnalisations");
    }
}
