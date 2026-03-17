using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    [Header("Referencias")]
    public ParticleSystem llama;
    public GameObject character;  // Personaje principal
    public GameObject character2; // Personaje fantasma/clon

    [Header("Parámetros de la Llama")]
    public float velocidadCarga = 0.5f;     
    public float velocidadExplosion = 10f;  

    [Header("Configuración de Tiempos Ataque")]
    public float tiempoCarga = 2.0f;        
    public float tiempoExplosion = 1.0f;    
    public float tiempoEnfriamiento = 0.5f; 

    [Header("Configuración Boost")]
    public float distanciaRetrocesoBoost = 2.0f; // Cuánto aparece hacia atrás character 2
    public float distanciaAvanceBoost = 10.0f;   // Cuánto recorre hacia adelante character 2
    public float duracionMovimientoBoost = 0.5f; // Cuánto tarda en hacer el recorrido
    public ParticleSystem boostParticles;
    public ParticleSystem boostParticles2;

    private bool attacking; 
    
    // Animators separados
    private Animator animator1; // Del character principal
    private Animator animator2; // Del character 2 (clon)
    
    private Vector3 escalaInicialDefecto;

    [Header("Audio")]
    public AudioSource sonidoBoost;
    public AudioSource sonidoAttack;
    public AudioSource voice;

    

    private void Start()
    {
        // 1. Configurar Animator Principal
        if (character != null)
        {
            animator1 = character.GetComponent<Animator>();
            if (animator1 == null) animator1 = character.GetComponentInChildren<Animator>();
        }

        // 2. Configurar Animator Secundario (Character 2)
        if (character2 != null)
        {
            animator2 = character2.GetComponent<Animator>();
            if (animator2 == null) animator2 = character2.GetComponentInChildren<Animator>();
            
            // Aseguramos que empiece apagado
            character2.SetActive(false);
        }

        // Configuración Llama
        if (llama != null)
        {
            escalaInicialDefecto = llama.transform.localScale;
        }
    }

    public void Attack()
    {
        if (!attacking)
        {
            StartCoroutine(SecuenciaAtaque());
        }
    }

    public void Boost()
    {
        if (!attacking)
        {
            StartCoroutine(SecuenciaBoost());
        }
    }

    // --- CORRUTINA DE ATAQUE (BOLA DE FUEGO) ---
    IEnumerator SecuenciaAtaque()
    {
        attacking = true;

        if (animator1 != null) animator1.SetTrigger("Attack");
        if (voice != null) voice.Play();

        if (llama != null)
        {
            yield return new WaitForSeconds(0.2f);
            
            llama.transform.localScale = Vector3.zero;
            llama.Play();

            // FASE 1: CARGA 
            

            float timer = 0f;
            while (timer < tiempoCarga)
            {
                llama.transform.localScale += Vector3.one * velocidadCarga * Time.deltaTime;
                timer += Time.deltaTime;
                yield return null;
            }
            if (sonidoAttack != null) sonidoAttack.Play();
            // FASE 2: EXPLOSIÓN 
            timer = 0f;
            while (timer < tiempoExplosion)
            {
                llama.transform.localScale += Vector3.one * velocidadExplosion * Time.deltaTime;
                timer += Time.deltaTime;
                yield return null;
            }

            // FASE 3: FIN 
            llama.Stop();

            yield return new WaitForSeconds(tiempoEnfriamiento);
            llama.transform.localScale = Vector3.zero;
        }

        attacking = false;
    }

    // --- CORRUTINA DE BOOST (DASH DEL CLON) ---
    IEnumerator SecuenciaBoost()
    {
        if(attacking) yield break;
        attacking = true;

        // Guardamos posición inicial del principal
        Vector3 posicionInicialChar1 = character.transform.position;

        if (character2 != null)
        {
            // 1. Activar Character 2
            character2.SetActive(true);

            // 2. Reproducir Audio Boost (Play)
            if (sonidoBoost != null) sonidoBoost.Play();
            boostParticles2.Play();
            // 3. Teletransportar Character 1 atrás
            Vector3 posicionAtras = character.transform.position - (character.transform.forward * distanciaRetrocesoBoost);
            character.transform.position = posicionAtras;

            // 4. Activar Trigger en AMBOS animators
            if (animator1 != null) animator1.SetTrigger("Boost");
            if (animator2 != null) animator2.SetTrigger("Boost");

            // 5. Mover Character 2 hacia delante
            float timer = 0f;
            Vector3 startPos = character2.transform.position;
            Vector3 endPos = startPos + (character2.transform.forward * (distanciaAvanceBoost));

            while (timer < duracionMovimientoBoost)
            {
                character2.transform.position = Vector3.Lerp(startPos, endPos, timer / duracionMovimientoBoost);
                timer += Time.deltaTime;
                yield return null;
            }
            character2.transform.position = endPos;
            boostParticles2.Stop();
            // 6. Desactivar Character 2
            character2.SetActive(false);

            // 7. Mover Character1 hacia delante
            timer = 0f;
            startPos = posicionAtras;
            endPos = posicionInicialChar1;
            boostParticles.Play();
            while (timer < duracionMovimientoBoost)
            {
                character.transform.position = Vector3.Lerp(startPos, endPos, timer / duracionMovimientoBoost);
                timer += Time.deltaTime;
                yield return null;
            }
            character.transform.position = endPos;
            character2.transform.position = endPos;
            boostParticles.Stop();
            // 9. Activar Trigger en AMBOS animators
            if (animator1 != null) animator1.SetTrigger("Boost");
            if (animator2 != null) animator2.SetTrigger("Boost");
        }

        // 7. Restaurar posición Character 1
        if (character != null)
        {
            character.transform.position = posicionInicialChar1;
        }

        // 8. Parar el Audio Boost (Stop) al terminar la secuencia
        if (sonidoBoost != null) sonidoBoost.Stop();

        attacking = false; 
    }
}