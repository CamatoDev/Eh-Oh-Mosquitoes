using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Indicateur radar 360° en réalité augmentée.
/// Affiche une flèche à l'écran pointant dynamiquement vers le moustique le plus proche,
/// permettant au joueur de ne jamais être perdu dans l'espace 3D de la pièce.
/// </summary>
public class MosquitoRadarArrow : MonoBehaviour
{
    [Header("Références")]
    [Tooltip("Caméra principale (AR Camera)")]
    public Camera targetCamera;

    [Tooltip("Composant Image de la flèche")]
    public Image arrowImage;

    [Tooltip("RectTransform de la flèche")]
    public RectTransform arrowRect;

    [Header("Paramètres d'Affichage")]
    [Tooltip("Rayon du cercle radar autour du centre de l'écran (en unités Canvas)")]
    public float radarRadius = 220f;

    [Tooltip("Échelle normale de la flèche")]
    public Vector3 normalScale = new Vector3(1f, 1f, 1f);

    [Header("Couleurs selon la Proximité")]
    public Color farColor = new Color(1.0f, 0.85f, 0.1f, 0.95f);   // Jaune (> 4m)
    public Color midColor = new Color(1.0f, 0.55f, 0.05f, 0.95f);  // Orange (2m - 4m)
    public Color closeColor = new Color(1.0f, 0.15f, 0.15f, 1.0f); // Rouge vif (< 2m)

    void Awake()
    {
        if (arrowRect == null)
        {
            arrowRect = GetComponent<RectTransform>();
        }
        if (arrowImage == null)
        {
            arrowImage = GetComponent<Image>();
        }
        ResolveCamera();
    }

    void OnEnable()
    {
        ResolveCamera();
    }

    void ResolveCamera()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }

    void LateUpdate()
    {
        // Vérifier si le radar est activé dans les paramètres du joueur
        if (SettingsManager.Instance != null)
        {
            if (!SettingsManager.Instance.radarEnabled)
            {
                if (arrowImage != null && arrowImage.enabled) arrowImage.enabled = false;
                return;
            }
            radarRadius = SettingsManager.Instance.radarRadius;
        }

        if (targetCamera == null)
        {
            ResolveCamera();
            if (targetCamera == null)
            {
                if (arrowImage != null) arrowImage.enabled = false;
                return;
            }
        }

        // Trouver le moustique le plus proche (drones standards + boss)
        Vector3 playerPos = targetCamera.transform.position;
        Vector3 closestEnemyPos = Vector3.zero;
        float minDistanceSqr = float.MaxValue;
        bool foundEnemy = false;

        // 1. Drones normaux
        for (int i = EnemyAI.allDrones.Count - 1; i >= 0; i--)
        {
            EnemyAI drone = EnemyAI.allDrones[i];
            if (drone == null || !drone.gameObject.activeInHierarchy)
            {
                continue;
            }

            float distSqr = (drone.transform.position - playerPos).sqrMagnitude;
            if (distSqr < minDistanceSqr)
            {
                minDistanceSqr = distSqr;
                closestEnemyPos = drone.transform.position;
                foundEnemy = true;
            }
        }

        // 2. Boss / Ennemi Final
        for (int i = FinalEnemyAI.allFinalDrones.Count - 1; i >= 0; i--)
        {
            FinalEnemyAI boss = FinalEnemyAI.allFinalDrones[i];
            if (boss == null || !boss.gameObject.activeInHierarchy)
            {
                continue;
            }

            float distSqr = (boss.transform.position - playerPos).sqrMagnitude;
            if (distSqr < minDistanceSqr)
            {
                minDistanceSqr = distSqr;
                closestEnemyPos = boss.transform.position;
                foundEnemy = true;
            }
        }

        // Aucun moustique présent
        if (!foundEnemy)
        {
            if (arrowImage != null && arrowImage.enabled)
            {
                arrowImage.enabled = false;
            }
            return;
        }

        // Activer l'affichage
        if (arrowImage != null && !arrowImage.enabled)
        {
            arrowImage.enabled = true;
        }

        float actualDistance = Mathf.Sqrt(minDistanceSqr);

        // Projection sur l'écran
        Vector3 screenPos = targetCamera.WorldToScreenPoint(closestEnemyPos);
        bool isBehind = screenPos.z < 0;

        if (isBehind)
        {
            // Inversion pour les cibles situées derrière la caméra
            screenPos *= -1f;
        }

        Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        Vector2 screenDir = new Vector2(screenPos.x - screenCenter.x, screenPos.y - screenCenter.y);

        if (screenDir.sqrMagnitude < 0.001f)
        {
            screenDir = isBehind ? Vector2.down : Vector2.up;
        }
        else
        {
            screenDir.Normalize();
        }

        // Positionnement sur le cercle radar
        if (arrowRect != null)
        {
            arrowRect.anchoredPosition = screenDir * radarRadius;

            // Rotation : aligner la flèche (qui pointe vers le haut par défaut)
            float angle = Mathf.Atan2(screenDir.y, screenDir.x) * Mathf.Rad2Deg;
            arrowRect.localEulerAngles = new Vector3(0, 0, angle - 90f);

            // Effet d'échelle pulsée si le moustique est très proche (< 2 mètres)
            if (actualDistance < 2.0f)
            {
                float pulse = 1.0f + 0.18f * Mathf.Sin(Time.time * 10f);
                arrowRect.localScale = normalScale * pulse;
            }
            else
            {
                arrowRect.localScale = normalScale;
            }
        }

        // Teinte dynamique selon la distance
        if (arrowImage != null)
        {
            if (actualDistance < 2.0f)
            {
                arrowImage.color = closeColor;
            }
            else if (actualDistance < 4.0f)
            {
                arrowImage.color = midColor;
            }
            else
            {
                arrowImage.color = farColor;
            }
        }
    }
}
