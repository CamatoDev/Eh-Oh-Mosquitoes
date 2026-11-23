using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gestionnaire centralisé des paramètres et préférences utilisateur pour Eh-Oh-Mosquitoes.
/// Persistance automatique via PlayerPrefs, application temps réel des volumes multi-canaux
/// (Master, SFX, BGM, Bourdonnement 3D), haptique mobile, réticule de visée (via ReticleLibrary),
/// compteur & fréquence FPS, pseudo du joueur et classement Top 10 local.
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
    private const string KEY_LEADERBOARD_COUNT = "LEADERBOARD_COUNT";
    private const string KEY_SANITIZED_V1 = "LEADERBOARD_SANITIZED_TWINS";

    // Protection anti-rebond en mémoire (évite l'enregistrement double à la même seconde)
    private string lastAddedPseudo = "";
    private int lastAddedScore = -1;
    private float lastAddedTime = -999f;

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
    [Tooltip("Option B : Vitesse de rotation caméra réservée au mode test PC/Éditeur (le tracking Vuforia AR mobile reste 1:1)")]
    [Range(0.2f, 2.0f)] public float gyroSensitivity = 1.0f;

    [Header("Personnalisation Viseur & Radar")]
    public int reticleIndex = 1; // 0 = Classique, 1 = Ndop Guerrier, 2 = Précision
    public int reticleColorIndex = 0; // 0 = Or, 1 = Vert, 2 = Rouge, 3 = Blanc
    public bool radarEnabled = true;
    public float radarRadius = 220f;
    [HideInInspector] public Sprite[] fallbackReticleSprites;

    [Header("Graphismes & Batterie AR")]
    public int targetFPS = 60; // 30 ou 60
    public bool showFPSCounter = false;

    [Header("Système & Langues")]
    public string currentLanguage = "FR"; // "FR" ou "EN"
    public string playerPseudo = "Chasseur K-Mer";

    private ReticleLibrary cachedReticleLibrary;

    // Événements pour notifier les éléments de scène en temps réel
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
        ApplyAudioSettings();
        ApplyGraphicSettings();
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

        // Nettoyage automatique des anciens doublons jumeaux polluant les PlayerPrefs
        SanitizeExistingLeaderboard();
    }

    /// <summary>
    /// Sauvegarde l'ensemble des réglages dans PlayerPrefs et notifie toute la scène
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

    /// <summary>
    /// Applique Master + Mute global sur AudioListener et notifie toutes les AudioSources
    /// (BGM, SFX, Bourdonnement 3D) pour mise à jour immédiate.
    /// </summary>
    public void ApplyAudioSettings()
    {
        AudioListener.pause = false;
        AudioListener.volume = isMuteGlobal ? 0f : masterVolume;
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

    public ReticleLibrary GetReticleLibrary()
    {
        if (cachedReticleLibrary == null)
        {
            cachedReticleLibrary = Resources.Load<ReticleLibrary>("ReticleLibrary");
        }
        return cachedReticleLibrary;
    }

    public Sprite GetReticleSprite(int index)
    {
        ReticleLibrary lib = GetReticleLibrary();
        if (lib != null)
        {
            Sprite libSprite = lib.GetSprite(index);
            if (libSprite != null) return libSprite;
        }

        if (fallbackReticleSprites != null && fallbackReticleSprites.Length > 0)
        {
            int safeIdx = Mathf.Clamp(index, 0, fallbackReticleSprites.Length - 1);
            return fallbackReticleSprites[safeIdx];
        }

        return null;
    }

    public float GetReticleScaleMultiplier(int index)
    {
        ReticleLibrary lib = GetReticleLibrary();
        if (lib != null)
        {
            return lib.GetScaleMultiplier(index);
        }
        return 1f;
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
        return PlayerPrefs.GetInt(KEY_BEST_SCORE, 0);
    }

    public void UpdateBestScore(int newScore)
    {
        if (newScore <= 0) return;

        int currentBest = GetBestScore();
        if (newScore > currentBest)
        {
            PlayerPrefs.SetInt(KEY_BEST_SCORE, newScore);
            PlayerPrefs.SetInt("HighScore", newScore);
            PlayerPrefs.Save();
            Debug.Log($"[SettingsManager] Nouveau record enregistré : {newScore} pts !");
        }
        AddLeaderboardScore(playerPseudo, newScore);
        NotifySettingsChanged();
    }

    public void ResetBestScore()
    {
        PlayerPrefs.SetInt(KEY_BEST_SCORE, 0);
        PlayerPrefs.SetInt("HighScore", 0);
        PlayerPrefs.SetInt(KEY_LEADERBOARD_COUNT, 0);
        for (int i = 0; i < 10; i++)
        {
            PlayerPrefs.DeleteKey($"LEADERBOARD_PSEUDO_{i}");
            PlayerPrefs.DeleteKey($"LEADERBOARD_SCORE_{i}");
            PlayerPrefs.DeleteKey($"LEADERBOARD_DATE_{i}");
        }
        PlayerPrefs.Save();
        Debug.Log("[SettingsManager] Records et classement réinitialisés à 0.");
        NotifySettingsChanged();
    }

    [System.Serializable]
    public struct ScoreEntry
    {
        public string pseudo;
        public int score;
        public string date;
    }

    public List<ScoreEntry> GetTopScores(int count = 10)
    {
        List<ScoreEntry> list = new List<ScoreEntry>();
        int savedCount = Mathf.Min(PlayerPrefs.GetInt(KEY_LEADERBOARD_COUNT, 0), count);
        for (int i = 0; i < savedCount; i++)
        {
            string p = PlayerPrefs.GetString($"LEADERBOARD_PSEUDO_{i}", "Chasseur K-Mer");
            int s = PlayerPrefs.GetInt($"LEADERBOARD_SCORE_{i}", 0);
            string d = PlayerPrefs.GetString($"LEADERBOARD_DATE_{i}", "--/--");
            if (s > 0)
            {
                list.Add(new ScoreEntry { pseudo = p, score = s, date = d });
            }
        }
        return list;
    }

    public void AddLeaderboardScore(string pseudo, int score)
    {
        if (score <= 0) return;

        string effectivePseudo = string.IsNullOrEmpty(pseudo) ? playerPseudo : pseudo;

        // Protection anti-rebond immédiat : empêche l'enregistrement double à la même seconde
        if (lastAddedScore == score && lastAddedPseudo == effectivePseudo && (Time.realtimeSinceStartup - lastAddedTime) < 2.0f)
        {
            Debug.LogWarning($"[SettingsManager] Doublon immédiat ignoré pour '{effectivePseudo}' avec {score} pts.");
            return;
        }

        lastAddedPseudo = effectivePseudo;
        lastAddedScore = score;
        lastAddedTime = Time.realtimeSinceStartup;

        List<ScoreEntry> list = GetTopScores(10);
        list.Add(new ScoreEntry
        {
            pseudo = effectivePseudo,
            score = score,
            date = System.DateTime.Now.ToString("dd/MM")
        });
        list.Sort((a, b) => b.score.CompareTo(a.score));

        SaveLeaderboardList(list);
    }

    /// <summary>
    /// Sauvegarde propre d'une liste ordonnée dans PlayerPrefs et efface les clés excédentaires
    /// </summary>
    private void SaveLeaderboardList(List<ScoreEntry> list)
    {
        int finalCount = Mathf.Min(list.Count, 10);
        PlayerPrefs.SetInt(KEY_LEADERBOARD_COUNT, finalCount);
        for (int i = 0; i < finalCount; i++)
        {
            PlayerPrefs.SetString($"LEADERBOARD_PSEUDO_{i}", list[i].pseudo);
            PlayerPrefs.SetInt($"LEADERBOARD_SCORE_{i}", list[i].score);
            PlayerPrefs.SetString($"LEADERBOARD_DATE_{i}", list[i].date);
        }
        for (int i = finalCount; i < 10; i++)
        {
            PlayerPrefs.DeleteKey($"LEADERBOARD_PSEUDO_{i}");
            PlayerPrefs.DeleteKey($"LEADERBOARD_SCORE_{i}");
            PlayerPrefs.DeleteKey($"LEADERBOARD_DATE_{i}");
        }
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Nettoie automatiquement les doublons jumeaux existants dans PlayerPrefs (Option 2 : multi-runs distincts autorisés)
    /// </summary>
    public void SanitizeExistingLeaderboard()
    {
        if (PlayerPrefs.GetInt(KEY_SANITIZED_V1, 0) == 1) return;

        List<ScoreEntry> raw = GetTopScores(10);
        if (raw.Count > 1)
        {
            List<ScoreEntry> cleaned = new List<ScoreEntry>();
            for (int i = 0; i < raw.Count; i++)
            {
                bool isTwin = false;
                // Détecte les doublons stricts jumeaux (même pseudo, même score, même date) créés par le bogue double-trigger
                for (int j = 0; j < cleaned.Count; j++)
                {
                    if (cleaned[j].pseudo == raw[i].pseudo &&
                        cleaned[j].score == raw[i].score &&
                        cleaned[j].date == raw[i].date)
                    {
                        isTwin = true;
                        break;
                    }
                }

                if (!isTwin)
                {
                    cleaned.Add(raw[i]);
                }
            }

            if (cleaned.Count != raw.Count)
            {
                Debug.Log($"[SettingsManager] Nettoyage initial : {raw.Count - cleaned.Count} doublon(s) jumeau(x) supprimé(s).");
                SaveLeaderboardList(cleaned);
            }
        }

        PlayerPrefs.SetInt(KEY_SANITIZED_V1, 1);
        PlayerPrefs.Save();
    }

    public void NotifySettingsChanged()
    {
        OnSettingsChanged?.Invoke();
    }
}
