using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Esta clase base gestiona la salud estándar de cualquier enemigo.
public class HealthSystem : MonoBehaviour
{
    [Header("Sistema de Vida Base")]
    public float maxHealth = 100f;
    
    // La hacemos 'protected' para que las clases hijas (como DragonHealth)
    // puedan leerla, pero otros scripts externos no.
    protected float currentHealth;

    // Usamos 'virtual' en Start para que las clases hijas puedan
    // añadir su propia lógica de Start si lo necesitan.
    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Método público principal para recibir daño.
    /// Lo marcamos como 'virtual' para que las clases hijas puedan
    /// redefinir su comportamiento.
    /// </summary>
    public virtual void TakeDamage(float amount)
    {
        // Si ya está muerto, no seguir ejecutando
        if (currentHealth <= 0)
        {
            Debug.Log(gameObject.name + " ya está muerto, ignorando daño.");
            return;
        }

        currentHealth -= amount;
        Debug.Log(gameObject.name + " recibe " + amount + " de daño. Vida restante: " + currentHealth + "/" + maxHealth);

        // Llamar a OnHealthChanged para que las clases hijas actualicen UI, etc.
        OnHealthChanged();

        // Comprobar si ha muerto
        if (currentHealth <= 0)
        {
            Debug.Log(gameObject.name + " ha llegado a 0 de vida. Llamando a Die()...");
            Die();
        }
    }

    /// <summary>
    /// Método virtual que se llama cada vez que la vida cambia.
    /// Las clases hijas pueden sobreescribirlo para actualizar UI, efectos, etc.
    /// </summary>
    protected virtual void OnHealthChanged()
    {
        // Por defecto no hace nada, las clases hijas lo implementan si lo necesitan
    }

    /// <summary>
    /// Lógica de muerte. También virtual por si un enemigo
    /// necesita morir de una forma especial (ej. explotar).
    /// </summary>
    protected virtual void Die()
    {
        Debug.Log(gameObject.name + " ha muerto.");
        Destroy(gameObject);
    }
}