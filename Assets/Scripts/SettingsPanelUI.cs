using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Contrôleur d'interface utilisateur pour le Panel des Paramètres de Eh-Oh-Mosquitoes.
/// Reproduction fidèle de la maquette carousel Ndop (5 onglets modulaires) avec liaison complète
/// en temps réel vers SettingsManager (audio multi-canaux, haptique, réticule ReticleLibrary,
/// compteur FPS, langue FR/EN, pseudo joueur et réinitialisation du meilleur score à 0).
/// </summary>
public class SettingsPanelUI : MonoBehaviour
{
    [Header("Structure Principale")]
    public GameObject panelRoot;
    public Button closeButton;
    public Button saveButton;
    public Text headerTitleText;

    [Header("Navigation par Onglets")]
    public Button[] tabButtons; // 0=Audio, 1=Controles, 2=Viseur, 3=Graphismes, 4=Systeme
    public GameObject[] tabContents;
    public Text[] tabButtonTexts;

    [Header("Onglet 1 : Audio")]
    public Slider masterSlider;
    public Text masterValueText;
    public Slider sfxSlider;
    public Text sfxValueText;
    public Slider bgmSlider;
    public Text bgmValueText;
    public Slider buzzSlider;
    public Text buzzValueText;
    public Button muteGlobalButton;
    public Image muteGlobalToggleBg;
    public RectTransform muteGlobalToggleKnob;

    [Header("Onglet 2 : Contrôles & Haptique")]
    public Button hapticBiteButton;
    public Image hapticBiteToggleBg;
    public RectTransform hapticBiteToggleKnob;
    public Button handednessRightButton;
    public Button handednessLeftButton;
    public Slider gyroSlider;
    public Text gyroValueText;

    [Header("Onglet 3 : Viseur & Radar 360°")]
    public Button[] reticleStyleButtons; // 0=Classique, 1=Ndop Guerrier, 2=Precision
    public Outline[] reticleStyleOutlines;
    public Button[] reticleColorButtons; // 0=Or, 1=Vert, 2=Rouge, 3=Blanc
    public Outline[] reticleColorOutlines;
    public Image reticlePreviewImage;
    public Sprite[] reticleSprites;
    public Button radarToggleButton;
    public Image radarToggleBg;
    public RectTransform radarToggleKnob;
    public Slider radarRadiusSlider;
    public Text radarRadiusValueText;

    [Header("Onglet 4 : Graphismes & Batterie AR")]
    public Button fps30Button;
    public Button fps60Button;
    public Button showFPSButton;
    public Image showFPSToggleBg;
    public RectTransform showFPSToggleKnob;

    [Header("Onglet 5 : Système & Langues")]
    public Button langFRButton;
    public Button langENButton;
    public InputField pseudoInput;
    public Text bestScoreValueText;
    public Button resetScoreButton;
    public Text resetScoreFeedbackText;
    public Text creditsText;

    private int currentTabIndex = 0;
    private AudioSource cachedClickAudioSource;
    private bool eventsBound = false;

    // Palette de couleurs Ndop
    private readonly Color COLOR_GOLD = new Color(0.96f, 0.72f, 0.18f, 1f);      // #F5B82E
    private readonly Color COLOR_DARK_TEXT = new Color(0.08f, 0.11f, 0.07f, 1f);  // #141B12
    private readonly Color COLOR_LIGHT_TEXT = new Color(0.96f, 0.93f, 0.88f, 1f); // #F4EDE0
    private readonly Color COLOR_ACTIVE_BG = new Color(0.96f, 0.72f, 0.18f, 1f);  // Gold
    private readonly Color COLOR_INACTIVE_BG = new Color(0.11f, 0.15f, 0.10f, 1f);// Dark Olive
    private readonly Color COLOR_TOGGLE_ON = new Color(0.25f, 0.36f, 0.20f, 1f);  // Green Olive
    private readonly Color COLOR_TOGGLE_OFF = new Color(0.11f, 0.15f, 0.10f, 1f); // Dark Olive
    private readonly Color COLOR_KNOB_ON = Color.white;
    private readonly Color COLOR_KNOB_OFF = new Color(0.55f, 0.48f, 0.35f, 1f);

    private void Awake()
    {
        if (panelRoot == null) panelRoot = gameObject;
    }

    private void Start()
    {
        BindEvents();
    }

    private void OnEnable()
    {
        BindEvents();
        ResolveClickAudioSource();
        SyncReticleSpritesFromLibrary();
        RefreshUIFromManager();
        SelectTab(currentTabIndex, playSound: false);
    }

    private void ResolveClickAudioSource()
    {
        if (cachedClickAudioSource != null) return;

        MainMenu mm = FindObjectOfType<MainMenu>();
        if (mm != null && mm.buttonAudioSource != null)
        {
            cachedClickAudioSource = mm.buttonAudioSource;
            return;
        }

        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null && gm.pauseAudioSource != null)
        {
            cachedClickAudioSource = gm.pauseAudioSource;
        }
    }

    private void PlayUIClick()
    {
        ResolveClickAudioSource();
        if (cachedClickAudioSource == null) return;
        if (SettingsManager.Instance != null)
        {
            if (SettingsManager.Instance.isMuteGlobal) return;
            cachedClickAudioSource.volume = SettingsManager.Instance.sfxVolume;
        }
        cachedClickAudioSource.Play();
    }

    private void SyncReticleSpritesFromLibrary()
    {
        if (SettingsManager.Instance == null) return;

        // Transmettre les sprites de secours au manager si besoin
        if (reticleSprites != null && reticleSprites.Length > 0)
        {
            SettingsManager.Instance.fallbackReticleSprites = reticleSprites;
        }

        // Mettre à jour les 3 vignettes de cartes avec ReticleLibrary (prioritaire) ou reticleSprites
        if (reticleStyleButtons != null)
        {
            for (int i = 0; i < reticleStyleButtons.Length; i++)
            {
                if (reticleStyleButtons[i] == null) continue;
                Sprite sp = SettingsManager.Instance.GetReticleSprite(i);
                if (sp == null && reticleSprites != null && i < reticleSprites.Length)
                {
                    sp = reticleSprites[i];
                }

                if (sp != null)
                {
                    Transform iconTr = reticleStyleButtons[i].transform.Find(reticleStyleButtons[i].name + "_Icon");
                    if (iconTr != null)
                    {
                        Image iconImg = iconTr.GetComponent<Image>();
                        if (iconImg != null)
                        {
                            iconImg.sprite = sp;
                            iconImg.preserveAspect = true;
                        }
                    }
                }
            }
        }
    }

    private void BindEvents()
    {
        if (eventsBound) return;
        eventsBound = true;

        if (closeButton != null) closeButton.onClick.AddListener(ClosePanel);
        if (saveButton != null) saveButton.onClick.AddListener(SaveAndClose);

        // Navigation onglets
        for (int i = 0; i < tabButtons.Length; i++)
        {
            int tabIndex = i;
            if (tabButtons[i] != null)
            {
                tabButtons[i].onClick.AddListener(() => SelectTab(tabIndex, playSound: true));
            }
        }

        // 1. Audio Sliders
        if (masterSlider != null) masterSlider.onValueChanged.AddListener(OnMasterSliderChanged);
        if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(OnSFXSliderChanged);
        if (bgmSlider != null) bgmSlider.onValueChanged.AddListener(OnBGMSliderChanged);
        if (buzzSlider != null) buzzSlider.onValueChanged.AddListener(OnBuzzSliderChanged);
        if (muteGlobalButton != null) muteGlobalButton.onClick.AddListener(ToggleMuteGlobal);

        // 2. Contrôles
        if (hapticBiteButton != null) hapticBiteButton.onClick.AddListener(ToggleHapticBite);
        if (handednessRightButton != null) handednessRightButton.onClick.AddListener(() => SetHandedness(false));
        if (handednessLeftButton != null) handednessLeftButton.onClick.AddListener(() => SetHandedness(true));
        if (gyroSlider != null) gyroSlider.onValueChanged.AddListener(OnGyroSliderChanged);

        // 3. Viseur & Radar
        for (int i = 0; i < reticleStyleButtons.Length; i++)
        {
            int rIndex = i;
            if (reticleStyleButtons[i] != null)
                reticleStyleButtons[i].onClick.AddListener(() => SetReticleStyle(rIndex));
        }

        for (int i = 0; i < reticleColorButtons.Length; i++)
        {
            int cIndex = i;
            if (reticleColorButtons[i] != null)
                reticleColorButtons[i].onClick.AddListener(() => SetReticleColor(cIndex));
        }

        if (radarToggleButton != null) radarToggleButton.onClick.AddListener(ToggleRadar);
        if (radarRadiusSlider != null) radarRadiusSlider.onValueChanged.AddListener(OnRadarRadiusChanged);

        // 4. Graphismes
        if (fps30Button != null) fps30Button.onClick.AddListener(() => SetTargetFPS(30));
        if (fps60Button != null) fps60Button.onClick.AddListener(() => SetTargetFPS(60));
        if (showFPSButton != null) showFPSButton.onClick.AddListener(ToggleShowFPS);

        // 5. Système
        if (langFRButton != null) langFRButton.onClick.AddListener(() => SetLanguage("FR"));
        if (langENButton != null) langENButton.onClick.AddListener(() => SetLanguage("EN"));
        if (pseudoInput != null) pseudoInput.onEndEdit.AddListener(OnPseudoChanged);
        if (resetScoreButton != null) resetScoreButton.onClick.AddListener(OnResetScoreClicked);
    }

    public void OpenPanel()
    {
        panelRoot.SetActive(true);
        SyncReticleSpritesFromLibrary();
        RefreshUIFromManager();
        SelectTab(0, playSound: false);
    }

    public void ClosePanel()
    {
        PlayUIClick();
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SaveAllSettings();
        }
        panelRoot.SetActive(false);
    }

    public void SaveAndClose()
    {
        PlayUIClick();
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SaveAllSettings();
        }
        panelRoot.SetActive(false);
    }

    public void SelectTab(int index, bool playSound = false)
    {
        if (playSound) PlayUIClick();
        currentTabIndex = index;
        for (int i = 0; i < tabContents.Length; i++)
        {
            if (tabContents[i] != null)
                tabContents[i].SetActive(i == index);
        }

        for (int i = 0; i < tabButtons.Length; i++)
        {
            if (tabButtons[i] != null)
            {
                SetSegmentedVisual(tabButtons[i], i == index);
            }
        }

        UpdateHeaderTitle();
    }

    private void UpdateHeaderTitle()
    {
        if (headerTitleText == null || SettingsManager.Instance == null) return;
        bool isFR = SettingsManager.Instance.currentLanguage == "FR";

        switch (currentTabIndex)
        {
            case 0:
                headerTitleText.text = isFR ? "PARAMÈTRES AUDIO" : "AUDIO SETTINGS";
                break;
            case 1:
                headerTitleText.text = isFR ? "CONTRÔLES & HAPTIQUE" : "CONTROLS & HAPTICS";
                break;
            case 2:
                headerTitleText.text = isFR ? "VISEUR & RADAR 360°" : "CROSSHAIR & 360° RADAR";
                break;
            case 3:
                headerTitleText.text = isFR ? "GRAPHISMES & BATTERIE AR" : "GRAPHICS & AR BATTERY";
                break;
            case 4:
                headerTitleText.text = isFR ? "SYSTÈME & LANGUES" : "SYSTEM & LANGUAGES";
                break;
            default:
                headerTitleText.text = isFR ? "PARAMÈTRES" : "SETTINGS";
                break;
        }
    }

    public void RefreshUIFromManager()
    {
        if (SettingsManager.Instance == null) return;
        SettingsManager sm = SettingsManager.Instance;

        // 1. Audio
        if (masterSlider != null) masterSlider.SetValueWithoutNotify(sm.masterVolume);
        if (masterValueText != null) masterValueText.text = Mathf.RoundToInt(sm.masterVolume * 100f) + "%";

        if (sfxSlider != null) sfxSlider.SetValueWithoutNotify(sm.sfxVolume);
        if (sfxValueText != null) sfxValueText.text = Mathf.RoundToInt(sm.sfxVolume * 100f) + "%";

        if (bgmSlider != null) bgmSlider.SetValueWithoutNotify(sm.bgmVolume);
        if (bgmValueText != null) bgmValueText.text = Mathf.RoundToInt(sm.bgmVolume * 100f) + "%";

        if (buzzSlider != null) buzzSlider.SetValueWithoutNotify(sm.mosquitoBuzzVolume);
        if (buzzValueText != null) buzzValueText.text = Mathf.RoundToInt(sm.mosquitoBuzzVolume * 100f) + "%";

        UpdateToggleSwitch(muteGlobalToggleBg, muteGlobalToggleKnob, sm.isMuteGlobal);

        // 2. Contrôles
        UpdateToggleSwitch(hapticBiteToggleBg, hapticBiteToggleKnob, sm.hapticBiteEnabled);
        SetSegmentedVisual(handednessRightButton, !sm.isLeftHanded);
        SetSegmentedVisual(handednessLeftButton, sm.isLeftHanded);

        if (gyroSlider != null) gyroSlider.SetValueWithoutNotify(sm.gyroSensitivity);
        if (gyroValueText != null) gyroValueText.text = sm.gyroSensitivity.ToString("F1") + "x" + (Mathf.Approximately(sm.gyroSensitivity, 1.0f) ? " (Défaut)" : "");

        // 3. Viseur & Radar
        for (int i = 0; i < reticleStyleButtons.Length; i++)
        {
            if (reticleStyleOutlines != null && i < reticleStyleOutlines.Length && reticleStyleOutlines[i] != null)
            {
                reticleStyleOutlines[i].enabled = (i == sm.reticleIndex);
                reticleStyleOutlines[i].effectColor = COLOR_GOLD;
            }
        }

        for (int i = 0; i < reticleColorButtons.Length; i++)
        {
            if (reticleColorOutlines != null && i < reticleColorOutlines.Length && reticleColorOutlines[i] != null)
            {
                reticleColorOutlines[i].enabled = (i == sm.reticleColorIndex);
                reticleColorOutlines[i].effectColor = Color.white;
            }
        }

        UpdateReticlePreview();
        UpdateToggleSwitch(radarToggleBg, radarToggleKnob, sm.radarEnabled);
        if (radarRadiusSlider != null) radarRadiusSlider.SetValueWithoutNotify(sm.radarRadius);
        if (radarRadiusValueText != null) radarRadiusValueText.text = Mathf.RoundToInt(sm.radarRadius) + " px";

        // 4. Graphismes
        SetSegmentedVisual(fps30Button, sm.targetFPS == 30);
        SetSegmentedVisual(fps60Button, sm.targetFPS == 60);
        UpdateToggleSwitch(showFPSToggleBg, showFPSToggleKnob, sm.showFPSCounter);

        // 5. Système
        SetSegmentedVisual(langFRButton, sm.currentLanguage == "FR");
        SetSegmentedVisual(langENButton, sm.currentLanguage == "EN");
        if (pseudoInput != null) pseudoInput.text = sm.playerPseudo;
        if (bestScoreValueText != null) bestScoreValueText.text = FormatScore(sm.GetBestScore());
        if (resetScoreFeedbackText != null) resetScoreFeedbackText.text = "";

        UpdateLanguageTexts();
        UpdateHeaderTitle();
    }

    private string FormatScore(int score)
    {
        return score.ToString("N0").Replace(",", " ").Replace("\u00A0", " ") + " PTS";
    }

    private void SetSegmentedVisual(Button btn, bool isActive)
    {
        if (btn == null) return;
        Image img = btn.GetComponent<Image>();
        if (img != null)
        {
            img.color = isActive ? COLOR_ACTIVE_BG : COLOR_INACTIVE_BG;
        }

        Text txt = btn.GetComponentInChildren<Text>();
        if (txt != null)
        {
            txt.color = isActive ? COLOR_DARK_TEXT : COLOR_LIGHT_TEXT;
        }
    }

    private void UpdateToggleSwitch(Image bg, RectTransform knob, bool isOn)
    {
        if (bg != null)
        {
            bg.color = isOn ? COLOR_TOGGLE_ON : COLOR_TOGGLE_OFF;
        }
        if (knob != null)
        {
            knob.anchoredPosition = new Vector2(isOn ? 24f : -24f, 0f);
            Image knobImg = knob.GetComponent<Image>();
            if (knobImg != null)
            {
                knobImg.color = isOn ? COLOR_KNOB_ON : COLOR_KNOB_OFF;
            }
        }
    }

    private void OnMasterSliderChanged(float val)
    {
        if (SettingsManager.Instance == null) return;
        SettingsManager.Instance.masterVolume = val;
        SettingsManager.Instance.ApplyAudioSettings();
        if (masterValueText != null) masterValueText.text = Mathf.RoundToInt(val * 100f) + "%";
    }

    private void OnSFXSliderChanged(float val)
    {
        if (SettingsManager.Instance == null) return;
        SettingsManager.Instance.sfxVolume = val;
        SettingsManager.Instance.ApplyAudioSettings();
        if (sfxValueText != null) sfxValueText.text = Mathf.RoundToInt(val * 100f) + "%";
    }

    private void OnBGMSliderChanged(float val)
    {
        if (SettingsManager.Instance == null) return;
        SettingsManager.Instance.bgmVolume = val;
        SettingsManager.Instance.ApplyAudioSettings();
        if (bgmValueText != null) bgmValueText.text = Mathf.RoundToInt(val * 100f) + "%";
    }

    private void OnBuzzSliderChanged(float val)
    {
        if (SettingsManager.Instance == null) return;
        SettingsManager.Instance.mosquitoBuzzVolume = val;
        SettingsManager.Instance.ApplyAudioSettings();
        if (buzzValueText != null) buzzValueText.text = Mathf.RoundToInt(val * 100f) + "%";
    }

    private void ToggleMuteGlobal()
    {
        if (SettingsManager.Instance == null) return;
        SettingsManager.Instance.isMuteGlobal = !SettingsManager.Instance.isMuteGlobal;
        SettingsManager.Instance.ApplyAudioSettings();
        SettingsManager.Instance.SaveAllSettings();
        UpdateToggleSwitch(muteGlobalToggleBg, muteGlobalToggleKnob, SettingsManager.Instance.isMuteGlobal);
        if (!SettingsManager.Instance.isMuteGlobal)
        {
            PlayUIClick();
        }
    }

    private void ToggleHapticBite()
    {
        PlayUIClick();
        if (SettingsManager.Instance == null) return;
        SettingsManager.Instance.hapticBiteEnabled = !SettingsManager.Instance.hapticBiteEnabled;
        UpdateToggleSwitch(hapticBiteToggleBg, hapticBiteToggleKnob, SettingsManager.Instance.hapticBiteEnabled);
        SettingsManager.Instance.NotifySettingsChanged();
    }

    private void SetHandedness(bool leftHanded)
    {
        PlayUIClick();
        if (SettingsManager.Instance == null) return;
        SettingsManager.Instance.isLeftHanded = leftHanded;
        SetSegmentedVisual(handednessRightButton, !leftHanded);
        SetSegmentedVisual(handednessLeftButton, leftHanded);
        SettingsManager.Instance.NotifySettingsChanged();

        PlayerActions pa = FindObjectOfType<PlayerActions>();
        if (pa != null) pa.ApplyHandedness();
    }

    private void OnGyroSliderChanged(float val)
    {
        if (SettingsManager.Instance == null) return;
        SettingsManager.Instance.gyroSensitivity = val;
        if (gyroValueText != null) gyroValueText.text = val.ToString("F1") + "x" + (Mathf.Approximately(val, 1.0f) ? " (Défaut)" : "");
        SettingsManager.Instance.NotifySettingsChanged();
    }

    private void SetReticleStyle(int index)
    {
        PlayUIClick();
        if (SettingsManager.Instance == null) return;
        SettingsManager.Instance.reticleIndex = index;

        for (int i = 0; i < reticleStyleButtons.Length; i++)
        {
            if (reticleStyleOutlines != null && i < reticleStyleOutlines.Length && reticleStyleOutlines[i] != null)
                reticleStyleOutlines[i].enabled = (i == index);
        }

        UpdateReticlePreview();
        SettingsManager.Instance.NotifySettingsChanged();
    }

    private void SetReticleColor(int index)
    {
        PlayUIClick();
        if (SettingsManager.Instance == null) return;
        SettingsManager.Instance.reticleColorIndex = index;

        for (int i = 0; i < reticleColorButtons.Length; i++)
        {
            if (reticleColorOutlines != null && i < reticleColorOutlines.Length && reticleColorOutlines[i] != null)
                reticleColorOutlines[i].enabled = (i == index);
        }

        UpdateReticlePreview();
        SettingsManager.Instance.NotifySettingsChanged();
    }

    private void UpdateReticlePreview()
    {
        if (SettingsManager.Instance == null) return;
        if (reticlePreviewImage != null)
        {
            Sprite chosen = SettingsManager.Instance.GetReticleSprite(SettingsManager.Instance.reticleIndex);
            if (chosen == null && reticleSprites != null && reticleSprites.Length > SettingsManager.Instance.reticleIndex)
            {
                chosen = reticleSprites[SettingsManager.Instance.reticleIndex];
            }
            if (chosen != null)
            {
                reticlePreviewImage.sprite = chosen;
                reticlePreviewImage.preserveAspect = true;
            }
            reticlePreviewImage.color = SettingsManager.Instance.GetReticleColor();
        }
    }

    private void ToggleRadar()
    {
        PlayUIClick();
        if (SettingsManager.Instance == null) return;
        SettingsManager.Instance.radarEnabled = !SettingsManager.Instance.radarEnabled;
        UpdateToggleSwitch(radarToggleBg, radarToggleKnob, SettingsManager.Instance.radarEnabled);
        SettingsManager.Instance.NotifySettingsChanged();
    }

    private void OnRadarRadiusChanged(float val)
    {
        if (SettingsManager.Instance == null) return;
        SettingsManager.Instance.radarRadius = val;
        if (radarRadiusValueText != null) radarRadiusValueText.text = Mathf.RoundToInt(val) + " px";
        SettingsManager.Instance.NotifySettingsChanged();
    }

    private void SetTargetFPS(int fps)
    {
        PlayUIClick();
        if (SettingsManager.Instance == null) return;
        SettingsManager.Instance.targetFPS = fps;
        SettingsManager.Instance.ApplyGraphicSettings();
        SetSegmentedVisual(fps30Button, fps == 30);
        SetSegmentedVisual(fps60Button, fps == 60);
    }

    private void ToggleShowFPS()
    {
        PlayUIClick();
        if (SettingsManager.Instance == null) return;
        SettingsManager.Instance.showFPSCounter = !SettingsManager.Instance.showFPSCounter;
        UpdateToggleSwitch(showFPSToggleBg, showFPSToggleKnob, SettingsManager.Instance.showFPSCounter);
        SettingsManager.Instance.NotifySettingsChanged();
    }

    private void SetLanguage(string lang)
    {
        PlayUIClick();
        if (SettingsManager.Instance == null) return;
        SettingsManager.Instance.currentLanguage = lang;
        SetSegmentedVisual(langFRButton, lang == "FR");
        SetSegmentedVisual(langENButton, lang == "EN");
        UpdateLanguageTexts();
        UpdateHeaderTitle();
        SettingsManager.Instance.NotifySettingsChanged();
    }

    private void OnPseudoChanged(string newPseudo)
    {
        if (SettingsManager.Instance == null) return;
        if (!string.IsNullOrEmpty(newPseudo))
        {
            SettingsManager.Instance.playerPseudo = newPseudo.Trim();
            SettingsManager.Instance.SaveAllSettings();
        }
    }

    private void OnResetScoreClicked()
    {
        PlayUIClick();
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.ResetBestScore();
            if (bestScoreValueText != null)
            {
                bestScoreValueText.text = FormatScore(SettingsManager.Instance.GetBestScore());
            }
            if (resetScoreFeedbackText != null)
            {
                resetScoreFeedbackText.text = SettingsManager.Instance.currentLanguage == "FR"
                    ? "Meilleur score remis à 0 !"
                    : "Best score reset to 0!";
            }
        }
    }

    private void UpdateLanguageTexts()
    {
        if (SettingsManager.Instance == null) return;
        bool isFR = SettingsManager.Instance.currentLanguage == "FR";

        if (tabButtonTexts != null && tabButtonTexts.Length >= 5)
        {
            tabButtonTexts[0].text = isFR ? "Audio" : "Audio";
            tabButtonTexts[1].text = isFR ? "Contrôles" : "Controls";
            tabButtonTexts[2].text = isFR ? "Viseur" : "Crosshair";
            tabButtonTexts[3].text = isFR ? "Graphismes" : "Graphics";
            tabButtonTexts[4].text = isFR ? "Système" : "System";
        }

        if (saveButton != null)
        {
            Text saveText = saveButton.GetComponentInChildren<Text>();
            if (saveText != null)
                saveText.text = isFR ? "ENREGISTRER & RETOUR" : "SAVE & CLOSE";
        }

        if (creditsText != null)
        {
            creditsText.text = isFR
                ? "Développeur : Camatodev\nÉditeur : Vortex Makers x K-Mer Play\nMoteur : Unity 2022.3 LTS - Vuforia Engine AR\nVersion : v1.2 (Festival Pousse-Pions Build)"
                : "Developer: Camatodev\nPublisher: Vortex Makers x K-Mer Play\nEngine: Unity 2022.3 LTS - Vuforia Engine AR\nVersion: v1.2 (Festival Pousse-Pions Build)";
        }
    }
}
