using System.Collections;
using UnityEngine;

public class AirEnemySpawner : MonoBehaviour
{
    [Header("Préfabriqué Moustique")]
    public GameObject mosquitoPrefab;

    [Header("Configuration du Spawning")]
    [Tooltip("Nombre de moustiques au démarrage")]
    public int initialCount = 5;

    [Tooltip("Distance d'apparition devant le joueur (en mètres)")]
    public float spawnDistance = 4.2f;

    [Tooltip("Hauteur relative par rapport à la caméra")]
    public float heightOffset = 0.1f;

    [Tooltip("Délai avant le premier spawn pour laisser le tracking AR se stabiliser")]
    public float startDelay = 0.6f;

    private Transform playerCamera;

    void Start()
    {
        ResolveCamera();
        StartCoroutine(SpawnInitialMosquitoes());
    }

    void ResolveCamera()
    {
        if (Camera.main != null)
        {
            playerCamera = Camera.main.transform;
            return;
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerCamera = playerObj.transform;
        }
    }

    IEnumerator SpawnInitialMosquitoes()
    {
        yield return new WaitForSeconds(startDelay);

        if (mosquitoPrefab == null)
        {
            Debug.LogError("[AirEnemySpawner] mosquitoPrefab n'est pas assigné !");
            yield break;
        }

        ResolveCamera();
        if (playerCamera == null)
        {
            Debug.LogError("[AirEnemySpawner] Caméra du joueur introuvable pour positionner les moustiques !");
            yield break;
        }

        // Projection horizontale pour éliminer le pitch du smartphone : toujours à hauteur d'yeux
        Vector3 camForward = Vector3.ProjectOnPlane(playerCamera.forward, Vector3.up).normalized;
        if (camForward.sqrMagnitude < 0.01f)
        {
            camForward = Vector3.forward;
        }

        // Répartition en éventail panoramique dans la pièce (jusqu'à 5 moustiques)
        float[] angleOffsets = initialCount switch
        {
            1 => new float[] { 0f },
            2 => new float[] { -25f, 25f },
            3 => new float[] { -38f, 0f, 38f },
            4 => new float[] { -45f, -15f, 15f, 45f },
            5 => new float[] { -60f, -30f, 0f, 30f, 60f },
            _ => new float[] { -60f, -30f, 0f, 30f, 60f }
        };

        for (int i = 0; i < angleOffsets.Length; i++)
        {
            Quaternion rot = Quaternion.AngleAxis(angleOffsets[i], Vector3.up);
            Vector3 dir = rot * camForward;

            // Variation légère d'altitude pour un effet naturel
            float individualHeight = heightOffset + ((i % 2 == 0) ? 0.25f : -0.2f);

            Vector3 spawnPos = playerCamera.position + (dir * spawnDistance) + new Vector3(0, individualHeight, 0);

            // Orienter le moustique vers le joueur
            Vector3 lookDirection = (playerCamera.position - spawnPos).normalized;
            Quaternion spawnRot = lookDirection.sqrMagnitude > 0.001f ? Quaternion.LookRotation(lookDirection) : Quaternion.identity;

            GameObject spawned = Instantiate(mosquitoPrefab, spawnPos, spawnRot);

            // Ancrer le moustique à son emplacement spatial dans la pièce
            EnemyAI ai = spawned.GetComponent<EnemyAI>();
            if (ai != null)
            {
                ai.patrolCenter = spawnPos;
                ai.baseHeight = spawnPos.y;
            }

            Debug.Log($"[AirEnemySpawner] Moustique #{i + 1} déployé en position spatiale {spawnPos}");
        }

        Debug.Log($"[AirEnemySpawner] {angleOffsets.Length} moustiques déployés avec succès dans l'espace 3D de la pièce !");
    }
}
