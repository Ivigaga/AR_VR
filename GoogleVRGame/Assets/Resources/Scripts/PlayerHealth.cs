using UnityEngine;
using UnityEngine.UI; // Necesario para la UI (Slider, Image)
using UnityEngine.SceneManagement; // Necesario para cargar escenas
using System.Collections; // Necesario para Coroutines

public class PlayerHealth : HealthSystem
{
    // --- Referencias de UI ---
    // 1. La Barra de Vida
    public Slider healthSlider; 

    // 2. El Efecto de Daño (Vignette)
    public Image damageEffectImage; 

    public float damageEffectFadeSpeed = 1.5f; // Qué tan rápido se quita el efecto
    private Color damageEffectColor; // Color del efecto (rojo)

    // --- Configuración de Muerte ---
    public string deathScene; // Escena a cargar cuando muera el jugador

    protected override void Start()
    {
        // Llama al Start de la clase base (HealthSystem)
        base.Start();
        
        // Configura el Slider
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        // Configura el efecto de daño
        if (damageEffectImage != null)
        {
            damageEffectColor = damageEffectImage.color;
            // Asegúrate de que empieza totalmente transparente
            damageEffectColor.a = 0f;
            damageEffectImage.color = damageEffectColor;
        }
    }

    // Sobrescribe TakeDamage para añadir efectos visuales
    public override void TakeDamage(float amount)
    {
        // Llama al TakeDamage de la clase base (gestiona currentHealth y muerte)
        base.TakeDamage(amount);

        // Activa el efecto visual
        StartCoroutine(ShowDamageEffect());
    }

    // Este método se llama automáticamente cuando la vida cambia
    protected override void OnHealthChanged()
    {
        // Actualiza la barra de vida
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }

    IEnumerator ShowDamageEffect()
    {
        if (damageEffectImage == null) yield break; // Salir si no hay imagen

        // 1. Muestra el efecto (ponlo rojo y visible)
        damageEffectColor.a = 0.5f; // Opacidad de 50%
        damageEffectImage.color = damageEffectColor;

        // 2. Espera un frame
        yield return null;

        // 3. Desvanece el efecto
        while (damageEffectColor.a > 0f)
        {
            // Baja el alfa (transparencia) poco a poco
            damageEffectColor.a -= Time.deltaTime * damageEffectFadeSpeed;
            damageEffectImage.color = damageEffectColor;
            yield return null;
        }
    }

    protected override void Die()
    {
        Debug.Log("¡El jugador ha muerto!");
        
        // Cargar escena de muerte si está configurada
        if (!string.IsNullOrEmpty(deathScene))
        {
            SceneManager.LoadScene(deathScene);
        }
        else
        {
            Debug.LogWarning("No se ha configurado una escena de muerte en PlayerHealth");
        }
    }
}
