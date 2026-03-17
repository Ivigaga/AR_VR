using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPositionReset : MonoBehaviour
{
    private Vector3 startPosition;
    private Quaternion startRotation;

    void Awake()
    {
        // Guardamos la posición y rotación exactas del inicio
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    void OnEnable()
    {
        // Nos suscribimos al evento de "escena cargada"
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // Nos desuscribimos al destruir o desactivar
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Este método se ejecuta automáticamente después de SceneManager.LoadScene
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
        
        // Si usas CharacterController, a veces hay que desactivarlo un milisegundo
        // para que el movimiento de posición funcione correctamente en VR.
    }
}