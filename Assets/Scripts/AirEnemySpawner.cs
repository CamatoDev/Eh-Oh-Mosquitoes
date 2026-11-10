using System.Collections;
using UnityEngine;

public class AirEnemySpawner : MonoBehaviour
{
    [Header("Préfabriqué Moustique")]
    public GameObject mosquitoPrefab;

    [Header("Configuration du Spawning")]
    [Tooltip("Nombre de moustiques au démarrage")]
    public int initialCount = 3;

    [Tooltip("Distance d'apparition devant le joueur (en mètres)")]
    public float spawnDistance = 3.5f;

    [Tooltip("Délai avant le premier spawn pour laisser le tracking AR se stabiliser")]
    public float startDelay = 0.5f;

    private Transform playerCamera;

    void Start()
    {
        // Recherche de la référence joueur / caméra
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerCamera = playerObj.transform;
        }
        else if (Camera.main != null)
        {
            playerCamera = Camera.main.transform;
        }

        StartCoroutine(SpawnInitialMosquitoes());
    }

    IEnumerator SpawnInitialMosquitoes()
    {
        yield return new WaitForSeconds(startDelay);

        if (mosquitoPrefab == null)
        {
            Debug.LogError("[AirEnemySpawner] mosquitoPrefab n'est pas assigné dans l'Inspector !");
            yield break;
        }

        if (playerCamera == null)
        {
            if (Camera.main != null)
            {
                playerCamera = Camera.main.transform;
            }
            else
            {
                Debug.LogError("[AirEnemySpawner] Caméra du joueur introuvable pour positionner les moustiques !");
                yield break;
            }
        }

        // Répartition angulaire devant la caméra du joueur
        float[] angleOffsets = initialCount switch
        {
            1 => new float[] { 0f },
            2 => new float[] { -25f, 25f },
            _ => new float[] { -35f, 0f, 35f }
        };

        for (int i = 0; i < angleOffsets.Length; i++)
        {
            Quaternion rotation = Quaternion.Euler(0, angleOffsets[i], 0);
            Vector3 forwardDirection = rotation * playerCamera.forward;

            // Légère variation verticale pour un vol naturel
            float heightOffset = (i == 1) ? 0.3f : -0.2f;

            Vector3 spawnPos = playerCamera.position + (forwardDirection * spawnDistance) + new Vector3(0, heightOffset, 0);

            GameObject spawned = Instantiate(mosquitoPrefab, spawnPos, Quaternion.identity);
            Debug.Log($"[AirEnemySpawner] Moustique #{i + 1} déployé en {spawnPos}");
        }

        Debug.Log($"[AirEnemySpawner] {angleOffsets.Length} moustiques ont été déployés en suspension dans les airs !");
    }
}
