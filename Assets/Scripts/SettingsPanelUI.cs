using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Contrôleur d'interface utilisateur pour le Panel des Paramètres de Eh-Oh-Mosquitoes.
/// Gère la navigation par onglets modulaires, les curseurs audio, le retour tactile,
/// le choix du réticule Ndop, le framerate (30/60 FPS) et le bilinguisme Français/Anglais.
/// </summary>
public class SettingsPanelUI : MonoBehaviour
{
    [Header("Panneau Principal")]
    public GameObject panelRoot;
    public Button closeButton;
    public Button saveButton;
    public Text headerTitleText;

    [Header("Onglets de Navigation")]
    public Button[] tabButtons;
    public GameObject[] tabContents;
    public Text[] tabButtonTexts;

    [Header("Onglet Audio")]
    public Slider masterSlider;
    public Text masterValueText;
    public Slider sfxSlider;
    public Text sfxValueText;
    public Slider bgmSlider;
    public Text bgmValueText;
    public Slider buzzSlider;
    public Text buzzValueText;
    public Button muteGlobalButton;
    public Text muteGlobalStatusText;

    [Header("Onglet Contrôles & Haptique")]
    public Button hapticBiteButton;
    public Text hapticBiteStatusText;
    public Button handednessButton;
    public Text handednessStatusText;
    public Slider gyroSlider;
    public Text gyroValueText;

    [Header("Onglet Viseur & Radar")]
    public Button[] reticleStyleButtons;
    public Button[] reticleColorButtons;
    public Button radarToggleButton;
    public Text radarToggleStatusText;
    public Slider radarRadiusSlider;
    public Text radarRadiusValueText;
    public Image reticlePreviewImage;
    public Sprite[] reticleSprites; // 0=Classique, 1=Ndop, 2=Précision

    [Header("Onglet Graphismes")]
    public Button fps30Button;
    public Button fps60Button;
    public Text fpsModeStatusText;
    public Button showFPSButton;
    public Text showFPSStatusText;

    [Header("Onglet Système")]
    public Button langFRButton;
    public Button langENButton;
    public Button resetScoreButton;
    public Text resetScoreFeedbackText;
    public Text creditsText;

    private int currentTabIndex = 0;

    void Awake()
    {
        if (panelRoot == null) panelRoot = gameObject;
    }

    void Start()
    {
        BindEvents();
    }

    void OnEnable()
    {
        RefreshUIFromManager();
        SelectTab(currentTabIndex);
    }

    void BindEvents()
    {
        if (closeButton != null) closeButton.onClick.AddListener(ClosePanel);
        if (saveButton != null) saveButton.onClick.AddListener(SaveAndClose);

        // Onglets
        for (int i = 0; i < tabButtons.Length; i++)
        {
            int tabIndex = i;
            if (tabButtons[i] != null)
            {
                tabButtons[i].onClick.AddListener(() => SelectTab(tabIndex));
            }
        }

        // Audio Sliders
        if (masterSlider != null) masterSlider.onValueChanged.AddListener(OnMasterSliderChanged);
        if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(OnSFXSliderChanged);
        if (bgmSlider != null) bgmSlider.onValueChanged.AddListener(OnBGMSliderChanged);
        if (buzzSlider != null) buzzSlider.onValueChanged.AddListener(OnBuzzSliderChanged);
        if (muteGlobalButton != null) muteGlobalButton.onClick.AddListener(ToggleMuteGlobal);

        // Contrôles
        if (hapticBiteButton != null) hapticBiteButton.onClick.AddListener(ToggleHapticBite);
        if (handednessButton != null) handednessButton.onClick.AddListener(ToggleHandedness);
        if (gyroSlider != null) gyroSlider.onValueChanged.AddListener(OnGyroSliderChanged);

        // Viseur & Radar
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

        // Graphismes
        if (fps30Button != null) fps30Button.onClick.AddListener(() => SetTargetFPS(30));
        if (fps60Button != null) fps60Button.onClick.AddListener(() => SetTargetFPS(60));
        if (showFPSButton != null) showFPSButton.onClick.AddListener(ToggleShowFPS);

        // Système
        if (langFRButton != null) langFRButton.onClick.AddListener(() => SetLanguage("FR"));
        if (langENButton != null) langENButton.onClick.AddListener(() => SetLanguage("EN"));
        if (resetScoreButton != null) resetScoreButton.onClick.AddListener(OnResetScoreClicked);
    }

    public void OpenPanel()
    {
        panelRoot.SetActive(true);
        RefreshUIFromManager();
        SelectTab(0);
    }

    public void ClosePanel()
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SaveAllSettings();
        }
        panelRoot.SetActive(false);
    }

    public void SaveAndClose()
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SaveAllSettings();
        }
        ClosePanel();
    }

    public void SelectTab(int index)
    {
        currentTabIndex = index;
        for (int i = 0; i < tabContents.Length; i++)
        {
            if (tabContents[i] != null)
            {
                tabContents[i].SetActive(i == index);
            }
        }

        // Style actif sur les boutons d'onglet
        for (int i = 0; i < tabButtons.Length; i++)
        {
            if (tabButtons[i] != null)
            {
                ColorBlock cb = tabButtons[i].colors;
                cb.normalColor = (i == index) ? new Color(0.96f, 0.72f, 0.18f, 1f) : new Color(0.14f, 0.20f, 0.12f, 0.8f);
                tabButtons[i].colors = cb;
            }
        }
    }

    public void RefreshUIFromManager()
    {
        if (SettingsManager.Instance == null) return;
        SettingsManager sm = SettingsManager.Instance;

        // Audio
        if (masterSlider != null) masterSlider.value = sm.masterVolume;
        if (masterValueText != null) masterValueText.text = Mathf.RoundToInt(sm.masterVolume * 100f) + "%";

        if (sfxSlider != null) sfxSlider.value = sm.sfxVolume;
        if (sfxValueText != null) sfxValueText.text = Mathf.RoundToInt(sm.sfxVolume * 100f) + "%";

        if (bgmSlider != null) bgmSlider.value = sm.bgmVolume;
        if (bgmValueText != null) bgmValueText.text = Mathf.RoundToInt(sm.bgmVolume * 100f) + "%";

        if (buzzSlider != null) buzzSlider.value = sm.mosquitoBuzzVolume;
        if (buzzValueText != null) buzzValueText.text = Mathf.RoundToInt(sm.mosquitoBuzzVolume * 100f) + "%";

        if (muteGlobalStatusText != null)
            muteGlobalStatusText.text = sm.isMuteGlobal ? (sm.currentLanguage == "FR" ? "ACTIVÉ" : "MUTED") : (sm.currentLanguage == "FR" ? "SON ACTIF" : "AUDIO ON");

        // Contrôles
        if (hapticBiteStatusText != null)
            hapticBiteStatusText.text = sm.hapticBiteEnabled ? (sm.currentLanguage == "FR" ? "ACTIVÉ (PIQÛRES)" : "ENABLED (BITES)") : (sm.currentLanguage == "FR" ? "DÉSACTIVÉ" : "DISABLED");

        if (handednessStatusText != null)
            handednessStatusText.text = sm.isLeftHanded ? (sm.currentLanguage == "FR" ? "MODE GAUCHER" : "LEFT-HANDED") : (sm.currentLanguage == "FR" ? "MODE DROITIER" : "RIGHT-HANDED");

        if (gyroSlider != null) gyroSlider.value = sm.gyroSensitivity;
        if (gyroValueText != null) gyroValueText.text = sm.gyroSensitivity.ToString("F1") + "x";

        // Viseur & Radar
        UpdateReticlePreview();
        if (radarToggleStatusText != null)
            radarToggleStatusText.text = sm.radarEnabled ? (sm.currentLanguage == "FR" ? "RADAR VISIBLE" : "RADAR ON") : (sm.currentLanguage == "FR" ? "MASQUÉ" : "HIDDEN");

        if (radarRadiusSlider != null) radarRadiusSlider.value = sm.radarRadius;
        if (radarRadiusValueText != null) radarRadiusValueText.text = Mathf.RoundToInt(sm.radarRadius) + " px";

        // Graphismes
        if (fpsModeStatusText != null)
            fpsModeStatusText.text = sm.targetFPS == 30 ? (sm.currentLanguage == "FR" ? "30 FPS (ÉCO BATTERIE)" : "30 FPS (BATTERY SAVER)") : (sm.currentLanguage == "FR" ? "60 FPS (QUALITÉ FLUIDE)" : "60 FPS (SMOOTH)");

        if (showFPSStatusText != null)
            showFPSStatusText.text = sm.showFPSCounter ? (sm.currentLanguage == "FR" ? "AFFICHÉ" : "VISIBLE") : (sm.currentLanguage == "FR" ? "MASQUÉ" : "HIDDEN");

        // Langue & Textes
        UpdateLanguageTexts();
    }

    void OnMasterSliderChanged(float val)
    {
        if (SettingsManager.Instance == null) return;
        SettingsManager.Instance.masterVolume = val;
        SettingsManager.Instance.ApplyAudioSettings();
        if (masterValueText != null) masterValueText.text = Mathf.RoundToInt(val * 100f) + "%";
    }

    void OnSFXSliderChanged(float val)
    {
        if (SettingsManager.Instance == null) return;
        SettingsManager.Instance.sfxVolume = val;
        if (sfxValueText != null) sfxValueText.text = Mathf.RoundToInt(val * 100f) + "%";
    }

    void OnBGMSliderChanged(float val)
    {
        if (SettingsManager.Instance == null) return;
        SettingsManager.Instance.bgmVolume = val;
        if (bgmValueText != null) bgmValueText.text = Mathf.RoundToInt(val * 100f) + "%";
    }

    void OnBuzzSliderChanged(float val)
    {
        if (SettingsManager.Instance == null) return;
        SettingsManager.Instance.mosquitoBuzzVolume = val;
        if (buzzValueText != null) buzzValueText.text = Mathf.RoundToInt(val * 100f) + "%";
    }

    void ToggleMuteGlobal()
    {
        if (SettingsManager.Instance == null) return;
        SettingsManager.Instance.isMuteGlobal = !SettingsManager.Instance.isMuteGlobal;
        SettingsManager.Instance.ApplyAudioSettings();
        RefreshUIFromManager();
    }

    void ToggleHapticBite()
    {
        if (SettingsManager.Instance == null) return;
        SettingsManager.Instance.hapticBiteEnabled = !SettingsManager.Instance.hapticBiteEnabled;
        RefreshUIFromManager();
    }

    void ToggleHandedness()
    {
        if (SettingsManager.Instance == null) return;
        SettingsManager.Instance.isLeftHanded = !SettingsManager.Instance.isLeftHanded;
        RefreshUIFromManager();
        PlayerActions pa = FindObjectOfType<PlayerActions>();
        if (pa != null) pa.ApplyHandedness();
    }

    void OnGyroSliderChanged(float val)
    {
        if (SettingsManager.Instance == null) return;
        SettingsManager.Instance.gyroSensitivity = val;
        if (gyroValueText != null) gyroValueText.text = val.ToString("F1") + "x";
    }

    void SetReticleStyle(int index)
    {
        if (SettingsManager.Instance == null) return;
        SettingsManager.Instance.reticleIndex = index;
        UpdateReticlePreview();
    }

    void SetReticleColor(int index)
    {
        if (SettingsManager.Instance == null) return;
        SettingsManager.Instance.reticleColorIndex = index;
        UpdateReticlePreview();
    }

    void UpdateReticlePreview()
    {
        if (SettingsManager.Instance == null) return;
        if (reticlePreviewImage != null && reticleSprites != null && reticleSprites.Length > SettingsManager.Instance.reticleIndex)
        {
            reticlePreviewImage.sprite = reticleSprites[SettingsManager.Instance.reticleIndex];
            reticlePreviewImage.color = SettingsManager.Instance.GetReticleColor();
        }
    }

    void ToggleRadar()
    {
        if (SettingsManager.Instance == null) return;
        SettingsManager.Instance.radarEnabled = !SettingsManager.Instance.radarEnabled;
        RefreshUIFromManager();
    }

    void OnRadarRadiusChanged(float val)
    {
        if (SettingsManager.Instance == null) return;
        SettingsManager.Instance.radarRadius = val;
        if (radarRadiusValueText != null) radarRadiusValueText.text = Mathf.RoundToInt(val) + " px";
    }

    void SetTargetFPS(int fps)
    {
        if (SettingsManager.Instance == null) return;
        SettingsManager.Instance.targetFPS = fps;
        SettingsManager.Instance.ApplyGraphicSettings();
        RefreshUIFromManager();
    }

    void ToggleShowFPS()
    {
        if (SettingsManager.Instance == null) return;
        SettingsManager.Instance.showFPSCounter = !SettingsManager.Instance.showFPSCounter;
        RefreshUIFromManager();
    }

    void SetLanguage(string lang)
    {
        if (SettingsManager.Instance == null) return;
        SettingsManager.Instance.currentLanguage = lang;
        RefreshUIFromManager();
    }

    void OnResetScoreClicked()
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.ResetBestScore();
            if (resetScoreFeedbackText != null)
            {
                resetScoreFeedbackText.text = SettingsManager.Instance.currentLanguage == "FR" ? "Scores réinitialisés !" : "Scores reset!";
            }
        }
    }

    void UpdateLanguageTexts()
    {
        if (SettingsManager.Instance == null) return;
        bool isFR = SettingsManager.Instance.currentLanguage == "FR";

        if (headerTitleText != null)
            headerTitleText.text = isFR ? "⚙️ PARAMÈTRES" : "⚙️ SETTINGS";

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
                ? "Développeur : Camatodev\nÉditeur : Vortex Makers × K-Mer Play\nMoteur : Unity 2022.3 LTS • Vuforia AR\nVersion : v1.2"
                : "Developer: Camatodev\nPublisher: Vortex Makers × K-Mer Play\nEngine: Unity 2022.3 LTS • Vuforia AR\nVersion: v1.2";
        }
    }
}
