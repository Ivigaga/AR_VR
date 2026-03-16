using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Chest : MonoBehaviour
{
    [Header("Configuración")]
    // Arrastra aquí en el inspector el objeto que quieres que cambie de tamaño
    public Transform objetoParaAgrandar;

    private Animator animator;
    public AudioSource audioSource = null;
    public AudioClip audioClip = null;
    private bool open=false;
    public string startScene="MenuScene";

    void Start()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("¡El cofre no tiene un componente Animator!");
        }
    }

    void Update()
    {

    }

    public void OpenChest()
    {
        // 1. Activar animación
        if (animator != null)
        {
            animator.SetTrigger("Open");
        }

        // 2. Cambiar el tamaño del objeto
            if (objetoParaAgrandar != null)
            {
                objetoParaAgrandar.localScale = new Vector3(12f, 12f, 12f);
                if(!open){// INICIO DEL CAMBIO: En lugar de reproducir directamente, iniciamos la corrutina
            StartCoroutine(PlayAudioAndLoadScene(0.3f));
            open=true;}
            
            }
            // FIN DEL CAMBIO
        
        else
        {
            Debug.LogWarning("Se intentó abrir el cofre, pero no hay ningún 'objetoParaAgrandar' asignado en el Inspector.");
        }
    }

    // NUEVO MÉTODO: Corrutina para esperar
    private IEnumerator PlayAudioAndLoadScene(float initialDelay)
    {
        
        // Espera inicial antes de que suene el audio (0.3s según tu llamada)
        yield return new WaitForSeconds(initialDelay);

        // Reproduce el audio
        if (audioSource != null && audioClip != null)
        {
            audioSource.PlayOneShot(audioClip);
        }
        else
        {
             Debug.LogWarning("Falta AudioSource o AudioClip en el cofre.");
        }

        // NUEVA ESPERA: 0.5 segundos después de que inicia el sonido
        yield return new WaitForSeconds(2f);

        // Carga la escena
        Debug.Log("Cargando escena: " + startScene);
        SceneManager.LoadScene(startScene);
    }
}