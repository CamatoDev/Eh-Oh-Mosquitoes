using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Compteur de performances (FPS) discret dans la direction artistique Ndop,
/// positionné légèrement au-dessus de la barre de vie dans la scène de jeu.
/// S'affiche ou se masque dynamiquement selon SettingsManager.Instance.showFPSCounter.
/// </summary>
public class FPSCounterUI : MonoBehaviour
{
    [Header("Références UI")]
    public GameObject visualContainer;
    public Text fpsText;

    [Header("Rafraîchissement")]
    [Tooltip("Intervalle de mise à jour de l'affichage en secondes")]
    public float updateInterval = 0.25f;

    private float accumulatedTime = 0f;
    private int frameCount = 0;
    private float timer = 0f;

    // Palette Ndop dynamique selon la fluidité
    private readonly Color COLOR_GOOD = new Color(0.961f, 0.722f, 0.180f, 1f);  // Or africain (#F5B82E) >= 45 FPS
    private readonly Color COLOR_MID  = new Color(0.941f, 0.541f, 0.141f, 1f);  // Ambre / Orange (25-44 FPS)
    private readonly Color COLOR_LOW  = new Color(0.878f, 0.271f, 0.220f, 1f);  // Rouge alerte (< 25 FPS)

    private void Awake()
    {
        if (visualContainer == null)
        {
            visualContainer = gameObject;
        }
        if (fpsText == null)
        {
            fpsText = GetComponentInChildren<Text>(true);
        }
    }

    private void OnEnable()
    {
        SettingsManager.OnSettingsChanged += RefreshVisibility;
        RefreshVisibility();
    }

    private void Start()
    {
        RefreshVisibility();
    }

    private void OnDisable()
    {
        SettingsManager.OnSettingsChanged -= RefreshVisibility;
    }

    public void RefreshVisibility()
    {
        bool shouldShow = SettingsManager.Instance != null && SettingsManager.Instance.showFPSCounter;

        // Si visualContainer est un enfant séparé, on le toggle ; sinon on toggle les Graphics pour que ce script reste abonné !
        if (visualContainer != null && visualContainer != gameObject)
        {
            visualContainer.SetActive(shouldShow);
        }
        else
        {
            Graphic[] graphics = GetComponentsInChildren<Graphic>(true);
            for (int i = 0; i < graphics.Length; i++)
            {
                graphics[i].enabled = shouldShow;
            }
        }
    }

    private void Update()
    {
        if (SettingsManager.Instance == null || !SettingsManager.Instance.showFPSCounter)
        {
            return;
        }

        float dt = Time.unscaledDeltaTime;
        if (dt <= 0.0001f) return;

        accumulatedTime += dt;
        frameCount++;
        timer += dt;

        if (timer >= updateInterval)
        {
            int fps = Mathf.RoundToInt(frameCount / accumulatedTime);
            if (fpsText != null)
            {
                fpsText.text = fps + " FPS";
                if (fps >= 45)
                    fpsText.color = COLOR_GOOD;
                else if (fps >= 25)
                    fpsText.color = COLOR_MID;
                else
                    fpsText.color = COLOR_LOW;
            }

            accumulatedTime = 0f;
            frameCount = 0;
            timer = 0f;
        }
    }
}
