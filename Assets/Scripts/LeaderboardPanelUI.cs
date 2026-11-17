using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Contrôleur d'interface utilisateur pour le Panel Classement (MyScore — Top 5 / Top 10).
/// Reproduction fidèle de la maquette HTML Ndop : Podium Top 3 (Or/Argent/Bronze),
/// liste #4 à #10, surbrillance dorée du pseudo actif, état vide après reset et bilinguisme FR/EN.
/// </summary>
public class LeaderboardPanelUI : MonoBehaviour
{
    [System.Serializable]
    public class PodiumSlotUI
    {
        public GameObject root;
        public Image bgImage;
        public Image borderImage;
        public Text pseudoText;
        public Text scoreText;
        public Text dateText;
    }

    [System.Serializable]
    public class ScoreRowUI
    {
        public GameObject root;
        public Image bgImage;
        public Image borderImage;
        public Text rankText;
        public Text pseudoText;
        public GameObject meBadge;
        public Text meBadgeText;
        public Text scoreText;
        public Text dateText;
    }

    [Header("Structure Principale")]
    public GameObject panelRoot;
    public Button closeButton;
    public Button backFooterButton;
    public Text backFooterButtonText;
    public Text headerTitleText;

    [Header("Sélecteur Top 5 / Top 10")]
    public Button top5Button;
    public Button top10Button;

    [Header("Carte Podium Top 3 (index 0=#1, 1=#2, 2=#3)")]
    public GameObject podiumCardRoot;
    public Text podiumTagText;
    public PodiumSlotUI[] podiumSlots; // [0] = Rank 1 (center), [1] = Rank 2 (left), [2] = Rank 3 (right)

    [Header("Carte Classement Local (#4 à #10)")]
    public Text listTagText;
    public GameObject listHeaderRoot;
    public Text colHunterText;
    public Text colScoreText;
    public Text colDateText;
    public ScoreRowUI[] rankRows; // 7 lignes : #4, #5, #6, #7, #8, #9, #10
    public GameObject moreGamesHintRoot;
    public Text moreGamesHintText;

    [Header("État Vide (0 score enregistré)")]
    public GameObject emptyStateRoot;
    public Text emptyTitleText;
    public Text emptyDescText;

    [Header("Bandeau Profil Actif")]
    public Text activeProfileLabelText;
    public Text activePseudoText;
    public Text personalBestText;
    public Text personalRankText;

    private int currentTopCount = 5;
    private bool eventsBound = false;
    private AudioSource cachedClickAudioSource;

    // Palette Ndop
    private readonly Color COLOR_GOLD = new Color(0.961f, 0.722f, 0.180f, 1f);
    private readonly Color COLOR_DARK_TEXT = new Color(0.078f, 0.106f, 0.071f, 1f);
    private readonly Color COLOR_LIGHT_TEXT = new Color(0.957f, 0.929f, 0.878f, 1f);
    private readonly Color COLOR_DIM_TEXT = new Color(0.722f, 0.686f, 0.620f, 1f);
    private readonly Color COLOR_INNER_DARK = new Color(0.078f, 0.106f, 0.071f, 1f);
    private readonly Color COLOR_ME_BG = new Color(0.165f, 0.155f, 0.095f, 1f);
    private readonly Color COLOR_RANK1_BG = new Color(0.155f, 0.145f, 0.085f, 1f);
    private readonly Color COLOR_NORMAL_BORDER = new Color(0.282f, 0.400f, 0.220f, 0.55f);

    private void Awake()
    {
        if (panelRoot == null) panelRoot = gameObject;
    }

    private void OnEnable()
    {
        BindEvents();
        SettingsManager.OnSettingsChanged += RefreshLeaderboard;
        RefreshLeaderboard();
    }

    private void OnDisable()
    {
        SettingsManager.OnSettingsChanged -= RefreshLeaderboard;
    }

    private void BindEvents()
    {
        if (eventsBound) return;
        eventsBound = true;

        if (closeButton != null) closeButton.onClick.AddListener(ClosePanel);
        if (backFooterButton != null) backFooterButton.onClick.AddListener(ClosePanel);
        if (top5Button != null) top5Button.onClick.AddListener(() => SetTopCount(5));
        if (top10Button != null) top10Button.onClick.AddListener(() => SetTopCount(10));
    }

    private void PlayUIClick()
    {
        if (cachedClickAudioSource == null)
        {
            MainMenu mm = FindObjectOfType<MainMenu>();
            if (mm != null && mm.buttonAudioSource != null)
            {
                cachedClickAudioSource = mm.buttonAudioSource;
            }
        }
        if (cachedClickAudioSource == null) return;
        if (SettingsManager.Instance != null)
        {
            if (SettingsManager.Instance.isMuteGlobal) return;
            cachedClickAudioSource.volume = SettingsManager.Instance.sfxVolume;
        }
        cachedClickAudioSource.Play();
    }

    public void OpenPanel()
    {
        panelRoot.SetActive(true);
        RefreshLeaderboard();
    }

    public void ClosePanel()
    {
        PlayUIClick();
        panelRoot.SetActive(false);
    }

    public void SetTopCount(int count)
    {
        PlayUIClick();
        currentTopCount = (count == 10) ? 10 : 5;
        RefreshLeaderboard();
    }

    private void SetSegmentedVisual(Button btn, bool isActive)
    {
        if (btn == null) return;
        Image img = btn.GetComponent<Image>();
        if (img != null) img.color = isActive ? COLOR_GOLD : COLOR_INNER_DARK;
        Text txt = btn.GetComponentInChildren<Text>();
        if (txt != null) txt.color = isActive ? COLOR_DARK_TEXT : COLOR_LIGHT_TEXT;
    }

    private string FormatScore(int score)
    {
        return score.ToString("N0").Replace(",", " ").Replace("\u00A0", " ") + " PTS";
    }

    public void RefreshLeaderboard()
    {
        SettingsManager sm = SettingsManager.Instance;
        bool isFR = (sm == null || sm.currentLanguage == "FR");
        string activePseudo = (sm != null && !string.IsNullOrEmpty(sm.playerPseudo)) ? sm.playerPseudo : "Chasseur K-Mer";

        // 1. Textes localisés FR / EN
        if (headerTitleText != null)
            headerTitleText.text = isFR ? "CLASSEMENT CHASSEURS" : "HUNTERS LEADERBOARD";
        if (podiumTagText != null)
            podiumTagText.text = isFR ? "PODIUM ÉLITE" : "TOP 3 PODIUM";
        if (listTagText != null)
            listTagText.text = isFR ? $"CLASSEMENT LOCAL (TOP {currentTopCount})" : $"LOCAL RANKING (TOP {currentTopCount})";
        if (colHunterText != null)
            colHunterText.text = isFR ? "CHASSEUR" : "HUNTER";
        if (colScoreText != null)
            colScoreText.text = "SCORE";
        if (colDateText != null)
            colDateText.text = "DATE";
        if (activeProfileLabelText != null)
            activeProfileLabelText.text = isFR ? "PROFIL ACTIF (PARAMÈTRES)" : "ACTIVE PROFILE (SETTINGS)";
        if (activePseudoText != null)
            activePseudoText.text = activePseudo;
        if (backFooterButtonText != null)
            backFooterButtonText.text = isFR ? "RETOUR AU MENU" : "BACK TO MENU";

        SetSegmentedVisual(top5Button, currentTopCount == 5);
        SetSegmentedVisual(top10Button, currentTopCount == 10);

        // 2. Récupération des vrais scores enregistrés
        List<SettingsManager.ScoreEntry> allScores = sm != null ? sm.GetTopScores(10) : new List<SettingsManager.ScoreEntry>();
        bool hasAnyScore = allScores.Count > 0;

        // 3. Gestion de l'état vide (0 score / après Reset)
        if (!hasAnyScore)
        {
            if (podiumCardRoot != null) podiumCardRoot.SetActive(false);
            if (listHeaderRoot != null) listHeaderRoot.SetActive(false);
            if (moreGamesHintRoot != null) moreGamesHintRoot.SetActive(false);
            if (rankRows != null)
            {
                for (int i = 0; i < rankRows.Length; i++)
                {
                    if (rankRows[i] != null && rankRows[i].root != null)
                        rankRows[i].root.SetActive(false);
                }
            }
            if (emptyStateRoot != null) emptyStateRoot.SetActive(true);
            if (emptyTitleText != null)
                emptyTitleText.text = isFR ? "Aucun score enregistré" : "No scores recorded yet";
            if (emptyDescText != null)
                emptyDescText.text = isFR
                    ? $"Lance une partie pour entrer dans le classement sous le pseudo « {activePseudo} » !"
                    : $"Start a game to enter the leaderboard as \"{activePseudo}\"!";

            if (personalBestText != null) personalBestText.text = "0 PTS";
            if (personalRankText != null) personalRankText.text = isFR ? "Non classé" : "Unranked";
            return;
        }

        // 4. État peuplé : Podium Top 3
        if (emptyStateRoot != null) emptyStateRoot.SetActive(false);
        if (podiumCardRoot != null) podiumCardRoot.SetActive(true);
        if (listHeaderRoot != null) listHeaderRoot.SetActive(true);

        if (podiumSlots != null)
        {
            for (int i = 0; i < podiumSlots.Length && i < 3; i++)
            {
                PodiumSlotUI slot = podiumSlots[i];
                if (slot == null) continue;

                if (i < allScores.Count)
                {
                    SettingsManager.ScoreEntry entry = allScores[i];
                    bool isMe = string.Equals(entry.pseudo, activePseudo, System.StringComparison.OrdinalIgnoreCase);

                    if (slot.pseudoText != null) slot.pseudoText.text = entry.pseudo;
                    if (slot.scoreText != null) slot.scoreText.text = FormatScore(entry.score);
                    if (slot.dateText != null) slot.dateText.text = entry.date;

                    if (slot.bgImage != null)
                        slot.bgImage.color = isMe ? COLOR_ME_BG : (i == 0 ? COLOR_RANK1_BG : COLOR_INNER_DARK);
                    if (slot.borderImage != null)
                        slot.borderImage.color = (isMe || i == 0) ? COLOR_GOLD : COLOR_NORMAL_BORDER;
                }
                else
                {
                    if (slot.pseudoText != null) slot.pseudoText.text = "---";
                    if (slot.scoreText != null) slot.scoreText.text = "0 PTS";
                    if (slot.dateText != null) slot.dateText.text = "--/--";
                    if (slot.bgImage != null) slot.bgImage.color = COLOR_INNER_DARK;
                    if (slot.borderImage != null) slot.borderImage.color = COLOR_NORMAL_BORDER;
                }
            }
        }

        // 5. Lignes #4 à #10 (selon Top 5 ou Top 10)
        int maxRowsToShow = (currentTopCount == 5) ? 2 : 7; // Top 5 => rangs #4 et #5 ; Top 10 => rangs #4 à #10
        int visibleRowCount = 0;

        if (rankRows != null)
        {
            for (int i = 0; i < rankRows.Length; i++)
            {
                ScoreRowUI row = rankRows[i];
                if (row == null || row.root == null) continue;

                int scoreIndex = i + 3; // index 3 = rang #4
                bool shouldShow = (i < maxRowsToShow) && (scoreIndex < allScores.Count);
                row.root.SetActive(shouldShow);

                if (shouldShow)
                {
                    visibleRowCount++;
                    SettingsManager.ScoreEntry entry = allScores[scoreIndex];
                    bool isMe = string.Equals(entry.pseudo, activePseudo, System.StringComparison.OrdinalIgnoreCase);

                    if (row.rankText != null)
                    {
                        row.rankText.text = "#" + (scoreIndex + 1);
                        row.rankText.color = isMe ? COLOR_GOLD : COLOR_DIM_TEXT;
                    }
                    if (row.pseudoText != null) row.pseudoText.text = entry.pseudo;
                    if (row.meBadge != null) row.meBadge.SetActive(isMe);
                    if (row.meBadgeText != null) row.meBadgeText.text = isFR ? "TOI" : "YOU";
                    if (row.scoreText != null) row.scoreText.text = FormatScore(entry.score);
                    if (row.dateText != null) row.dateText.text = entry.date;

                    if (row.bgImage != null) row.bgImage.color = isMe ? COLOR_ME_BG : COLOR_INNER_DARK;
                    if (row.borderImage != null) row.borderImage.color = isMe ? COLOR_GOLD : COLOR_NORMAL_BORDER;
                }
            }
        }

        // Si on a 1 à 3 scores seulement (tous sur le podium), afficher un message discret dans la liste
        if (moreGamesHintRoot != null)
        {
            moreGamesHintRoot.SetActive(visibleRowCount == 0);
            if (moreGamesHintText != null)
            {
                moreGamesHintText.text = isFR
                    ? $"Joue d'autres parties pour remplir le classement Top {currentTopCount} !"
                    : $"Play more games to fill up the Top {currentTopCount} leaderboard!";
            }
        }

        // 6. Bandeau résumé du profil actif
        int bestPersonalScore = 0;
        int bestPersonalRank = -1;
        for (int i = 0; i < allScores.Count; i++)
        {
            if (string.Equals(allScores[i].pseudo, activePseudo, System.StringComparison.OrdinalIgnoreCase))
            {
                if (allScores[i].score > bestPersonalScore)
                {
                    bestPersonalScore = allScores[i].score;
                    bestPersonalRank = i + 1;
                }
            }
        }

        if (personalBestText != null)
            personalBestText.text = FormatScore(bestPersonalScore);
        if (personalRankText != null)
        {
            personalRankText.text = bestPersonalRank > 0
                ? (isFR ? $"Meilleur rang : #{bestPersonalRank}" : $"Best rank: #{bestPersonalRank}")
                : (isFR ? "Pas encore classé" : "Not ranked yet");
        }
    }
}
