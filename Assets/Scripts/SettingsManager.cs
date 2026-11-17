using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gestionnaire centralisé des paramètres et préférences utilisateur pour Eh-Oh-Mosquitoes.
/// Persistance automatique via PlayerPrefs, application temps réel des volumes,
/// haptique mobile, réticule de visée et fréquence d'affichage (FPS).
/// Gère également le pseudo du joueur et le stockage des meilleurs scores (Top 5 / Top 10).
/// </summary>
public class SettingsManager : MonoBehaviour
{
    private static SettingsManager _instance;
    public static SettingsManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SettingsManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("SettingsManager");
                    _instance = go.AddComponent<SettingsManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    [Header("Clés PlayerPrefs")]
    private const string KEY_MASTER_VOL = "SETTINGS_MASTER_VOL";
    private const string KEY_SFX_VOL = "SETTINGS_SFX_VOL";
    private const string KEY_BGM_VOL = "SETTINGS_BGM_VOL";
    private const string KEY_BUZZ_VOL = "SETTINGS_BUZZ_VOL";
    private const string KEY_MUTE_GLOBAL = "SETTINGS_MUTE_GLOBAL";
    private const string KEY_HAPTIC_BITE = "SETTINGS_HAPTIC_BITE";
    private const string KEY_LEFT_HANDED = "SETTINGS_LEFT_HANDED";
    private const string KEY_GYRO_SENS = "SETTINGS_GYRO_SENS";
    private const string KEY_RETICLE_INDEX = "SETTINGS_RETICLE_INDEX";
    private const string KEY_RETICLE_COLOR = "SETTINGS_RETICLE_COLOR";
    private const string KEY_RADAR_ENABLED = "SETTINGS_RADAR_ENABLED";
    private const string KEY_RADAR_RADIUS = "SETTINGS_RADAR_RADIUS";
    private const string KEY_TARGET_FPS = "SETTINGS_TARGET_FPS";
    private const string KEY_SHOW_FPS = "SETTINGS_SHOW_FPS";
    private const string KEY_LANGUAGE = "SETTINGS_LANGUAGE";
    private const string KEY_PLAYER_PSEUDO = "PLAYER_PSEUDO";
    private const string KEY_BEST_SCORE = "BEST_SCORE";

    [Header("Audio")]
    [Range(0f, 1f)] public float masterVolume = 0.8f;
    [Range(0f, 1f)] public float sfxVolume = 0.9f;
    [Range(0f, 1f)] public float bgmVolume = 0.65f;
    [Range(0f, 1f)] public float mosquitoBuzzVolume = 0.75f;
    public bool isMuteGlobal = false;

    [Header("Contrôles & Haptique")]
    [Tooltip("Retour haptique déclenché UNIQUEMENT lors d'une piqûre de moustique subie")]
    public bool hapticBiteEnabled = true;
    public bool isLeftHanded = false;
    [Range(0.2f, 2.0f)] public float gyroSensitivity = 1.0f;

    [Header("Personnalisation Viseur & Radar")]
    public int reticleIndex = 1; // 0 = Classique, 1 = Ndop Guerrier, 2 = Précision
    public int reticleColorIndex = 0; // 0 = Or, 1 = Vert, 2 = Rouge, 3 = Blanc
    public bool radarEnabled = true;
    public float radarRadius = 220f;

    [Header("Graphismes & Batterie AR")]
    public int targetFPS = 60; // 30 ou 60
    public bool showFPSCounter = false;

    [Header("Système & Langues")]
    public string currentLanguage = "FR"; // "FR" ou "EN"
    public string playerPseudo = "Chasseur K-Mer";

    // Événements pour notifier les éléments de scène
    public delegate void OnSettingsChangedDelegate();
    public static event OnSettingsChangedDelegate OnSettingsChanged;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
        LoadAllSettings();
    }

    private void Start()
    {
        ApplyAudioSettings();
        ApplyGraphicSettings();
    }

    /// <summary>
    /// Charge les préférences sauvegardées ou applique les valeurs par défaut
    /// </summary>
    public void LoadAllSettings()
    {
        masterVolume = PlayerPrefs.GetFloat(KEY_MASTER_VOL, 0.8f);
        sfxVolume = PlayerPrefs.GetFloat(KEY_SFX_VOL, 0.9f);
        bgmVolume = PlayerPrefs.GetFloat(KEY_BGM_VOL, 0.65f);
        mosquitoBuzzVolume = PlayerPrefs.GetFloat(KEY_BUZZ_VOL, 0.75f);
        isMuteGlobal = PlayerPrefs.GetInt(KEY_MUTE_GLOBAL, 0) == 1;

        hapticBiteEnabled = PlayerPrefs.GetInt(KEY_HAPTIC_BITE, 1) == 1;
        isLeftHanded = PlayerPrefs.GetInt(KEY_LEFT_HANDED, 0) == 1;
        gyroSensitivity = PlayerPrefs.GetFloat(KEY_GYRO_SENS, 1.0f);

        reticleIndex = PlayerPrefs.GetInt(KEY_RETICLE_INDEX, 1);
        reticleColorIndex = PlayerPrefs.GetInt(KEY_RETICLE_COLOR, 0);
        radarEnabled = PlayerPrefs.GetInt(KEY_RADAR_ENABLED, 1) == 1;
        radarRadius = PlayerPrefs.GetFloat(KEY_RADAR_RADIUS, 220f);

        targetFPS = PlayerPrefs.GetInt(KEY_TARGET_FPS, 60);
        showFPSCounter = PlayerPrefs.GetInt(KEY_SHOW_FPS, 0) == 1;

        currentLanguage = PlayerPrefs.GetString(KEY_LANGUAGE, "FR");
        if (currentLanguage != "FR" && currentLanguage != "EN")
        {
            currentLanguage = "FR";
        }

        playerPseudo = PlayerPrefs.GetString(KEY_PLAYER_PSEUDO, "Chasseur K-Mer");
    }

    /// <summary>
    /// Sauvegarde l'ensemble des réglages dans PlayerPrefs
    /// </summary>
    public void SaveAllSettings()
    {
        PlayerPrefs.SetFloat(KEY_MASTER_VOL, masterVolume);
        PlayerPrefs.SetFloat(KEY_SFX_VOL, sfxVolume);
        PlayerPrefs.SetFloat(KEY_BGM_VOL, bgmVolume);
        PlayerPrefs.SetFloat(KEY_BUZZ_VOL, mosquitoBuzzVolume);
        PlayerPrefs.SetInt(KEY_MUTE_GLOBAL, isMuteGlobal ? 1 : 0);

        PlayerPrefs.SetInt(KEY_HAPTIC_BITE, hapticBiteEnabled ? 1 : 0);
        PlayerPrefs.SetInt(KEY_LEFT_HANDED, isLeftHanded ? 1 : 0);
        PlayerPrefs.SetFloat(KEY_GYRO_SENS, gyroSensitivity);

        PlayerPrefs.SetInt(KEY_RETICLE_INDEX, reticleIndex);
        PlayerPrefs.SetInt(KEY_RETICLE_COLOR, reticleColorIndex);
        PlayerPrefs.SetInt(KEY_RADAR_ENABLED, radarEnabled ? 1 : 0);
        PlayerPrefs.SetFloat(KEY_RADAR_RADIUS, radarRadius);

        PlayerPrefs.SetInt(KEY_TARGET_FPS, targetFPS);
        PlayerPrefs.SetInt(KEY_SHOW_FPS, showFPSCounter ? 1 : 0);
        PlayerPrefs.SetString(KEY_LANGUAGE, currentLanguage);
        PlayerPrefs.SetString(KEY_PLAYER_PSEUDO, playerPseudo);

        PlayerPrefs.Save();
        NotifySettingsChanged();
    }

    public void ApplyAudioSettings()
    {
        AudioListener.volume = isMuteGlobal ? 0f : masterVolume;
        AudioListener.pause = isMuteGlobal;
        NotifySettingsChanged();
    }

    public void ApplyGraphicSettings()
    {
        Application.targetFrameRate = targetFPS;
        QualitySettings.vSyncCount = (targetFPS == 60) ? 1 : 0;
        NotifySettingsChanged();
    }

    public void TriggerBiteHaptic()
    {
        if (!hapticBiteEnabled) return;

#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            Handheld.Vibrate();
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("[SettingsManager] Erreur haptique : " + e.Message);
        }
#else
        Debug.Log("[SettingsManager] Haptique déclenché (Piqûre moustique subie) !");
#endif
    }

    public Color GetReticleColor()
    {
        return reticleColorIndex switch
        {
            0 => new Color(0.96f, 0.72f, 0.18f, 1f), // Or africain (#F5B82E)
            1 => new Color(0.35f, 0.52f, 0.27f, 1f), // Vert Ndop (#5A8546)
            2 => new Color(0.84f, 0.20f, 0.16f, 1f), // Rouge alerte (#D63428)
            _ => Color.white
        };
    }

    public int GetBestScore()
    {
        return PlayerPrefs.GetInt(KEY_BEST_SCORE, 12450);
    }

    public void UpdateBestScore(int newScore)
    {
        int currentBest = GetBestScore();
        if (newScore > currentBest)
        {
            PlayerPrefs.SetInt(KEY_BEST_SCORE, newScore);
            PlayerPrefs.SetInt("HighScore", newScore);
            PlayerPrefs.Save();
            Debug.Log($"[SettingsManager] Nouveau record enregistré : {newScore} pts !");
        }
        AddLeaderboardScore(playerPseudo, newScore);
    }

    public void ResetBestScore()
    {
        PlayerPrefs.DeleteKey(KEY_BEST_SCORE);
        PlayerPrefs.DeleteKey("HighScore");
        for (int i = 0; i < 10; i++)
        {
            PlayerPrefs.DeleteKey($"LEADERBOARD_PSEUDO_{i}");
            PlayerPrefs.DeleteKey($"LEADERBOARD_SCORE_{i}");
            PlayerPrefs.DeleteKey($"LEADERBOARD_DATE_{i}");
        }
        PlayerPrefs.Save();
        Debug.Log("[SettingsManager] Records et meilleurs scores réinitialisés.");
    }

    [System.Serializable]
    public struct ScoreEntry
    {
        public string pseudo;
        public int score;
        public string date;
    }

    public List<ScoreEntry> GetTopScores(int count = 5)
    {
        List<ScoreEntry> list = new List<ScoreEntry>();
        for (int i = 0; i < count; i++)
        {
            string defaultPseudo = i == 0 ? "Chasseur Élite" : (i == 1 ? "Mami Wata" : "Tirailleur");
            int defaultScore = i == 0 ? 12450 : (5 - i) * 2200;
            string p = PlayerPrefs.GetString($"LEADERBOARD_PSEUDO_{i}", defaultPseudo);
            int s = PlayerPrefs.GetInt($"LEADERBOARD_SCORE_{i}", defaultScore);
            string d = PlayerPrefs.GetString($"LEADERBOARD_DATE_{i}", "17/11");
            list.Add(new ScoreEntry { pseudo = p, score = s, date = d });
        }
        return list;
    }

    public void AddLeaderboardScore(string pseudo, int score)
    {
        List<ScoreEntry> list = GetTopScores(10);
        list.Add(new ScoreEntry
        {
            pseudo = string.IsNullOrEmpty(pseudo) ? playerPseudo : pseudo,
            score = score,
            date = System.DateTime.Now.ToString("dd/MM")
        });
        list.Sort((a, b) => b.score.CompareTo(a.score));
        for (int i = 0; i < 10; i++)
        {
            if (i < list.Count)
            {
                PlayerPrefs.SetString($"LEADERBOARD_PSEUDO_{i}", list[i].pseudo);
                PlayerPrefs.SetInt($"LEADERBOARD_SCORE_{i}", list[i].score);
                PlayerPrefs.SetString($"LEADERBOARD_DATE_{i}", list[i].date);
            }
        }
        PlayerPrefs.Save();
    }

    private void NotifySettingsChanged()
    {
        OnSettingsChanged?.Invoke();
    }
}
