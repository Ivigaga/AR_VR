using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.PyroParticles; // ¡Importante!
using UnityEngine.SceneManagement;

public class GoblinAI : HealthSystem
{
    [Header("Sistema de Movimiento y Persecución")]
    public float moveSpeed = 3f;
    [Tooltip("Arrastra aquí el Transform del jugador para que el goblin lo persiga")]
    public Transform player;
    
    [Tooltip("Distancia máxima a la que el goblin detecta y persigue al jugador")]
    public float detectionRange = 15f;
    
    [Tooltip("Offset de rotación en el eje Y (usa 180 si camina de espaldas, 0 si mira bien)")]
    public float rotationYOffset = 180f;
    
    [Header("Sistema de Suelo y Gravedad")]
    [Tooltip("Si está activado, el goblin usará gravedad real para pegarse al suelo")]
    public bool useGravity = true;
    
    [Tooltip("Fuerza adicional hacia abajo para pegar mejor al suelo en pendientes")]
    public float groundStickForce = 20f;
    
    [Tooltip("Distancia para detectar si está en el suelo")]
    public float groundCheckDistance = 0.3f;
    
    [Tooltip("Layer del suelo/terreno")]
    public LayerMask groundLayer = -1; // -1 = todos los layers
    
    private bool isGrounded = false;

    private float initialMoveSpeed;
    private bool isEnraged50 = false;
    private bool isEnraged25 = false;

    [Header("Sistema de Ataque")]
    public float minAttackCooldown = 5.0f;
    public float baseAttackDelay = 3.0f;
    
    [Tooltip("Distancia a la que el goblin puede atacar al jugador")]
    public float attackRange = 3f;
    [Tooltip("Tiempo de espera antes de que el goblin empiece a actuar (útil para hordas)")]
    public float initialDelay = 0f;
    
    [Tooltip("Tiempo antes del PRIMER ataque cuando entra en rango (más rápido que los siguientes)")]
    public float firstAttackDelay = 0.5f;
    
    [Tooltip("Cantidad de daño que hace el goblin al jugador por golpe")]
    public float attackDamage = 10f;

    [Tooltip("Multiplicador del cooldown al 50% de vida")]
    public float enraged50CooldownMultiplier = 0.75f;
    
    [Tooltip("Multiplicador del cooldown al 25% de vida")]
    public float enraged25CooldownMultiplier = 0.5f;
    
    private float randomAttackDelay;
    private float attackTimer;
    private Animator animator;
    private Rigidbody rb;
    private bool isActive = false; // Controla si el goblin ya está activo
    private bool hasAttackedOnce = false; // Controla si ya hizo el primer ataque
    private bool isDead = false; // Controla si el goblin ya está muerto

    public float attackAnimationDelay = 0.5f;

    public string nextScene;

    // -------------------------------------------------------------------

    protected override void Start()
    {
        base.Start(); 

        initialMoveSpeed = moveSpeed;

        animator = GetComponent<Animator>();
        if (animator == null)
            Debug.LogWarning(gameObject.name + " no tiene un componente Animator.");
            
        // Configurar Rigidbody para evitar que se caiga o rote
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Usar gravedad real para pegarse al suelo
            rb.useGravity = useGravity;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | 
                           RigidbodyConstraints.FreezeRotationZ; // Evita que se caiga de lado
            rb.drag = 0f; // Sin resistencia para movimiento natural
            rb.angularDrag = 5f; // Resistencia a la rotación
        }
        else
        {
            Debug.LogWarning(gameObject.name + " no tiene Rigidbody. Se necesita uno para usar gravedad.");
        }
            
        // Validación del jugador
        if (player == null)
        {
            Debug.LogError(gameObject.name + ": ¡No se ha asignado el Transform del jugador! " +
                          "Asigna el jugador en el Inspector.");
        }

        // Iniciar con el timer del primer ataque (más rápido)
        attackTimer = firstAttackDelay;
        
        // Iniciar el delay inicial si es mayor que 0
        if (initialDelay > 0)
        {
            StartCoroutine(ActivateAfterDelay());
        }
        else
        {
            isActive = true; // Activar inmediatamente si no hay delay
        }
    }

    /// <summary>
    /// Corrutina para activar el goblin después del delay inicial
    /// </summary>
    private IEnumerator ActivateAfterDelay()
    {
        yield return new WaitForSeconds(initialDelay);
        isActive = true;
        Debug.Log(gameObject.name + " está ahora activo después de " + initialDelay + " segundos.");
    }

    void Update()
    {
        // Si está muerto, no hacer nada más
        if (isDead)
            return;
            
        // Si no está activo aún, no hacer nada
        if (!isActive)
            return;
            
        // Si no hay jugador asignado, no hacer nada
        if (player == null)
            return;

        // Verificar si está en el suelo
        CheckGroundStatus();

        // Calcular distancia solo en el plano horizontal (ignorando Y)
        Vector3 goblinPosFlat = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 playerPosFlat = new Vector3(player.position.x, 0, player.position.z);
        float distanceToPlayer = Vector3.Distance(goblinPosFlat, playerPosFlat);

        bool isMoving = false; // Variable para controlar la animación de caminar
        bool isInAttackRange = false; // Variable para controlar si está en rango de ataque

        // DENTRO DEL RANGO DE DETECCIÓN
        if (distanceToPlayer <= detectionRange)
        {
            // Rotar para mirar al jugador siempre que lo detecte
            LookAtPlayer();
            
            // DENTRO DEL RANGO DE ATAQUE
            if (distanceToPlayer <= attackRange)
            {
                // Está en rango de ataque: se queda quieto y ataca
                isMoving = false; // NO camina
                isInAttackRange = true; // SÍ ataca
                
                // Aplicar fuerza hacia abajo solo si está en el suelo
                if (rb != null && useGravity && isGrounded)
                {
                    rb.AddForce(Vector3.down * groundStickForce, ForceMode.Force);
                }
            }
            // FUERA DEL RANGO DE ATAQUE pero DENTRO DEL DE DETECCIÓN
            else
            {
                // Se mueve hacia el jugador (animación de caminar)
                Vector3 directionToPlayer = (playerPosFlat - goblinPosFlat).normalized;
                
                // Calcular velocidad de movimiento horizontal
                Vector3 moveDirection = new Vector3(directionToPlayer.x, 0, directionToPlayer.z);
                
                // Usar Rigidbody para movimiento con física
                if (rb != null)
                {
                    // Movimiento horizontal usando velocidad
                    Vector3 targetVelocity = moveDirection * moveSpeed;
                    targetVelocity.y = rb.velocity.y; // Mantener velocidad vertical (gravedad)
                    rb.velocity = targetVelocity;
                    
                    // Fuerza extra hacia abajo solo si está en el suelo (para pegarse en pendientes)
                    if (useGravity && isGrounded)
                    {
                        rb.AddForce(Vector3.down * groundStickForce, ForceMode.Force);
                    }
                }
                else
                {
                    // Fallback sin Rigidbody (movimiento simple)
                    Vector3 newPosition = transform.position + moveDirection * moveSpeed * Time.deltaTime;
                    transform.position = newPosition;
                }
                
                isMoving = true; // SÍ camina
                isInAttackRange = false; // NO ataca todavía
            }
        }
        // FUERA DEL RANGO DE DETECCIÓN
        else
        {
            // No hace nada, se queda quieto
            isMoving = false;
            isInAttackRange = false;
            
            // Detener movimiento horizontal pero mantener gravedad
            if (rb != null)
            {
                rb.velocity = new Vector3(0, rb.velocity.y, 0);
            }
        }

        // Actualizar animaciones
        if (animator != null)
        {
            animator.SetBool("isWalking", isMoving);
            
            // Debug: Comentar estas líneas cuando funcione
            if (isMoving != animator.GetBool("isWalking"))
            {
                Debug.Log(gameObject.name + " - isWalking cambiado a: " + isMoving + " | Distancia: " + distanceToPlayer.ToString("F2"));
            }
        }

        // Lógica de Ataque: Atacar SOLO si está en rango de ataque
        if (isInAttackRange)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                Debug.Log(gameObject.name + " - Intentando atacar. En rango: " + isInAttackRange + " | Distancia: " + distanceToPlayer.ToString("F2"));
                Attack();
            }
        }
        else
        {
            // Si sale del rango de ataque, resetear el flag del primer ataque
            // para que cuando vuelva a entrar, use el firstAttackDelay
            if (hasAttackedOnce)
            {
                hasAttackedOnce = false;
                attackTimer = firstAttackDelay; // Preparar para el próximo primer ataque
            }
        }
    }

    /// <summary>
    /// Verifica si el goblin está tocando el suelo
    /// </summary>
    void CheckGroundStatus()
    {
        if (rb == null) return;
        
        // Raycast hacia abajo desde el centro del goblin
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f; // Ligeramente arriba del centro
        
        isGrounded = Physics.Raycast(rayOrigin, Vector3.down, out hit, groundCheckDistance, groundLayer);
        
        // Debug opcional (comentar cuando funcione)
        // Debug.DrawRay(rayOrigin, Vector3.down * groundCheckDistance, isGrounded ? Color.green : Color.red);
    }

    /// <summary>
    /// Hace que el goblin mire hacia el jugador en el plano horizontal
    /// </summary>
    void LookAtPlayer()
    {
        // Calcular dirección hacia el jugador en el plano horizontal
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0; // Ignorar diferencia de altura
        
        // Si hay una dirección válida, rotar hacia ella
        if (directionToPlayer.magnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            
            // Aplicar offset de rotación (configurable en el Inspector)
            targetRotation *= Quaternion.Euler(0, rotationYOffset, 0);
            
            transform.rotation = targetRotation;
        }
    }

    // --- Funciones de Ataque ---

   /// <summary>
    /// Inicia el ataque: activa la animación y programa el daño
    /// </summary>
    private void Attack()
    {
        // 1. Activar la animación de ataque
        if (animator != null)
        {
            animator.SetTrigger("doAttack");
            Debug.Log(gameObject.name + " está ATACANDO (animación)!");
        }

        // 2. Iniciar la corrutina que hará daño después del delay
        StartCoroutine(AttackSequence());

        // 3. Marcar que ya atacó una vez
        hasAttackedOnce = true;

        // 4. Reinicia el temporizador (para el PRÓXIMO ataque)
        SetNextAttackTimer();
    }

    /// <summary>
    /// Corrutina: Espera el delay de animación y luego hace daño al jugador
    /// </summary>
    private IEnumerator AttackSequence()
    {
        // 1. Esperar a que la animación llegue al momento del golpe
        yield return new WaitForSeconds(attackAnimationDelay);

        // 2. Hacer daño al jugador si aún está en rango y vivo
        if (player != null && currentHealth > 0)
        {
            // Verificar que el jugador siga en rango de ataque
            float distanceToPlayer = Vector3.Distance(
                new Vector3(transform.position.x, 0, transform.position.z),
                new Vector3(player.position.x, 0, player.position.z)
            );
            
            if (distanceToPlayer <= attackRange)
            {
                // Intentar obtener el HealthSystem del jugador
                HealthSystem playerHealth = player.GetComponent<HealthSystem>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(attackDamage);
                    Debug.Log(gameObject.name + " golpea a " + player.name + " causando " + attackDamage + " de daño!");
                }
                else
                {
                    Debug.LogWarning(gameObject.name + " intentó hacer daño pero el jugador no tiene HealthSystem.");
                }
            }
            else
            {
                Debug.Log(gameObject.name + " falló el ataque - jugador fuera de rango.");
            }
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
        // Si ya está muerto, no recibir más daño
        if (isDead || currentHealth <= 0) return;
        
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
        // Si ya está muerto, no ejecutar de nuevo
        if (isDead) return;
        
        isDead = true; // Marcar como muerto
        Debug.Log(gameObject.name + " ha muerto.");
        
        // Detener completamente el Rigidbody
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true; // Desactivar física
        }
        
        // Activar animación de muerte
        if (animator != null)
        {
            animator.SetBool("isDead", true);
        }
        
        // Desactivar el comportamiento del goblin
        this.enabled = false;
        
        // Opcional: Desactivar el collider para que no bloquee
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }
        
        // Opcional: Cargar escena después de un delay para que se vea la animación
        if (!string.IsNullOrEmpty(nextScene))
        {
            StartCoroutine(LoadSceneAfterDelay(2f)); // 2 segundos para ver la animación
        }
    }

    /// 
    /// Corrutina para cargar la siguiente escena después de un delay
    /// 
    private IEnumerator LoadSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(nextScene);
    }
    
    private void OnDrawGizmosSelected()
    {
        // Dibuja el rango de detección (amarillo)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Dibuja el rango de ataque (rojo)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // Línea hacia el jugador si está asignado
        if (player != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
}