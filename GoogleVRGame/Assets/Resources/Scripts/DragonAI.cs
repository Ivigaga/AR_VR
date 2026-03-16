using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.PyroParticles; // ¡Importante!
using UnityEngine.SceneManagement;

public class DragonAI : HealthSystem
{
    [Header("Sistema de Movimiento")]
    public float moveSpeed = 3f;
    public float wanderRadius = 5f;
    
    private Vector3 centerPoint;
    private Vector3 targetPosition;

    private float initialMoveSpeed;
    private bool isEnraged50 = false;
    private bool isEnraged25 = false;

    [Header("Sistema de Ataque")]
    public float minAttackCooldown = 5.0f;
    public float baseAttackDelay = 3.0f;
    
    [Tooltip("El objeto al que el dragón disparará (ej. el Jugador)")]
    public Transform attackTarget; 

    // --- NUEVA VARIABLE DE SPAWN ---
    [Tooltip("El punto exacto desde donde disparará (ej. la boca)")]
    public Transform projectileSpawnPoint; // Arrastra el objeto de la boca aquí
    // ---------------------------------

    [Tooltip("Multiplicador del cooldown al 50% de vida")]
    public float enraged50CooldownMultiplier = 0.75f; 
    
    [Tooltip("Multiplicador del cooldown al 25% de vida")]
    public float enraged25CooldownMultiplier = 0.5f;
    
    private float randomAttackDelay;
    private float attackTimer;
    private Animator animator;
    private FireAttack magicScript; 

    public float attackAnimationDelay = 0.5f;

    public string nextScene;

    // -------------------------------------------------------------------

    protected override void Start()
    {
        base.Start(); 

        centerPoint = transform.position;
        initialMoveSpeed = moveSpeed;
        PickNewTargetPosition();
        randomAttackDelay = baseAttackDelay;

        animator = GetComponent<Animator>();
        if (animator == null)
            Debug.LogWarning(gameObject.name + " no tiene un componente Animator.");

        magicScript = GetComponent<FireAttack>();
        if (magicScript == null)
            Debug.LogWarning(gameObject.name + " no tiene el script 'Magic' para disparar.");
            
        // --- NUEVA COMPROBACIÓN ---
        // Si no se asigna un spawnPoint, usará el centro del dragón por defecto
        if (projectileSpawnPoint == null)
        {
            Debug.LogWarning(gameObject.name + ": 'projectileSpawnPoint' no está asignado. " +
                             "El proyectil saldrá del centro del dragón (transform).");
            // Opcional: Asignar el propio transform como fallback
            // projectileSpawnPoint = transform; 
        }
        // --------------------------

        SetNextAttackTimer();
    }

    void Update()
    {
        // Lógica de Movimiento
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            PickNewTargetPosition();

        // Lógica de Ataque
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
            Attack();
    }

    void PickNewTargetPosition()
    {
        Vector2 randomDirection = Random.insideUnitCircle * wanderRadius;
        targetPosition = centerPoint + new Vector3(randomDirection.x, randomDirection.y, 0);
    }

    // --- Funciones de Ataque ---

   /// <summary>
    /// MODIFICADO: Ahora inicia la corrutina de ataque en lugar de disparar
    /// </summary>
    private void Attack()
    {
        // 1. Activar la animación
        if (animator != null)
        {
            animator.SetTrigger("Attack"); 
            Debug.Log(gameObject.name + " está ATACANDO (animación)!");
        }

        // 2. Iniciar la corrutina que disparará después del delay
        StartCoroutine(AttackSequence());

        // 3. Reinicia el temporizador (para el PRÓXIMO ataque)
        //    (Esto se queda aquí para que el cooldown empiece a contar ya)
        SetNextAttackTimer();
    }

    /// <summary>
    /// NUEVA CORRUTINA: Espera y luego dispara el proyectil.
    /// </summary>
    private IEnumerator AttackSequence()
    {
        // 1. Esperar el tiempo de delay especificado
        yield return new WaitForSeconds(attackAnimationDelay);

        // 2. Disparar el proyectil (esta es la lógica que movimos)
        Debug.Log(gameObject.name + " ¡Dispara el proyectil!");
        if (magicScript != null && attackTarget != null)
        {
            magicScript.StartCurrent(attackTarget, projectileSpawnPoint);
        }
        else
        {
            if (magicScript == null) Debug.LogWarning(gameObject.name + " intentó disparar pero no tiene 'Magic'.");
            if (attackTarget == null) Debug.LogWarning(gameObject.name + " intentó disparar pero no tiene 'attackTarget'.");
        }
    }

    private void SetNextAttackTimer()
    {
        float baseCooldown = minAttackCooldown + Random.Range(0, randomAttackDelay);
        attackTimer = baseCooldown;
    }

    // --- Sistema de Vida (sin cambios) ---

    public override void TakeDamage(float amount)
    {
        if (currentHealth <= 0) return;
        currentHealth -= amount; 
        Debug.Log(gameObject.name + " tiene " + currentHealth + " de vida.");

        if (currentHealth <= 0)
        {
            Die();
        }
        else if (!isEnraged25 && currentHealth <= (maxHealth * 0.25f))
        {
            Debug.Log(gameObject.name + " ENFURECIDO (25%)! Velocidad x2!");
            isEnraged25 = true; 
            moveSpeed = initialMoveSpeed * 2f;
            randomAttackDelay = baseAttackDelay * enraged25CooldownMultiplier; 
        }
        else if (!isEnraged50 && currentHealth <= (maxHealth * 0.5f))
        {
            Debug.Log(gameObject.name + " ENFURECIDO (50%)! Velocidad x1.5!");
            isEnraged50 = true; 
            moveSpeed = initialMoveSpeed * 1.5f; 
            randomAttackDelay = baseAttackDelay * enraged50CooldownMultiplier; 
        }
    }

    protected override void Die()
    {
        Debug.Log(gameObject.name + " ha muerto.");
        SceneManager.LoadScene(nextScene);
    }
    
    private void OnDrawGizmosSelected()
    {
        Vector3 center = Application.isPlaying ? centerPoint : transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, wanderRadius);
    }
}