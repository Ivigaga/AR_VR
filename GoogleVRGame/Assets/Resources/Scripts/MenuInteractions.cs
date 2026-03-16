using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Para cargar escenas

public class MenuInteractions : MonoBehaviour
{
    [Header("Configuración de Puerta")]
    public string sceneToLoad; // Nombre de la escena a cargar

    /// <summary>
    /// Método público para interactuar con la puerta y cambiar de escena
    /// </summary>
    public void InteraccionarConPuerta()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.Log("Interactuando con puerta. Cargando escena: " + sceneToLoad);
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogError("No se ha asignado una escena a cargar en " + gameObject.name);
        }
    }

    /// <summary>
    /// Método para salir del juego (cerrar la aplicación)
    /// </summary>
    public void IrADormir()
    {
        Debug.Log("Saliendo del juego...");
        
        #if UNITY_EDITOR
            // Si estamos en el editor, detener el modo de juego
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // Si estamos en una build, cerrar la aplicación
            Application.Quit();
        #endif
    }
}
