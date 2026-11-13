using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalEnemyAI : MonoBehaviour
{
    public enum BossState { Patrolling, Attacking, Returning }

    [Header("Cible & État")]
    public Transform target;
    public BossState currentState = BossState.Patrolling;

    [Header("Ancrage Spatial & Patrouille Boss")]
    [Tooltip("Point central d'ancrage dans la pièce (indépendant du joueur)")]
    public Vector3 patrolCenter;
    public float patrolRadius = 3.5f;
    public float patrolSpeed = 2.5f;
    public float rotationSpeed = 3.5f;
    public float heightVariationRange = 1.0f;

    [Header("Attaque & Piqué Boss")]
    public float timeBetweenAttack = 8.0f;
    public float attackSpeed = 4.5f;
    public float attackRange = 1.2f;
    public float attackDamage = 35.0f;
    public float explosionRadius = 2.0f;

    [Header("Statistiques du Boss")]
    public float coin = 250f;
    public float startHealth = 300f;
    private float health;

    [Header("Dédoublement (Invocation d'escorte)")]
    public float cloneCooldown = 5.0f;
    private float cloneTimer = 0.0f;
    public GameObject dronePrefab;
    public float cloneSpawnDistance = 3.0f;
    public int maxDrones = 4;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip enemyDie;
    public AudioClip enemySpawn;

    // Variables internes
    private Transform playerHead;
    private Vector3 currentPatrolTarget;
    private float waypointTimer = 0.0f;
    private float nextWaypointDelay = 3.0f;
    private float attackTimer;
    private float baseHeight;

    private static List<FinalEnemyAI> allFinalDrones = new List<FinalEnemyAI>();

    void Start()
    {
        ResolvePlayer();

        health = startHealth > 0 ? startHealth : 300f;
        cloneTimer = cloneCooldown;
        attackTimer = timeBetweenAttack + Random.Range(1f, 3f);

        if (patrolCenter == Vector3.zero)
        {
            patrolCenter = transform.position;
        }
        baseHeight = transform.position.y;

        PickNewPatrolTarget();

        if (enemySpawn != null)
        {
            AudioSource.PlayClipAtPoint(enemySpawn, transform.position);
        }

        if (!allFinalDrones.Contains(this))
        {
            allFinalDrones.Add(this);
        }
    }

    void Update()
    {
        ResolvePlayer();

        switch (currentState)
        {
            case BossState.Patrolling:
                PatrolRoom();
                UpdateAttackTimer();
                break;

            case BossState.Attacking:
                AttackDive();
                break;

            case BossState.Returning:
                ReturnToPatrol();
                break;
        }

        HandleCloning();
    }

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

    void PatrolRoom()
    {
        waypointTimer += Time.deltaTime;
        if (waypointTimer >= nextWaypointDelay || Vector3.Distance(transform.position, currentPatrolTarget) < 0.5f)
        {
            PickNewPatrolTarget();
        }

        Vector3 targetPos = Vector3.MoveTowards(transform.position, currentPatrolTarget, patrolSpeed * Time.deltaTime);

        // Vol imposant et lourd pour le Boss
        float waveX = Mathf.Sin(Time.time * 4.5f) * 0.005f;
        float waveY = Mathf.Cos(Time.time * 5.0f) * 0.005f;
        float waveZ = Mathf.Sin(Time.time * 4.0f) * 0.005f;
        targetPos += new Vector3(waveX, waveY, waveZ);

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

        nextWaypointDelay = Random.Range(3.0f, 6.0f);
        waypointTimer = 0.0f;
    }

    void UpdateAttackTimer()
    {
        if (playerHead == null) return;

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
        {
            currentState = BossState.Attacking;
        }
    }

    void AttackDive()
    {
        if (playerHead == null)
        {
            currentState = BossState.Returning;
            return;
        }

        Vector3 targetPos = playerHead.position;
        Vector3 moveDir = (targetPos - transform.position).normalized;

        transform.position = Vector3.MoveTowards(transform.position, targetPos, attackSpeed * Time.deltaTime);

        if (moveDir.sqrMagnitude > 0.0001f)
        {
            Quaternion lookRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotationSpeed * 2f * Time.deltaTime);
        }

        float dist = Vector3.Distance(transform.position, targetPos);
        if (dist <= attackRange)
        {
            PlayerActions actions = playerHead.GetComponent<PlayerActions>();
            if (actions == null) actions = FindObjectOfType<PlayerActions>();
            if (actions != null)
            {
                actions.TakeDamage(attackDamage);
            }

            attackTimer = timeBetweenAttack + Random.Range(0f, 2f);
            currentState = BossState.Returning;
            PickNewPatrolTarget();
        }
    }

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

        if (Vector3.Distance(transform.position, currentPatrolTarget) < 0.6f)
        {
            currentState = BossState.Patrolling;
            waypointTimer = 0.0f;
        }
    }

    void HandleCloning()
    {
        if (dronePrefab == null || allFinalDrones.Count >= maxDrones) return;

        cloneTimer -= Time.deltaTime;
        if (cloneTimer <= 0)
        {
            CloneDrone();
            cloneTimer = cloneCooldown * (1f + ((float)allFinalDrones.Count / maxDrones));
        }
    }

    public void CloneDrone()
    {
        if (dronePrefab == null) return;

        Vector2 randOffset = Random.insideUnitCircle.normalized * Random.Range(2.0f, cloneSpawnDistance);
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
        PlayerStats stats = null;
        if (playerHead != null) stats = playerHead.GetComponent<PlayerStats>();
        if (stats == null) stats = FindObjectOfType<PlayerStats>();
        if (stats != null)
        {
            stats.points += coin;
        }

        allFinalDrones.Remove(this);

        if (enemyDie != null)
        {
            AudioSource.PlayClipAtPoint(enemyDie, transform.position);
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

    // Rétrocompatibilité
    public void OrbitPlayer() { PatrolRoom(); }
    public void RotateAroundPlayer() { }
    public void ReturnToPlayer() { ReturnToPatrol(); }
    public void VaryHeight() { }

    private void OnDestroy()
    {
        allFinalDrones.Remove(this);
    }
}
