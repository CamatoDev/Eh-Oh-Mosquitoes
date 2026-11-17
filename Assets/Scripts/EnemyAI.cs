using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum DroneState { Patrolling, Attacking, Returning }

    [Header("Cible & État")]
    public Transform target;
    public DroneState currentState = DroneState.Patrolling;

    [Header("Ancrage Spatial & Patrouille")]
    [Tooltip("Point central d'ancrage dans la pièce (indépendant du joueur)")]
    public Vector3 patrolCenter;
    [Tooltip("Rayon de balade autour du centre d'ancrage")]
    public float patrolRadius = 2.5f;
    [Tooltip("Vitesse de croisière en patrouille")]
    public float patrolSpeed = 2.0f;
    [Tooltip("Vitesse d'orientation vers la direction de vol")]
    public float rotationSpeed = 4.0f;
    [Tooltip("Variation d'altitude pendant la patrouille")]
    public float heightVariationRange = 0.8f;
    public float heightVariationSpeed = 1.0f;

    [Header("Attaque & Piqué")]
    [Tooltip("Délai entre deux attaques en piqué")]
    public float timeBetweenAttack = 9.0f;
    [Tooltip("Vitesse lors du piqué sur le joueur")]
    public float attackSpeed = 4.0f;
    [Tooltip("Distance de contact pour piquer le joueur")]
    public float attackRange = 0.9f;
    [Tooltip("Dégâts infligés au joueur lors d'une piqûre (8 PV = survie à plus de 12 attaques)")]
    public float attackDamage = 8.0f;
    [Tooltip("Rayon de l'explosion résiduelle")]
    public float explosionRadius = 0.5f;

    [Header("Statistiques de l'Ennemi")]
    public float coin = 100f;
    public float startHealth = 100f;
    private float health;

    [Header("Dédoublement (Clonage)")]
    public float cloneCooldown = 10.0f;
    private float cloneTimer = 0.0f;
    public GameObject dronePrefab;
    public float cloneSpawnDistance = 2.5f;
    [Tooltip("Nombre maximum de moustiques simultanés")]
    public int maxDrones = 10;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip enemyDie;

    // Variables internes de vol et de suivi
    private Transform playerHead;
    private Vector3 currentPatrolTarget;
    private float waypointTimer = 0.0f;
    private float nextWaypointDelay = 3.0f;
    private float attackTimer;
    [HideInInspector] public float baseHeight;

    // Liste statique globale de tous les drones actifs
    public static readonly List<EnemyAI> allDrones = new List<EnemyAI>();

    void Start()
    {
        ResolvePlayer();

        health = startHealth > 0 ? startHealth : 100f;
        cloneTimer = cloneCooldown;
        // Ancrage spatial dans le repère de la pièce
        if (patrolCenter == Vector3.zero)
        {
            patrolCenter = transform.position;
        }
        baseHeight = transform.position.y;
        PickNewPatrolTarget();

        if (!allDrones.Contains(this))
        {
            allDrones.Add(this);
        }

        // Échelonner les attaques pour que les moustiques attaquent à tour de rôle
        int droneIndex = allDrones.IndexOf(this);
        attackTimer = timeBetweenAttack + Random.Range(2.0f, 4.0f) + (droneIndex * 4.5f);
    }

    void Update()
    {
        ResolvePlayer();

        // Gestion de la machine à états : Patrouille vs Attaque vs Retour
        switch (currentState)
        {
            case DroneState.Patrolling:
                PatrolRoom();
                UpdateAttackTimer();
                break;

            case DroneState.Attacking:
                AttackDive();
                break;

            case DroneState.Returning:
                ReturnToPatrol();
                break;
        }

        // Gestion du dédoublement périodique
        HandleCloning();
    }

    /// <summary>
    /// Trouve la tête/caméra du joueur de façon 100% compatible ARCore et Vuforia.
    /// </summary>
    void ResolvePlayer()
    {
        if (playerHead != null) return;

        if (Camera.main != null)
        {
            playerHead = Camera.main.transform;
        }
        else
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerHead = playerObj.transform;
            }
        }

        target = playerHead;
    }

    /// <summary>
    /// Vol autonome et organique dans la zone de la pièce ("se balader").
    /// </summary>
    void PatrolRoom()
    {
        waypointTimer += Time.deltaTime;
        if (waypointTimer >= nextWaypointDelay || Vector3.Distance(transform.position, currentPatrolTarget) < 0.4f)
        {
            PickNewPatrolTarget();
        }

        // Déplacement progressif vers le waypoint
        Vector3 targetPos = Vector3.MoveTowards(transform.position, currentPatrolTarget, patrolSpeed * Time.deltaTime);

        // Micro-ondulation d'insecte en vol (battements d'ailes et flottaison réaliste)
        float waveX = Mathf.Sin(Time.time * 6.5f + (float)GetInstanceID()) * 0.003f;
        float waveY = Mathf.Cos(Time.time * 8.0f + (float)GetInstanceID()) * 0.004f;
        float waveZ = Mathf.Sin(Time.time * 7.2f + (float)GetInstanceID()) * 0.003f;
        targetPos += new Vector3(waveX, waveY, waveZ);

        // Orienter le regard du moustique face à sa trajectoire
        Vector3 moveDir = (targetPos - transform.position).normalized;
        if (moveDir.sqrMagnitude > 0.0001f)
        {
            Quaternion lookRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotationSpeed * Time.deltaTime);
        }

        transform.position = targetPos;
    }

    void PickNewPatrolTarget()
    {
        Vector2 circle = Random.insideUnitCircle * patrolRadius;
        float heightOffset = Random.Range(-heightVariationRange, heightVariationRange);

        currentPatrolTarget = new Vector3(
            patrolCenter.x + circle.x,
            baseHeight + heightOffset,
            patrolCenter.z + circle.y
        );

        nextWaypointDelay = Random.Range(2.5f, 5.0f);
        waypointTimer = 0.0f;
    }

    void UpdateAttackTimer()
    {
        if (playerHead == null) return;

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
        {
            currentState = DroneState.Attacking;
        }
    }

    /// <summary>
    /// Attaque en piqué droit sur la caméra/tête du joueur.
    /// </summary>
    void AttackDive()
    {
        if (playerHead == null)
        {
            currentState = DroneState.Returning;
            return;
        }

        Vector3 targetPos = playerHead.position;
        Vector3 moveDir = (targetPos - transform.position).normalized;

        transform.position = Vector3.MoveTowards(transform.position, targetPos, attackSpeed * Time.deltaTime);

        if (moveDir.sqrMagnitude > 0.0001f)
        {
            Quaternion lookRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotationSpeed * 2.0f * Time.deltaTime);
        }

        float dist = Vector3.Distance(transform.position, targetPos);
        if (dist <= attackRange)
        {
            // Infliger les dégâts au joueur
            PlayerActions actions = playerHead.GetComponent<PlayerActions>();
            if (actions == null) actions = FindObjectOfType<PlayerActions>();
            if (actions != null)
            {
                actions.TakeDamage(attackDamage);
            }

            // Réinitialiser le timer d'attaque et repartir patrouiller loin du joueur
            Vector3 retreatDir = (transform.position - playerHead.position).normalized;
            if (retreatDir.sqrMagnitude < 0.01f) retreatDir = Vector3.back;
            currentPatrolTarget = patrolCenter + retreatDir * patrolRadius;

            attackTimer = timeBetweenAttack + Random.Range(3.0f, 6.0f);
            currentState = DroneState.Returning;
        }
    }

    /// <summary>
    /// Après une piqûre, le moustique bat en retraite vers sa zone de patrouille.
    /// </summary>
    void ReturnToPatrol()
    {
        Vector3 targetPos = Vector3.MoveTowards(transform.position, currentPatrolTarget, patrolSpeed * 1.5f * Time.deltaTime);
        Vector3 moveDir = (targetPos - transform.position).normalized;

        if (moveDir.sqrMagnitude > 0.0001f)
        {
            Quaternion lookRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotationSpeed * Time.deltaTime);
        }

        transform.position = targetPos;

        if (Vector3.Distance(transform.position, currentPatrolTarget) < 0.5f)
        {
            currentState = DroneState.Patrolling;
            waypointTimer = 0.0f;
        }
    }

    void HandleCloning()
    {
        if (dronePrefab == null || allDrones.Count >= maxDrones) return;

        cloneTimer -= Time.deltaTime;
        if (cloneTimer <= 0)
        {
            CloneDrone();
            cloneTimer = cloneCooldown * (1f + ((float)allDrones.Count / maxDrones));
        }
    }

    public void CloneDrone()
    {
        if (dronePrefab == null) return;

        Vector2 randOffset = Random.insideUnitCircle.normalized * Random.Range(1.5f, cloneSpawnDistance);
        Vector3 spawnPos = patrolCenter + new Vector3(randOffset.x, Random.Range(-0.3f, 0.4f), randOffset.y);

        GameObject clone = Instantiate(dronePrefab, spawnPos, Quaternion.identity);
        EnemyAI cloneAI = clone.GetComponent<EnemyAI>();
        if (cloneAI != null)
        {
            cloneAI.patrolCenter = spawnPos;
            cloneAI.baseHeight = spawnPos.y;
        }
    }

    public void Damage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Dead();
        }
    }

    public void Dead()
    {
        // Créditer les points au joueur (fonctionne sur ARCore et Vuforia)
        PlayerStats stats = null;
        if (playerHead != null) stats = playerHead.GetComponent<PlayerStats>();
        if (stats == null) stats = FindObjectOfType<PlayerStats>();
        if (stats != null)
        {
            stats.points += coin;
        }

        allDrones.Remove(this);

        if (enemyDie != null)
        {
            bool muted = SettingsManager.Instance != null && SettingsManager.Instance.isMuteGlobal;
            float sfxVol = SettingsManager.Instance != null ? SettingsManager.Instance.sfxVolume : 1f;
            if (!muted && sfxVol > 0.001f)
            {
                AudioSource.PlayClipAtPoint(enemyDie, transform.position, sfxVol);
            }
        }

        Destroy(gameObject);
    }

    public void Explosion()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                PlayerActions actions = hitCollider.GetComponent<PlayerActions>();
                if (actions == null) actions = FindObjectOfType<PlayerActions>();
                if (actions != null)
                {
                    actions.TakeDamage(attackDamage);
                }
            }
        }
        Dead();
    }

    // Fonctions de rétrocompatibilité pour tout appel externe
    public void OrbitPlayer() { PatrolRoom(); }
    public void RotateAroundPlayer() { }
    public void ReturnToPlayer() { ReturnToPatrol(); }
    public void VaryHeight() { }

    private void OnDestroy()
    {
        allDrones.Remove(this);
    }
}
