using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Contrôleur du réticule de visée en jeu (attaché au GameObject Focus dans le niveau).
/// Applique automatiquement le sprite et la couleur choisis dans les Paramètres,
/// en conservant les proportions exactes (preserveAspect = true) même lorsque
/// vous remplacez les images dans Assets/Resources/ReticleLibrary.asset ou SettingsPanelUI.
/// </summary>
public class ReticleHUDController : MonoBehaviour
{
    [Header("Composant Image du Viseur")]
    public Image targetImage;

    [Header("Sprites de Secours (optionnel si ReticleLibrary est présent)")]
    public Sprite[] fallbackReticleSprites;

    private Vector2 baseSizeDelta;
    private bool baseSizeCaptured = false;

    private void Awake()
    {
        if (targetImage == null)
        {
            targetImage = GetComponent<Image>();
        }
        CaptureBaseSize();
    }

    private void OnEnable()
    {
        SettingsManager.OnSettingsChanged += ApplyReticleFromSettings;
        ApplyReticleFromSettings();
    }

    private void Start()
    {
        ApplyReticleFromSettings();
    }

    private void OnDisable()
    {
        SettingsManager.OnSettingsChanged -= ApplyReticleFromSettings;
    }

    private void CaptureBaseSize()
    {
        if (baseSizeCaptured) return;
        RectTransform rt = GetComponent<RectTransform>();
        if (rt != null)
        {
            baseSizeDelta = rt.sizeDelta;
            baseSizeCaptured = true;
        }
    }

    public void ApplyReticleFromSettings()
    {
        if (targetImage == null) targetImage = GetComponent<Image>();
        if (targetImage == null || SettingsManager.Instance == null) return;

        CaptureBaseSize();
        SettingsManager sm = SettingsManager.Instance;
        int idx = sm.reticleIndex;

        // 1. Priorité : bibliothèque / sprites synchronisés depuis SettingsManager (ReticleLibrary ou SettingsPanelUI)
        Sprite chosenSprite = sm.GetReticleSprite(idx);

        // 2. Secours : sprites locaux assignés sur ce composant
        if (chosenSprite == null && fallbackReticleSprites != null && fallbackReticleSprites.Length > 0)
        {
            int safeIdx = Mathf.Clamp(idx, 0, fallbackReticleSprites.Length - 1);
            chosenSprite = fallbackReticleSprites[safeIdx];
        }

        if (chosenSprite != null)
        {
            targetImage.sprite = chosenSprite;
        }

        // Toujours préserver les proportions exactes de l'image personnalisée et ne pas bloquer les clics
        targetImage.preserveAspect = true;
        targetImage.raycastTarget = false;
        targetImage.color = sm.GetReticleColor();

        // Appliquer le multiplicateur d'échelle optionnel de ReticleLibrary
        if (baseSizeCaptured)
        {
            RectTransform rt = GetComponent<RectTransform>();
            if (rt != null)
            {
                float mult = sm.GetReticleScaleMultiplier(idx);
                rt.sizeDelta = baseSizeDelta * mult;
            }
        }
    }
}
